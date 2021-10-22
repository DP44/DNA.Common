using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Profiling
{
	public class Profiler : DrawableGameComponent
	{
		public class ProfileEvent : IProfilerLinkedListNode, IDisposable
		{
			public string _name;
			public int _threadIndex;
			public long _startTime;
			public long _endTime;

			private IProfilerLinkedListNode _next;

			public ProfileEvent()
			{
				this._name = "Unnamed";
				this._threadIndex = -1;
				this._startTime = 0L;
				this._endTime = 0L;
			}

			public void Init(string name) =>
				this.Init(name, ProfilerUtils.ThreadIndex);

			public void Init(string name, ProfilerThreadEnum threadIndex)
			{
				this._name = name;
				this._threadIndex = (int)threadIndex;
				this._startTime = Stopwatch.GetTimestamp();
				this._endTime = this._startTime;
			}

			public void Dispose() =>
				this._endTime = Stopwatch.GetTimestamp();

			public IProfilerLinkedListNode NextNode
			{
				get =>
					return this._next;

				set =>
					this._next = value;
			}
		}

		private static Profiler _theInstance;
		
		private static Profiler.ProfileEvent _dummyEvent = 
			new Profiler.ProfileEvent();
		
		private ProfilerObjectCache<Profiler.ProfileEvent> _eventPool;
		private ProfilerLockFreeStack<Profiler.ProfileEvent> _activeEvents;
		private List<Profiler.ProfileEvent> _eventStack;
		private Profiler.ProfileEvent _eventsToBeReported;
		private Profiler.ProfileEvent _gfxEvent;
		private ProfilerPrimitiveBatch _primitiveBatch;
		private SpriteBatch _spriteBatch;
		private ProfilerSimpleStack<Profiler.ProfileEvent>[] _eventLists;
		private Dictionary<string, Color> _colorDict;
		private ProfilerCircularQueue<float> _memSamples;
		private float _sampleWaitTime;
		private string[] _sectionNames;
		private Color[] _sectionColors;
		private Vector2[] _sectionSizes;
		private Vector2 _stringSize;
		private double _ticksToMilliseconds;
		private long _frameStart;
		private long _newFrameStart;
		private long _frameEnd;
		private bool _wantProfiling;
		private bool _profiling;
		private bool _frameMarked;

		public static Profiler CreateComponent(Game game)
		{
			if (Profiler._theInstance == null)
			{
				Profiler._theInstance = new Profiler(game);
				Profiler._theInstance.UpdateOrder = int.MaxValue;
				Profiler._theInstance.DrawOrder = int.MaxValue;
				game.Components.Add(Profiler._theInstance);
			}

			return Profiler._theInstance;
		}

		public static Profiler Instance =>
			Profiler._theInstance;

		public static Profiler.ProfileEvent TimeSection(string name) =>
			Profiler.TimeSection(name, ProfilerUtils.ThreadIndex);

		public static Profiler.ProfileEvent TimeSection(string name, ProfilerThreadEnum tindex)
		{
			if (!Profiler.Profiling)
			{
				return Profiler._dummyEvent;
			}

			return Profiler._theInstance.InternalTimeSection(name, tindex);
		}

		public static void MarkFrame()
		{
			if (Profiler.Profiling)
			{
				Profiler._theInstance.InternalMarkFrame();
			}
		}

		public static void SetColor(string name, Color color)
		{
			if (Profiler.ProfilingAvailable)
			{
				Profiler._theInstance._colorDict.Add(name, color);
				Profiler._theInstance._sectionNames = null;
				Profiler._theInstance._sectionColors = null;
				Profiler._theInstance._sectionSizes = null;
			}
		}

		public static bool ProfilingAvailable =>
			Profiler._theInstance != null;

		public static bool Profiling
		{
			get =>
				Profiler.ProfilingAvailable && Profiler._theInstance._wantProfiling;

			set
			{
				if (Profiler.ProfilingAvailable)
				{
					Profiler._theInstance._wantProfiling = value;
				}
			}
		}

		private Profiler(Game game) : base(game)
		{
			this._eventPool = new ProfilerObjectCache<Profiler.ProfileEvent>();
			this._activeEvents = new ProfilerLockFreeStack<Profiler.ProfileEvent>();
			this._wantProfiling = false;
			this._profiling = false;
			
			Profiler._dummyEvent = new Profiler.ProfileEvent();
			Profiler._dummyEvent.Init("Dummy");
			
			this._ticksToMilliseconds = 1000.0 / (double)Stopwatch.Frequency;
			this._colorDict = new Dictionary<string, Color>();
			this._eventLists = new ProfilerSimpleStack<Profiler.ProfileEvent>[4];
			this._memSamples = new ProfilerCircularQueue<float>(120);
			this._sampleWaitTime = 0f;
			this._eventStack = new List<Profiler.ProfileEvent>(10);
			this._frameMarked = false;
			
			for (int i = 0; i < 4; i++)
			{
				this._eventLists[i] = new ProfilerSimpleStack<Profiler.ProfileEvent>();
			}
		}

		private Profiler.ProfileEvent InternalTimeSection(string name, ProfilerThreadEnum tindex)
		{
			Profiler.ProfileEvent profileEvent = this._eventPool.Get();

			// Keep track of the current event.
			this._activeEvents.Push(profileEvent);

			// Initalize the event.
			profileEvent.Init(name, tindex);

			// Return the ProfileEvent object.
			return profileEvent;
		}

		public override void Initialize()
		{
			base.Initialize();
			this._spriteBatch = new SpriteBatch(base.Game.GraphicsDevice);
			this._primitiveBatch = new ProfilerPrimitiveBatch(base.Game.GraphicsDevice);
			Profiler.SetColor("XNA (Graphics)", Color.Maroon);
		}

		private float GetStartMillis(Profiler.ProfileEvent e) =>
			(float)((double)(e._startTime - this._frameStart) * this._ticksToMilliseconds);

		private float GetElapsedMillis(Profiler.ProfileEvent e) =>
			return (float)((double)(e._endTime - e._startTime) * this._ticksToMilliseconds);

		private bool GetSectionColors()
		{
			if (this._colorDict.Count == 0 || ProfilerUtils.SystemFont == null)
			{
				return false;
			}
			
			if (this._sectionNames == null)
			{
				this._sectionNames = this._colorDict.Keys.ToArray<string>();
				this._sectionColors = this._colorDict.Values.ToArray<Color>();
				this._sectionSizes = new Vector2[this._sectionNames.Length];
				this._stringSize = Vector2.Zero;
				int num = 0;
			
				foreach (string text in this._sectionNames)
				{
					this._sectionSizes[num] = ProfilerUtils.SystemFont.MeasureString(text);
					this._stringSize = Vector2.Max(this._sectionSizes[num++], this._stringSize);
				}
			}
			
			return true;
		}

		public void InternalMarkFrame()
		{
			if (this._gfxEvent != null)
			{
				this._gfxEvent.Dispose();
				this._gfxEvent = null;
			}
			
			if (this._profiling)
			{
				this._frameStart = this._newFrameStart;
				this._frameEnd = Stopwatch.GetTimestamp();
			
				if (this._eventsToBeReported != null)
				{
					this._eventPool.PutList(this._eventsToBeReported);
				}
			
				this._eventsToBeReported = this._activeEvents.Clear();
			}
			
			if (this._wantProfiling != this._profiling)
			{
				this._profiling = this._wantProfiling;
			
				if (!this._profiling && this._eventsToBeReported != null)
				{
					this._eventPool.PutList(this._eventsToBeReported);
					this._eventsToBeReported = null;
					this._frameStart = -1L;
				}
			}
			
			if (this._profiling)
			{
				if (!this._activeEvents.Empty)
				{
					this._eventPool.PutList(this._activeEvents.Clear());
				}
			
				this._newFrameStart = Stopwatch.GetTimestamp();
			}
			
			this._frameMarked = true;
		}

		public override void Update(GameTime gameTime)
		{
			if (!this._frameMarked)
			{
				this.InternalMarkFrame();
			}
			
			this._frameMarked = false;
		}

		public override void Draw(GameTime gameTime)
		{
			if (this._profiling && this._eventsToBeReported != null)
			{
				ProfilerUtils._standard2DProjection = 
					Matrix.CreateOrthographicOffCenter(0f, 
						(float)base.Game.GraphicsDevice.Viewport.Width, 
						(float)base.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
				
				float num = (float)((double)(
					this._frameEnd - this._frameStart) * this._ticksToMilliseconds);
				
				this._sampleWaitTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				
				if (this._sampleWaitTime < 0f)
				{
					this._sampleWaitTime += 1f;
					this._memSamples.Add((float)(GC.GetTotalMemory(false) / 1024L));
				}
				
				Profiler.ProfileEvent profileEvent = this._eventsToBeReported;
				
				while (profileEvent != null)
				{
					Profiler.ProfileEvent profileEvent2 = profileEvent;
					profileEvent = (profileEvent2.NextNode as Profiler.ProfileEvent);
					this._eventLists[profileEvent2._threadIndex].Push(profileEvent2);
				}
				
				this._eventsToBeReported = null;
				
				float x = 200f + num * 30f;
				
				this._primitiveBatch.Begin(PrimitiveType.LineList);
				this._primitiveBatch.AddVertex(new Vector2(x, 180f), Color.White);
				this._primitiveBatch.AddVertex(new Vector2(x, 260f), Color.White);
				this._primitiveBatch.End();
				this._primitiveBatch.Begin(PrimitiveType.TriangleList);
				this._primitiveBatch.AddFilledBox(new Vector2(200f, 180f), 
												  new Vector2(990f, 5f), Color.Olive, false);
				this._primitiveBatch.AddFilledBox(new Vector2(200f, 180f), 
												  new Vector2(510f, 5f), Color.Yellow, false);
				this._primitiveBatch.AddFilledBox(new Vector2(200f, 185f), 
												  new Vector2(num * 30f, 5f), Color.Maroon, false);
				
				Vector2 vector = new Vector2(200f, 200f);
				
				foreach (ProfilerSimpleStack<Profiler.ProfileEvent> profilerSimpleStack 
					in this._eventLists)
				{
					if (!profilerSimpleStack.Empty)
					{
						Profiler.ProfileEvent profileEvent2 = profilerSimpleStack.Root;
						this._eventStack.Clear();
						float num2 = 100f;
						
						while (profileEvent2 != null)
						{
							while (this._eventStack.Count != 0 
								   && this._eventStack[this._eventStack.Count - 1]._endTime <= 
								   profileEvent2._startTime)
							{
								num2 = num2 * 3f / 2f;
								this._eventStack.RemoveAt(this._eventStack.Count - 1);
							}
							
							this._eventStack.Add(profileEvent2);
							num2 = num2 * 2f / 3f;
							float x2 = this.GetElapsedMillis(profileEvent2) * 30f;
							Color white;
							
							if (!this._colorDict.TryGetValue(profileEvent2._name, out white))
							{
								white = Color.White;
							}
						
							vector.X = 200f + this.GetStartMillis(profileEvent2) * 30f;
							
							this._primitiveBatch.AddFilledBox(
								vector, new Vector2(x2, num2), white, false);
							
							profileEvent2 = (profileEvent2.NextNode as Profiler.ProfileEvent);
						}
					
						this._eventPool.PutList(profilerSimpleStack.Root);
						profilerSimpleStack.Clear();
					}
				
					vector.Y += 20f;
				}
				
				this._primitiveBatch.End();
				
				if (this.GetSectionColors())
				{
					this._spriteBatch.Begin();
					this._primitiveBatch.Begin(PrimitiveType.TriangleList);
					vector.Y += 50f;
					vector.X = 200f;
					
					for (int j = 0; j < this._sectionNames.Length; j++)
					{
						if (vector.X + this._stringSize.X + this._stringSize.Y + 10f >= 
							(float)base.GraphicsDevice.Viewport.Width)
						{
							vector.X = 200f;
							vector.Y += this._stringSize.Y + 10f;
						}
						
						Vector2 vector2 = vector;
						vector2.X += this._stringSize.X - this._sectionSizes[j].X;
						
						this._spriteBatch.DrawString(ProfilerUtils.SystemFont, 
							this._sectionNames[j], vector2, Color.White);
						
						vector2.X = vector.X + this._stringSize.X + 10f;
						
						this._primitiveBatch.AddFilledBox(vector2, 
							new Vector2(this._stringSize.Y, this._stringSize.Y), 
							this._sectionColors[j], false);

						vector.X += this._stringSize.X + this._stringSize.Y + 20f;
					}
					
					this._primitiveBatch.End();
					this._spriteBatch.End();
				}
				
				vector.X = 320f;
				vector.Y += this._stringSize.Y + 40f;
				Vector2 size = new Vector2(620f, 300f);
				Vector2 scale = new Vector2(0f, 256000f);
				this._primitiveBatch.DrawGraphVerticalAxis(vector, size, Color.Red);
				this._primitiveBatch.DrawGraphBar(0f, scale, vector, size, Color.Red);
				this._primitiveBatch.DrawGraphBar(128000f, scale, vector, size, Color.Red);
				
				for (int k = 2; k < 25; k += 2)
				{
					this._primitiveBatch.DrawGraphBar((float)(k * 10000), 
						scale, vector, size, Color.Yellow);
				}
				
				this._primitiveBatch.DrawGraph(this._memSamples.Buffer, 
					this._memSamples.Head, scale, vector, size, Color.White);
				
				this._primitiveBatch.End();
			}

			this._gfxEvent = Profiler.TimeSection("XNA (Graphics)", ProfilerThreadEnum.MAIN);
		}
	}
}
