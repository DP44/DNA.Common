using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Particles
{
	public class ParticleEffect : ParticleBase<Texture2D>
	{
		[NonSerialized]
		private List<ParticleEmitter.ParticleEmitterCore> _particleCores = 
			new List<ParticleEmitter.ParticleEmitterCore>();

		/// <summary>
		/// 
		/// </summary>
		public void Flush()
		{
			foreach (ParticleEmitter.ParticleEmitterCore particleCore in this._particleCores)
			{
				particleCore.ParticleEmitter.ReleaseCore();
			}

			this._particleCores.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		internal ParticleEmitter.ParticleEmitterCore CreateParticleCore(ParticleEmitter emitter)
		{
			foreach (ParticleEmitter.ParticleEmitterCore particleCore in this._particleCores)
			{
				if (!particleCore.HasActiveParticles || 
					particleCore.ParticleEmitter.Scene == null)
				{
					particleCore.ParticleEmitter.ReleaseCore();
					emitter.SetCore(particleCore);
					return particleCore;
				}
			}

			ParticleEmitter.ParticleEmitterCore emitterCore = 
				new ParticleEmitter.ParticleEmitterCore(emitter);
			
			this._particleCores.Add(emitterCore);
			
			return emitterCore;
		}

		/// <summary>
		/// Creates a particle emitter.
		/// </summary>
		/// <param name="game">The game object.</param>
		public ParticleEmitter CreateEmitter(DNAGame game) =>
			ParticleEmitter.Create(game, this, null);

		/// <summary>
		/// Creates a particle emitter.
		/// </summary>
		/// <param name="game">The game object.</param>
		/// <param name="reflectionMap">The reflection map for the effect to use.</param>
		public ParticleEmitter CreateEmitter(DNAGame game, Texture2D reflectionMap) =>
			ParticleEmitter.Create(game, this, reflectionMap);
	}
}
