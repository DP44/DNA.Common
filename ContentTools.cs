using System;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace DNA {
	public static class ContentTools {
		/// <summary>
		/// Gets a localized version of an asset's name.
		/// </summary>
		/// <param name="rootDirectory">The root directory to scan.</param>
		/// <param name="assetName">The name of the asset to fetch.</param>
		public static string GetLocalizedAssetName(string rootDirectory, string assetName) 
		{
			string[] assetValues = new string[]
			{
				CultureInfo.CurrentUICulture.Name,
				CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
			};

			foreach (string assetValue in assetValues) 
			{
				string path = assetName + '.' + assetValue;

				if (File.Exists(Path.Combine(rootDirectory, path + ".xnb")))
				{
					return path;
				}
			}

			return assetName;
		}

		/// <summary>
		/// Loads an asset to the ContentManager instance.
		/// </summary>
		/// <param name="name">The name of the asset to load.</param>
		public static T LoadLocalized<T>(this ContentManager content, string name) 
		{
			string localizedAssetName = 
				ContentTools.GetLocalizedAssetName(content.RootDirectory, name);
			
			return content.Load<T>(localizedAssetName);
		}
	}
}
