using System;

namespace DNA.Multimedia.Audio
{
	public struct MusicalNote
	{
		private TimeSpan _duration;
		private Percentage _volume;
		private Tone _tone;

		public Tone Tone
		{
			get
			{
				return this._tone;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return this._duration;
			}
		}

		public Percentage Volume
		{
			get
			{
				return this._volume;
			}
		}

		public MusicalNote(Tone tone, TimeSpan duration)
		{
			this._tone = tone;
			this._duration = duration;
			this._volume = Percentage.OneHundred;
		}

		public MusicalNote(Tone tone, TimeSpan duration, Percentage volume)
		{
			this._tone = tone;
			this._duration = duration;
			this._volume = volume;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public bool Equals(MusicalNote other)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(MusicalNote) && this.Equals((MusicalNote)obj);
		}

		public static bool operator == (MusicalNote a, MusicalNote b)
		{
			return a.Equals(b);
		}

		public static bool operator != (MusicalNote a, MusicalNote b)
		{
			return !a.Equals(b);
		}
	}
}
