using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DNA.IO.Storage
{
	public static class StorageSettings
	{
		private static readonly Dictionary<string, Language> languageMap = new Dictionary<string, Language>
		{
			{
				"de",
				Language.German
			},
			{
				"es",
				Language.Spanish
			},
			{
				"fr",
				Language.French
			},
			{
				"it",
				Language.Italian
			},
			{
				"ja",
				Language.Japanese
			},
			{
				"en",
				Language.English
			}
		};

		private static readonly Dictionary<Language, string> cultureMap = new Dictionary<Language, string>
		{
			{
				Language.German,
				"de-DE"
			},
			{
				Language.Spanish,
				"es-ES"
			},
			{
				Language.French,
				"fr-FR"
			},
			{
				Language.Italian,
				"it-IT"
			},
			{
				Language.Japanese,
				"ja-JP"
			},
			{
				Language.English,
				"en-US"
			}
		};

		public static void SetSupportedLanguages(params Language[] supportedLanguages)
		{
			if (supportedLanguages == null)
			{
				throw new ArgumentNullException("supportedLanguages");
			}
			if (supportedLanguages.Length == 0)
			{
				throw new ArgumentException("supportedLanguages");
			}
			foreach (Language language in supportedLanguages)
			{
				if (language < Language.German || language > Language.English)
				{
					throw new ArgumentException("supportedLanguages");
				}
			}
			Language value;
			if (!StorageSettings.languageMap.TryGetValue(CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower(), out value) || !supportedLanguages.Contains(value))
			{
				Strings.Culture = new CultureInfo(StorageSettings.cultureMap[supportedLanguages[0]]);
				StorageSettings.ResetSaveDeviceStrings();
			}
		}

		public static void ResetSaveDeviceStrings()
		{
			MUSaveDevice.OkOption = Strings.Ok;
			MUSaveDevice.YesOption = Strings.Yes_Select_new_device;
			MUSaveDevice.NoOption = Strings.No_Continue_without_device;
			MUSaveDevice.DeviceOptionalTitle = Strings.Reselect_Storage_Device;
			MUSaveDevice.DeviceRequiredTitle = Strings.Storage_Device_Required;
			MUSaveDevice.ForceDisconnectedReselectionMessage = Strings.forceDisconnectedReselectionMessage;
			MUSaveDevice.PromptForDisconnectedMessage = Strings.promptForDisconnectedMessage;
			MUSaveDevice.ForceCancelledReselectionMessage = Strings.forceCanceledReselectionMessage;
			MUSaveDevice.PromptForCancelledMessage = Strings.promptForCancelledMessage;
		}
	}
}
