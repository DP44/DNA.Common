using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DNA
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	internal class CommonDialogResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		internal CommonDialogResources()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(CommonDialogResources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("DNA.CommonDialogResources", typeof(CommonDialogResources).Assembly);
					CommonDialogResources.resourceMan = resourceManager;
				}
				return CommonDialogResources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return CommonDialogResources.resourceCulture;
			}
			set
			{
				CommonDialogResources.resourceCulture = value;
			}
		}

		internal static Bitmap loginwithfacebook
		{
			get
			{
				object @object = CommonDialogResources.ResourceManager.GetObject("loginwithfacebook", CommonDialogResources.resourceCulture);
				return (Bitmap)@object;
			}
		}
	}
}
