using System;

namespace DNA.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DisplayNameAttribute : Attribute
	{
		private string _displayName;

		/// <summary>
		/// 
		/// </summary>
		public string DisplayName => 
			this._displayName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static string GetDisplayName(Type t)
		{
			object[] customAttributes = t.GetCustomAttributes(
				typeof(DisplayNameAttribute), false);
			
			#if DECOMPILED
				if (customAttributes.Length == 0)
				{
					throw new ArgumentException(
						"Class " + t.Name + " Does not have a Display Name");
				}
				
				DisplayNameAttribute displayNameAttribute = 
					(DisplayNameAttribute)customAttributes[0];
				
				return displayNameAttribute.DisplayName;
			#else
				return customAttributes.Length != 0
					? ((DisplayNameAttribute) customAttributes[0]).DisplayName
					: throw new ArgumentException(
						"Class " + t.Name + " Does not have a Display Name");
			#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public DisplayNameAttribute(string displayName) => 
			this._displayName = displayName;
	}
}
