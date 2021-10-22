using System;

namespace DNA.Drawing.Effects
{
	public interface IEffectColor
	{
		ColorF DiffuseColor { get; set; }
		ColorF AmbientColor { get; set; }
		ColorF SpecularColor { get; set; }
		ColorF EmissiveColor { get; set; }
	}
}
