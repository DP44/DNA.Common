using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DNA
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class CommonResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		internal CommonResources()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(CommonResources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("DNA.CommonResources", typeof(CommonResources).Assembly);
					CommonResources.resourceMan = resourceManager;
				}
				return CommonResources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return CommonResources.resourceCulture;
			}
			set
			{
				CommonResources.resourceCulture = value;
			}
		}

		internal static string Address_not_available
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Address_not_available", CommonResources.resourceCulture);
			}
		}

		internal static string Awards
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Awards", CommonResources.resourceCulture);
			}
		}

		internal static string Cancel
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Cancel", CommonResources.resourceCulture);
			}
		}

		internal static string Couldn_t_get_local_address
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Couldn_t_get_local_address", CommonResources.resourceCulture);
			}
		}

		internal static string Error
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Error", CommonResources.resourceCulture);
			}
		}

		internal static string Error_getting_address_dyndns_returned
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Error_getting_address_dyndns_returned", CommonResources.resourceCulture);
			}
		}

		internal static string Invalid_Login
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Invalid_Login", CommonResources.resourceCulture);
			}
		}

		internal static string Invalid_username_or_password_
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Invalid_username_or_password_", CommonResources.resourceCulture);
			}
		}

		internal static string LauncherControl_ValidateLicenseFacebook_There_was_a_problem_authenticating_your_facebook_account_
		{
			get
			{
				return CommonResources.ResourceManager.GetString("LauncherControl_ValidateLicenseFacebook_There_was_a_problem_authenticating_your_facebook_account_", CommonResources.resourceCulture);
			}
		}

		internal static string No_network_is_available
		{
			get
			{
				return CommonResources.ResourceManager.GetString("No_network_is_available", CommonResources.resourceCulture);
			}
		}

		internal static string Not_connected_to_internet
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Not_connected_to_internet", CommonResources.resourceCulture);
			}
		}

		internal static string Off
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Off", CommonResources.resourceCulture);
			}
		}

		internal static string OK
		{
			get
			{
				return CommonResources.ResourceManager.GetString("OK", CommonResources.resourceCulture);
			}
		}

		internal static string On
		{
			get
			{
				return CommonResources.ResourceManager.GetString("On", CommonResources.resourceCulture);
			}
		}

		internal static string There_was_an_error_
		{
			get
			{
				return CommonResources.ResourceManager.GetString("There_was_an_error_", CommonResources.resourceCulture);
			}
		}

		internal static string Unsupported_Hardware
		{
			get
			{
				return CommonResources.ResourceManager.GetString("Unsupported_Hardware", CommonResources.resourceCulture);
			}
		}

		internal static string We_re_sorry_but_your_video_hardware_is_not_currently_supported__We_are_currently_working_on_supporting_more_hardware__and_we_will_have_a_solution_for_you_soon_
		{
			get
			{
				return CommonResources.ResourceManager.GetString("We_re_sorry_but_your_video_hardware_is_not_currently_supported__We_are_currently_working_on_supporting_more_hardware__and_we_will_have_a_solution_for_you_soon_", CommonResources.resourceCulture);
			}
		}
	}
}
