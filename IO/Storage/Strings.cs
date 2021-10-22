using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DNA.IO.Storage
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class Strings
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		internal Strings()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Strings.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("DNA.IO.Storage.Strings", typeof(Strings).Assembly);
					Strings.resourceMan = resourceManager;
				}
				return Strings.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Strings.resourceCulture;
			}
			set
			{
				Strings.resourceCulture = value;
			}
		}

		internal static string forceCanceledReselectionMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("forceCanceledReselectionMessage", Strings.resourceCulture);
			}
		}

		internal static string forceDisconnectedReselectionMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("forceDisconnectedReselectionMessage", Strings.resourceCulture);
			}
		}

		internal static string NeedGamerService
		{
			get
			{
				return Strings.ResourceManager.GetString("NeedGamerService", Strings.resourceCulture);
			}
		}

		internal static string No_Continue_without_device
		{
			get
			{
				return Strings.ResourceManager.GetString("No_Continue_without_device", Strings.resourceCulture);
			}
		}

		internal static string Ok
		{
			get
			{
				return Strings.ResourceManager.GetString("Ok", Strings.resourceCulture);
			}
		}

		internal static string promptForCancelledMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("promptForCancelledMessage", Strings.resourceCulture);
			}
		}

		internal static string promptForDisconnectedMessage
		{
			get
			{
				return Strings.ResourceManager.GetString("promptForDisconnectedMessage", Strings.resourceCulture);
			}
		}

		internal static string Reselect_Storage_Device
		{
			get
			{
				return Strings.ResourceManager.GetString("Reselect_Storage_Device", Strings.resourceCulture);
			}
		}

		internal static string Storage_Device_Required
		{
			get
			{
				return Strings.ResourceManager.GetString("Storage_Device_Required", Strings.resourceCulture);
			}
		}

		internal static string StorageDevice_is_not_valid
		{
			get
			{
				return Strings.ResourceManager.GetString("StorageDevice_is_not_valid", Strings.resourceCulture);
			}
		}

		internal static string Yes_Select_new_device
		{
			get
			{
				return Strings.ResourceManager.GetString("Yes_Select_new_device", Strings.resourceCulture);
			}
		}
	}
}
