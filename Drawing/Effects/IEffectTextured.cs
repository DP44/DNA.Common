using System;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Effects
{
	public interface IEffectTextured
	{
		Texture DiffuseMap { get; set; }
		Texture OpacityMap { get; set; }
		Texture SpecularMap { get; set; }
		Texture NormalMap { get; set; }
		Texture LightMap { get; set; }
		Texture DisplacementMap { get; set; }
		Texture ReflectionMap { get; set; }
	}
}
