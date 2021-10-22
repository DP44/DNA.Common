using System;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography.Crypto
{
	public class KeyGenerationParameters
	{
		private SecureRandom random;

		private int strength;

		public KeyGenerationParameters(SecureRandom random, int strength)
		{
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}
			if (strength < 1)
			{
				throw new ArgumentException("strength must be a positive value", "strength");
			}
			this.random = random;
			this.strength = strength;
		}

		public SecureRandom Random
		{
			get
			{
				return this.random;
			}
		}

		public int Strength
		{
			get
			{
				return this.strength;
			}
		}
	}
}
