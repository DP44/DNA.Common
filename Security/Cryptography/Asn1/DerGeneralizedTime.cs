using System;
using System.Globalization;
using System.Text;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerGeneralizedTime : Asn1Object
	{
		private readonly string time;

		public static DerGeneralizedTime GetInstance(object obj)
		{
			if (obj == null || obj is DerGeneralizedTime)
			{
				return (DerGeneralizedTime)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerGeneralizedTime(((Asn1OctetString)obj).GetOctets());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerGeneralizedTime GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerGeneralizedTime.GetInstance(obj.GetObject());
		}

		public DerGeneralizedTime(string time)
		{
			this.time = time;
			try
			{
				this.ToDateTime();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("invalid date string: " + ex.Message);
			}
		}

		public DerGeneralizedTime(DateTime time)
		{
			this.time = time.ToString("yyyyMMddHHmmss\\Z");
		}

		internal DerGeneralizedTime(byte[] bytes)
		{
			this.time = ASCIIEncoder.GetString(bytes, 0, bytes.Length);
		}

		public string TimeString
		{
			get
			{
				return this.time;
			}
		}

		public string GetTime()
		{
			if (this.time[this.time.Length - 1] == 'Z')
			{
				return this.time.Substring(0, this.time.Length - 1) + "GMT+00:00";
			}
			int num = this.time.Length - 5;
			char c = this.time[num];
			if (c == '-' || c == '+')
			{
				return string.Concat(new string[]
				{
					this.time.Substring(0, num),
					"GMT",
					this.time.Substring(num, 3),
					":",
					this.time.Substring(num + 3)
				});
			}
			num = this.time.Length - 3;
			c = this.time[num];
			if (c == '-' || c == '+')
			{
				return this.time.Substring(0, num) + "GMT" + this.time.Substring(num) + ":00";
			}
			return this.time + this.CalculateGmtOffset();
		}

		private string CalculateGmtOffset()
		{
			char c = '+';
			int num = TimeZone.CurrentTimeZone.GetUtcOffset(this.ToDateTime()).Minutes;
			if (num < 0)
			{
				c = '-';
				num = -num;
			}
			int num2 = num / 60;
			num %= 60;
			return string.Concat(new object[]
			{
				"GMT",
				c,
				DerGeneralizedTime.Convert(num2),
				":",
				DerGeneralizedTime.Convert(num)
			});
		}

		private static string Convert(int time)
		{
			if (time < 10)
			{
				return "0" + time;
			}
			return time.ToString();
		}

		public DateTime ToDateTime()
		{
			string text = this.time;
			bool makeUniversal = false;
			string formatStr;
			if (text.EndsWith("Z"))
			{
				if (this.HasFractionalSeconds)
				{
					int count = text.Length - text.IndexOf('.') - 2;
					formatStr = "yyyyMMddHHmmss." + this.FString(count) + "\\Z";
				}
				else
				{
					formatStr = "yyyyMMddHHmmss\\Z";
				}
			}
			else if (this.time.IndexOf('-') > 0 || this.time.IndexOf('+') > 0)
			{
				text = this.GetTime();
				makeUniversal = true;
				if (this.HasFractionalSeconds)
				{
					int count2 = text.IndexOf("GMT") - 1 - text.IndexOf('.');
					formatStr = "yyyyMMddHHmmss." + this.FString(count2) + "'GMT'zzz";
				}
				else
				{
					formatStr = "yyyyMMddHHmmss'GMT'zzz";
				}
			}
			else if (this.HasFractionalSeconds)
			{
				int count3 = text.Length - 1 - text.IndexOf('.');
				formatStr = "yyyyMMddHHmmss." + this.FString(count3);
			}
			else
			{
				formatStr = "yyyyMMddHHmmss";
			}
			return this.ParseDateString(text, formatStr, makeUniversal);
		}

		private string FString(int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append('f');
			}
			return stringBuilder.ToString();
		}

		private DateTime ParseDateString(string dateStr, string formatStr, bool makeUniversal)
		{
			DateTime result = DateTime.ParseExact(dateStr, formatStr, DateTimeFormatInfo.InvariantInfo);
			if (!makeUniversal)
			{
				return result;
			}
			return result.ToUniversalTime();
		}

		private bool HasFractionalSeconds
		{
			get
			{
				return this.time.IndexOf('.') == 14;
			}
		}

		private byte[] GetOctets()
		{
			return ASCIIEncoder.GetBytes(this.time);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(24, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerGeneralizedTime derGeneralizedTime = asn1Object as DerGeneralizedTime;
			return derGeneralizedTime != null && this.time.Equals(derGeneralizedTime.time);
		}

		protected override int Asn1GetHashCode()
		{
			return this.time.GetHashCode();
		}
	}
}
