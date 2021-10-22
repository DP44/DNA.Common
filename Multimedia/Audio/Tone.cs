using System;
using DNA.Data.Units;

namespace DNA.Multimedia.Audio
{
	public struct Tone
	{
		private const int NotesPerOctive = 12;

		private const float BaseTone = 440f;

		private float _value;

		public override string ToString()
		{
			return this.BaseNote.ToString() + this.Octive.ToString();
		}

		public int Octive
		{
			get
			{
				if (this._value < 0f)
				{
					int num = (int)Math.Ceiling((double)this._value);
					int num2 = num / 12;
					return num2 - 1;
				}
				int num3 = (int)Math.Floor((double)this._value);
				return num3 / 12;
			}
		}

		public string NoteName
		{
			get
			{
				switch (this.BaseNote)
				{
				case Notes.A:
					return "A";
				case Notes.Bb:
					return "B♭";
				case Notes.B:
					return "B";
				case Notes.C:
					return "C";
				case Notes.Db:
					return "C♯";
				case Notes.D:
					return "D";
				case Notes.Eb:
					return "E♭";
				case Notes.E:
					return "E";
				case Notes.F:
					return "F";
				case Notes.Gb:
					return "F♯";
				case Notes.G:
					return "G";
				case Notes.Ab:
					return "G♯";
				default:
					return "";
				}
			}
		}

		public Notes BaseNote
		{
			get
			{
				int num = (int)Math.Round((double)this._value);
				if (num < 0)
				{
					num = -num;
					num %= 12;
					num = 12 - num;
					return (Notes)(num % 12);
				}
				return (Notes)(num % 12);
			}
		}

		public float Detune
		{
			get
			{
				int num = (int)Math.Round((double)this._value);
				return this._value - (float)num;
			}
		}

		public float Value
		{
			get
			{
				return this._value;
			}
		}

		public int KeyValue
		{
			get
			{
				return (int)this._value;
			}
		}

		public Frequency Frequency
		{
			get
			{
				return Tone.GetNoteFrequency(this._value);
			}
		}

		public static Tone FromKeyIndex(int value)
		{
			return new Tone((float)value);
		}

		public static Tone FromNote(Notes note, int octive)
		{
			if (octive < 0)
			{
				octive++;
			}
			float noteNumber = (float)note + (float)(12 * octive);
			return new Tone(noteNumber);
		}

		private static float NoteFromFrequency(Frequency frequency)
		{
			return (float)(12.0 * Math.Log((double)(frequency.Hertz / 440f), 2.0));
		}

		public static Tone FromFrequency(Frequency frequency)
		{
			return new Tone(Tone.NoteFromFrequency(frequency));
		}

		public static Tone Parse(string value)
		{
			int num = value.IndexOfAny(new char[]
			{
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9'
			});
			int octive = 5;
			string text;
			if (num > 0)
			{
				string s = value.Substring(num);
				octive = int.Parse(s);
				text = value.Substring(0, num);
			}
			else
			{
				text = value;
			}
			text = text.Trim();
			Notes note;
			try
			{
				note = (Notes)Enum.Parse(typeof(Notes), text, true);
			}
			catch
			{
				string a;
				// TODO: Clean control flow.
				if ((a = text) != null)
				{
					if (!(a == "C#"))
					{
						if (!(a == "D#"))
						{
							if (!(a == "F#"))
							{
								if (!(a == "G#"))
								{
									if (!(a == "A#"))
									{
										goto IL_C6;
									}
									note = Notes.Bb;
								}
								else
								{
									note = Notes.Ab;
								}
							}
							else
							{
								note = Notes.Gb;
							}
						}
						else
						{
							note = Notes.Eb;
						}
					}
					else
					{
						note = Notes.Db;
					}
					goto IL_D3;
				}
				IL_C6:
				throw new FormatException("Invalid Note");
			}
			IL_D3:
			return Tone.FromNote(note, octive);
		}

		private Tone(float noteNumber)
		{
			if (float.IsNaN(noteNumber))
			{
				this._value = 0f;
			}
			this._value = noteNumber;
		}

		private static Frequency GetNoteFrequency(float note)
		{
			return Frequency.FromHertz((float)(440.0 * Math.Pow(2.0, (double)note / 12.0)));
		}

		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}

		public bool Equals(Tone other)
		{
			return this._value == other._value;
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(Tone) && this.Equals((Tone)obj);
		}

		public static bool operator ==(Tone a, Tone b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Tone a, Tone b)
		{
			return !a.Equals(b);
		}
	}
}
