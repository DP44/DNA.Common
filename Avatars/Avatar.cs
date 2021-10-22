using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Drawing;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Avatars
{
	public class Avatar : Entity
	{
		public class AvatarVerifyView : View
		{
			private List<Avatar> _toVerify = new List<Avatar>();

			private List<Avatar> toRemove = new List<Avatar>();

			public void VerifyAvatar(Avatar avatar)
			{
				this._toVerify.Add(avatar);
			}

			public AvatarVerifyView(GraphicsDevice device)
			{
				int num = 32;
				this.SetDestinationTarget(new RenderTarget2D(device, num, num, false, SurfaceFormat.Bgra4444, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents));
			}

			protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
			{
				base.OnDraw(device, spriteBatch, gameTime);
				int width = base.Target.Width;
				this.toRemove.Clear();
				for (int i = 0; i < this._toVerify.Count; i++)
				{
					if (this._toVerify[i].AvatarState == AvatarRendererState.Ready)
					{
						Avatar avatar = this._toVerify[i];
						this.toRemove.Add(avatar);
						device.SetRenderTarget(base.Target);
						device.Clear(new Color(0, 0, 0, 0));
						avatar._avatarRenderer.World = Matrix.Identity;
						avatar._avatarRenderer.View = Matrix.CreateLookAt(new Vector3(0f, avatar.AvatarHeight / 2f, 5.5f), new Vector3(0f, avatar.AvatarHeight / 2f, 0f), Vector3.Up);
						avatar._avatarRenderer.Projection = Matrix.CreatePerspectiveFieldOfView(0.7853982f, 1f, 0.1f, 1000f);
						avatar._avatarRenderer.Draw(avatar.BindPose, avatar.Expression);
						avatar._avatarRenderer.Draw(avatar.BindPose, avatar.Expression);
						device.SetRenderTarget(null);
						byte[] array = new byte[width * width * 4];
						base.Target.GetData<byte>(array);
						int num = 0;
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j] != 0)
							{
								num++;
							}
						}
						float num2 = (float)num / (float)array.Length;
						avatar._invisible = (num2 < 0.01f);
					}
				}
				foreach (Avatar item in this.toRemove)
				{
					this._toVerify.Remove(item);
				}
			}
		}

		public const float WalkSpeed = 0.947f;

		public const float RunSpeed = 4f;

		private const float MAX_HEIGHT = 1.66651094f;

		private const float MIN_HEIGHT = 1.41208816f;

		public AvatarLightingManager LightingManager;

		private static AvatarDescription _defaultDescription;

		public static readonly byte[] DefaultDescriptionData = new byte[]
		{
			1,
			0,
			0,
			0,
			0,
			191,
			0,
			0,
			0,
			191,
			0,
			0,
			0,
			0,
			16,
			0,
			0,
			3,
			31,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			8,
			0,
			0,
			3,
			43,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			32,
			0,
			0,
			3,
			59,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			0,
			128,
			0,
			2,
			234,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32,
			0,
			2,
			158,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			64,
			0,
			2,
			100,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			63,
			128,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			byte.MaxValue,
			215,
			170,
			113,
			byte.MaxValue,
			110,
			83,
			38,
			byte.MaxValue,
			181,
			97,
			87,
			byte.MaxValue,
			99,
			129,
			167,
			byte.MaxValue,
			73,
			52,
			33,
			byte.MaxValue,
			83,
			149,
			202,
			byte.MaxValue,
			73,
			52,
			33,
			byte.MaxValue,
			207,
			89,
			105,
			byte.MaxValue,
			207,
			89,
			105,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			2,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			4,
			1,
			178,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			4,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			0,
			88,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			8,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			16,
			0,
			144,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32,
			0,
			49,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			32,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			32,
			0,
			49,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			32,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			16,
			0,
			144,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			16,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			8,
			0,
			88,
			0,
			1,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			8,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			4,
			1,
			178,
			0,
			3,
			193,
			200,
			241,
			9,
			161,
			156,
			178,
			224,
			0,
			4,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			224,
			0,
			2,
			77,
			216,
			48,
			81,
			160,
			3,
			51,
			5,
			26,
			3,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			19,
			202,
			106,
			209,
			13,
			230,
			203,
			203,
			185,
			0,
			179,
			142,
			247,
			181,
			126,
			186,
			157,
			86,
			192,
			29
		};

		public static readonly ReadOnlyCollection<int> DefaultParentBones = new ReadOnlyCollection<int>(new int[]
		{
			-1,
			0,
			0,
			0,
			0,
			1,
			2,
			2,
			3,
			3,
			1,
			6,
			5,
			6,
			5,
			8,
			5,
			8,
			5,
			14,
			12,
			11,
			16,
			15,
			14,
			20,
			20,
			20,
			22,
			22,
			22,
			25,
			25,
			25,
			28,
			28,
			28,
			33,
			33,
			33,
			33,
			33,
			33,
			33,
			36,
			36,
			36,
			36,
			36,
			36,
			36,
			37,
			38,
			39,
			40,
			43,
			44,
			45,
			46,
			47,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60
		});

		public static readonly ReadOnlyCollection<string> BoneNames;

		public static readonly ReadOnlyCollection<Matrix> DefaultBindPose = new ReadOnlyCollection<Matrix>(new Matrix[]
		{
			new Matrix(-0.9999998f, 3.191891E-16f, 1.192093E-07f, 0f, 3.191891E-16f, 1f, -1.665335E-16f, 0f, -1.192093E-07f, -1.665334E-16f, -0.9999998f, 0f, 6.185371E-06f, 0.7769224f, -0.008659153f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -7.071068E-06f, 0.02402931f, -4.082918E-06f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.09005712f, -0.1072602f, 0.008654864f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.09005711f, -0.1072602f, 0.008654864f, 1f),
			new Matrix(0.92f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.96f, 0f, 0f, 0.03059953f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0.1158395f, -0.007715893f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, -0.2889061f, -0.01396209f, 1f),
			new Matrix(0.88f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, -0.144453f, -0.006491148f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, -0.2889061f, -0.01396209f, 1f),
			new Matrix(0.88f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, -0.144453f, -0.006491148f, 1f),
			new Matrix(0.84f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.84f, 0f, 7.071068E-06f, 0.06121063f, -0.005139845f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.006229609f, -0.279172f, -0.02728323f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.007396337f, 0.1224098f, 0.01427236f, 1f),
			new Matrix(0.88f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0.002743572f, -0.1371553f, -0.01205149f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0.1737708f, -0.01882024f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.006243758f, -0.279172f, -0.02728323f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.007396337f, 0.1224098f, 0.01427236f, 1f),
			new Matrix(0.88f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.88f, 0f, -0.002757721f, -0.1371553f, -0.01205149f, 1f),
			new Matrix(0.96f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.92f, 0f, 0f, 0.04343981f, -0.004703019f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0.1005861f, 0.01940812f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1176201f, 6.937981E-05f, -0.02792418f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.006682158f, -0.08587508f, 0.1141176f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.1176201f, 6.937981E-05f, -0.02792418f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.006682158f, -0.08587508f, 0.1141176f, 1f),
			new Matrix(0.88f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0.0335325f, 0.006466653f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.2201223f, -0.005196214f, 0.0007593427f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0.1100541f, -0.001905322f, 0.0002776086f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0.004398212f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.2201223f, -0.005196214f, 0.0007593427f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, -0.1100541f, -0.001905322f, 0.0002776086f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, -0.004398197f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1130734f, -0.002655864f, 0.00172689f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0.08480331f, -0.001720548f, 0.001122681f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.1696137f, -0.003995299f, 0.002584212f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.1130735f, -0.002655864f, 0.00172689f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 0.88f, 0f, 0f, 0f, 0f, 0.88f, 0f, -0.08480334f, -0.001720548f, 0.001122681f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.1696137f, -0.003995299f, 0.002584212f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.0921219f, -0.02434099f, 0.03605649f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.09366339f, -0.02339423f, 0.009479525f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.08793581f, -0.02604997f, -0.01582371f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.07674938f, -0.02977967f, -0.03964907f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.06290424f, -0.1572471f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.09434927f, -0.09435052f, -4.082918E-06f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -7.092953E-06f, 0f, 0.008413997f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.0921219f, -0.02434099f, 0.03605649f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.09366339f, -0.02339423f, 0.009479525f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.08793581f, -0.02604997f, -0.01582371f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.07673526f, -0.02977967f, -0.03964907f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.06290424f, -0.1572471f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.09434932f, -0.09435052f, -4.082918E-06f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 7.033348E-06f, 0f, 0.008413997f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.03987372f, 0f, 0.000330681f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.04226375f, 0f, -1.224689E-05f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.0405314f, 0f, -0.0002939366f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.03423101f, 0f, -0.0003061891f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.05561399f, -0.03163874f, 0.04206999f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.03987378f, 0f, 0.000330681f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.04226381f, 0f, -1.224689E-05f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.04051721f, 0f, -0.0002939366f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.03424519f, 0f, -0.0003061891f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.05561393f, -0.03163874f, 0.04206999f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.02873683f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.0298894f, 0f, 1.224689E-05f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.02934492f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.02515888f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.02728015f, -0.01478016f, 0.01658305f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.02873683f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.02987522f, 0f, 1.224689E-05f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.0293591f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.02515888f, 0f, 0f, 1f),
			new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -0.02728015f, -0.01478016f, 0.01658305f, 1f)
		});

		private static readonly string[] nativeBoneNames = new string[]
		{
			"BASE__Skeleton",
			"BACKA__Skeleton",
			"LF_H__Skeleton",
			"RT_H__Skeleton",
			"SC_BASE__Skeleton",
			"BACKB__Skeleton",
			"LF_K__Skeleton",
			"LF_SC_H__Skeleton",
			"RT_K__Skeleton",
			"RT_SC_H__Skeleton",
			"SC_BACKA__Skeleton",
			"LF_A__Skeleton",
			"LF_C__Skeleton",
			"LF_SC_K__Skeleton",
			"NECK__Skeleton",
			"RT_A__Skeleton",
			"RT_C__Skeleton",
			"RT_SC_K__Skeleton",
			"SC_BACKB__Skeleton",
			"HEAD__Skeleton",
			"LF_S__Skeleton",
			"LF_T__Skeleton",
			"RT_S__Skeleton",
			"RT_T__Skeleton",
			"SC_NECK__Skeleton",
			"LF_E__Skeleton",
			"LF_SC_S__Skeleton",
			"LF_SC_TWIST_S__Skeleton",
			"RT_E__Skeleton",
			"RT_SC_S__Skeleton",
			"RT_SC_TWIST_S__Skeleton",
			"LF_E_TWIST__Skeleton",
			"LF_SC_E__Skeleton",
			"LF_W__Skeleton",
			"RT_E_TWIST__Skeleton",
			"RT_SC_E__Skeleton",
			"RT_W__Skeleton",
			"LF_FINGA__Skeleton",
			"LF_FINGB__Skeleton",
			"LF_FINGC__Skeleton",
			"LF_FINGD__Skeleton",
			"LF_PROP__Skeleton",
			"LF_SPECIAL__Skeleton",
			"LF_THUMB__Skeleton",
			"RT_FINGA__Skeleton",
			"RT_FINGB__Skeleton",
			"RT_FINGC__Skeleton",
			"RT_FINGD__Skeleton",
			"RT_PROP__Skeleton",
			"RT_SPECIAL__Skeleton",
			"RT_THUMB__Skeleton",
			"LF_FINGA1__Skeleton",
			"LF_FINGB1__Skeleton",
			"LF_FINGC1__Skeleton",
			"LF_FINGD1__Skeleton",
			"LF_THUMB1__Skeleton",
			"RT_FINGA1__Skeleton",
			"RT_FINGB1__Skeleton",
			"RT_FINGC1__Skeleton",
			"RT_FINGD1__Skeleton",
			"RT_THUMB1__Skeleton",
			"LF_FINGA2__Skeleton",
			"LF_FINGB2__Skeleton",
			"LF_FINGC2__Skeleton",
			"LF_FINGD2__Skeleton",
			"LF_THUMB2__Skeleton",
			"RT_FINGA2__Skeleton",
			"RT_FINGB2__Skeleton",
			"RT_FINGC2__Skeleton",
			"RT_FINGD2__Skeleton",
			"RT_THUMB2__Skeleton"
		};

		private AvatarRenderer _avatarRenderer;

		private AvatarAnimationCollection _animations;

		private AvatarExpression _expression = default(AvatarExpression);

		private AvatarDescription _avatarDescription;

		private Matrix[] _bonesToAvatar = new Matrix[71];

		private Matrix[] _boneTransformBuffer = new Matrix[71];

		private Matrix[] _wireFrameWorldTransforms;

		private Entity[] _partMap = new Entity[71];

		private VertexPositionColor[] _wireFrameVerts;

		public PerspectiveCamera EyePointCamera = new PerspectiveCamera();

		public bool HideHead;

		public object Tag;

		private bool _invisible;

		private bool _skeletonBuilt;

		private Skeleton _skeleton = Bone.BuildSkeleton(Avatar.DefaultBindPose, Avatar.DefaultParentBones, Avatar.BoneNames);

		private SkinnedModelEntity _proxyModel;

		public static Dictionary<string, int> boneNameLookup = new Dictionary<string, int>();

		private BasicEffect _wireFrameEffect;

		public Gamer _gamer;

		private bool[] _partMask = new bool[71];

		private Matrix eyeToHead = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 3.14159274f) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 3.14159274f)) * Matrix.CreateTranslation(new Vector3(0f, 0.05f, 0f));

		private Matrix[] _proxyBoneBuffer = new Matrix[71];

		public static AvatarDescription DefaultDescription
		{
			get
			{
				if (Avatar._defaultDescription == null)
				{
					Avatar._defaultDescription = AvatarDescription.CreateRandom();
				}
				return Avatar._defaultDescription;
			}
		}

		public ReadOnlyCollection<int> ParentBones
		{
			get
			{
				if (this._avatarRenderer != null && this._avatarRenderer.State == AvatarRendererState.Ready)
				{
					return this._avatarRenderer.ParentBones;
				}
				return Avatar.DefaultParentBones;
			}
		}

		public ReadOnlyCollection<Matrix> BindPose
		{
			get
			{
				if (this._avatarRenderer != null && this._avatarRenderer.State == AvatarRendererState.Ready)
				{
					return this._avatarRenderer.BindPose;
				}
				return Avatar.DefaultBindPose;
			}
		}

		public bool IsInvisible
		{
			get
			{
				return this._invisible;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				return this._skeleton;
			}
		}

		public SkinnedModelEntity ProxyModelEntity
		{
			get
			{
				return this._proxyModel;
			}
			set
			{
				if (this._proxyModel != null)
				{
					this._proxyModel.RemoveFromParent();
					this._proxyModel = null;
					this.SetEyePoint(this.AvatarHeight);
				}
				if (value != null)
				{
					if (!this._skeletonBuilt && this._avatarRenderer.State == AvatarRendererState.Ready)
					{
						this.BuildSkeleton();
					}
					this._proxyModel = value;
					base.Children.Add(this._proxyModel);
					this.SetEyePoint(1.54f);
				}
				this._proxyModel = value;
			}
		}

		public AvatarDescription Description
		{
			get
			{
				return this._avatarDescription;
			}
		}

		public AvatarRendererState AvatarState
		{
			get
			{
				return this._avatarRenderer.State;
			}
		}

		public AvatarRenderer AvatarRenderer
		{
			get
			{
				return this._avatarRenderer;
			}
		}

		public float AvatarHeight
		{
			get
			{
				return 1.6f;
			}
		}

		static Avatar()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 71; i++)
			{
				AvatarBone avatarBone = (AvatarBone)i;
				list.Add(avatarBone.ToString());
			}
			Avatar.BoneNames = new ReadOnlyCollection<string>(list);
			for (int j = 0; j < Avatar.nativeBoneNames.Length; j++)
			{
				Avatar.boneNameLookup[Avatar.nativeBoneNames[j]] = j;
			}
		}

		public AvatarExpression Expression
		{
			get
			{
				return this._expression;
			}
			set
			{
				this._expression = value;
			}
		}

		public AvatarAnimationCollection Animations
		{
			get
			{
				return this._animations;
			}
		}

		public Gamer Gamer
		{
			get
			{
				return this._gamer;
			}
		}

		public Entity GetAvatarPart(AvatarBone bone)
		{
			if (this._partMap[(int)bone] != null)
			{
				return this._partMap[(int)bone];
			}
			Entity entity = new Entity();
			this._partMap[(int)bone] = entity;
			base.Children.Add(entity);
			int num = (int)bone;
			do
			{
				this._partMask[num] = true;
				num = Avatar.DefaultParentBones[num];
			}
			while (num >= 0 && !this._partMask[num]);
			this.Skeleton.CopyTransformsTo(this._bonesToAvatar);
			this.UpdateParts(this._bonesToAvatar);
			entity.LocalToParent = this._bonesToAvatar[(int)bone];
			return entity;
		}

		public bool IsMale
		{
			get
			{
				return this._avatarDescription.BodyType == AvatarBodyType.Male;
			}
		}

		private void InitalizeParts()
		{
			for (int i = 0; i < 71; i++)
			{
				this._bonesToAvatar[i] = Matrix.Identity;
				this._partMask[i] = false;
			}
			this.GetAvatarPart(AvatarBone.Head);
			base.Children.Add(this.EyePointCamera);
		}

		public void SetAsPlayerAvatar(PlayerIndex index)
		{
			this.SetAsPlayerAvatar(Gamer.SignedInGamers[index]);
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer(this._gamer, delegate(IAsyncResult resulta)
			{
			}, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			this._avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription, false);
			this._skeletonBuilt = false;
		}

		public void SetAsPlayerAvatar(Gamer gamer)
		{
			this._gamer = gamer;
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer(this._gamer, delegate(IAsyncResult resulta)
			{
			}, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			this._avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription, false);
			this._skeletonBuilt = false;
		}

		public Avatar(Gamer gamer)
		{
			this._animations = new AvatarAnimationCollection(this);
			this._expression.Mouth = AvatarMouth.Neutral;
			this._gamer = gamer;
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer(this._gamer, delegate(IAsyncResult resulta)
			{
			}, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			this._avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription, false);
			if (!this._avatarDescription.IsValid)
			{
				this._avatarDescription = AvatarDescription.CreateRandom();
			}
			this._expression = default(AvatarExpression);
			this.InitalizeParts();
		}

		public Avatar(PlayerIndex index)
		{
			this._animations = new AvatarAnimationCollection(this);
			this._expression.Mouth = AvatarMouth.Neutral;
			this._gamer = Gamer.SignedInGamers[index];
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer(this._gamer, delegate(IAsyncResult resulta)
			{
			}, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			this._avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription, false);
			if (!this._avatarDescription.IsValid)
			{
				this._avatarDescription = AvatarDescription.CreateRandom();
			}
			this._expression = default(AvatarExpression);
			this.InitalizeParts();
		}

		public Avatar(AvatarDescription description)
		{
			this._animations = new AvatarAnimationCollection(this);
			this._expression.Mouth = AvatarMouth.Neutral;
			this._avatarDescription = description;
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription, false);
			this._expression = default(AvatarExpression);
			this.InitalizeParts();
		}

		public Avatar(bool useRandom)
		{
			this._animations = new AvatarAnimationCollection(this);
			this._expression.Mouth = AvatarMouth.Neutral;
			if (useRandom)
			{
				this.MakeRandom();
			}
			else
			{
				this.MakeDefault();
			}
			this.InitalizeParts();
		}

		public Matrix GetBoneToAvatar(AvatarBone bone)
		{
			return this._bonesToAvatar[(int)bone];
		}

		public void UpdateParts(Matrix[] skeltonBones)
		{
			ReadOnlyCollection<Matrix> bindPose = this.BindPose;
			this._bonesToAvatar[0] = skeltonBones[0] * bindPose[0];
			for (int i = 1; i < 71; i++)
			{
				if (this._partMask[i])
				{
					this._bonesToAvatar[i] = skeltonBones[i];
					this._bonesToAvatar[i].Translation = bindPose[i].Translation;
					Matrix.Multiply(ref this._bonesToAvatar[i], ref this._bonesToAvatar[Avatar.DefaultParentBones[i]], out this._bonesToAvatar[i]);
				}
			}
			for (int j = 0; j < 71; j++)
			{
				if (this._partMap[j] != null)
				{
					this._partMap[j].LocalToParent = this._bonesToAvatar[j];
				}
			}
		}

		public static List<int> FindInfluencedBones(AvatarBone avatarBone)
		{
			List<int> list = new List<int>();
			list.Add((int)avatarBone);
			for (int i = list[0] + 1; i < Avatar.DefaultParentBones.Count; i++)
			{
				if (list.Contains(Avatar.DefaultParentBones[i]))
				{
					list.Add(i);
				}
			}
			return list;
		}

		public static bool[] GetInfluncedBoneList(AvatarBone bone)
		{
			return Avatar.GetInfluncedBoneList(new AvatarBone[]
			{
				bone
			});
		}

		public static bool[] GetInfluncedBoneList(IList<AvatarBone> bones)
		{
			new Dictionary<int, int>();
			bool[] array = new bool[71];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = false;
			}
			foreach (AvatarBone avatarBone in bones)
			{
				List<int> list = Avatar.FindInfluencedBones(avatarBone);
				foreach (int num in list)
				{
					array[num] = true;
				}
			}
			return array;
		}

		public static bool[] GetInfluncedBoneList(IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			new Dictionary<int, int>();
			bool[] array;
			if (bones != null)
			{
				array = Avatar.GetInfluncedBoneList(bones);
			}
			else
			{
				array = new bool[71];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = true;
				}
			}
			bool[] influncedBoneList = Avatar.GetInfluncedBoneList(maskedBones);
			for (int j = 0; j < influncedBoneList.Length; j++)
			{
				if (influncedBoneList[j])
				{
					array[j] = false;
				}
			}
			return array;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (!this._skeletonBuilt && this._avatarRenderer.State == AvatarRendererState.Ready)
			{
				this.BuildSkeleton();
			}
			this._animations.Update(gameTime.ElapsedGameTime, this.Skeleton);
			Matrix matrix = this._bonesToAvatar[19];
			Vector3 translation = matrix.Translation;
			Vector3 forward = matrix.Forward;
			Vector3 up = matrix.Up;
			forward.Normalize();
			up.Normalize();
			matrix = Matrix.CreateWorld(translation, forward, up);
			Matrix localToParent = this.eyeToHead * matrix;
			this.EyePointCamera.LocalToParent = localToParent;
			if (this.HideHead)
			{
				Bone bone = this.Skeleton[19];
				bone.Scale = new Vector3(0.001f, 0.001f, 0.001f);
			}
			this.Skeleton.CopyTransformsTo(this._boneTransformBuffer);
			if (this.ProxyModelEntity != null)
			{
				Matrix matrix2 = Matrix.CreateRotationY(3.14159274f);
				matrix2.Translation = new Vector3(0f, 0.7769f, -0.008664f);
				this.ProxyModelEntity.DefaultPose[0] = this._boneTransformBuffer[0] * matrix2;
				for (int i = 1; i < this.ProxyModelEntity.DefaultPose.Length; i++)
				{
					string name = this.ProxyModelEntity.Skeleton[i].Name;
					int num = Avatar.boneNameLookup[name];
					Matrix matrix3 = this._boneTransformBuffer[num];
					matrix3.Translation = this.ProxyModelEntity.BindPose[i].Translation;
					this.ProxyModelEntity.DefaultPose[i] = matrix3;
					this.ProxyModelEntity.Skeleton[i].SetTransform(matrix3);
				}
			}
			this.UpdateParts(this._boneTransformBuffer);
			base.OnUpdate(gameTime);
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (this.ProxyModelEntity != null)
			{
				return;
			}
			if (this.LightingManager != null)
			{
				this.LightingManager.SetAvatarLighting(this);
			}
			this._avatarRenderer.World = base.LocalToWorld;
			this._avatarRenderer.View = view;
			this._avatarRenderer.Projection = projection;
			this.DrawWireframeBones(device, view, projection);
			base.Draw(device, gameTime, view, projection);
		}

		public void MakeDefault()
		{
			this._avatarDescription = Avatar.DefaultDescription;
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription);
			this._expression = default(AvatarExpression);
		}

		private void DefaultSkeleton()
		{
			this._skeletonBuilt = false;
			this._skeleton = Bone.BuildSkeleton(Avatar.DefaultBindPose, Avatar.DefaultParentBones, Avatar.BoneNames);
		}

		protected void SetEyePoint(float playerHeight)
		{
			float num = 0.045f;
			num += (playerHeight - 1.41208816f) / 0.254422784f * 0.01f;
			this.eyeToHead = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 3.14159274f) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 3.14159274f)) * Matrix.CreateTranslation(new Vector3(0f, num, 0f));
		}

		private void BuildSkeleton()
		{
			try
			{
				this.SetEyePoint(this.AvatarHeight);
				this._skeleton = Bone.BuildSkeleton(this._avatarRenderer);
				this._skeletonBuilt = true;
			}
			catch (InvalidOperationException)
			{
			}
		}

		public void MakeRandom()
		{
			this._avatarDescription = AvatarDescription.CreateRandom();
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription);
			this.DefaultSkeleton();
			this._expression = default(AvatarExpression);
		}

		public static bool Compare(AvatarDescription ad1, AvatarDescription ad2)
		{
			if (ad1.Description.Length != ad1.Description.Length)
			{
				return false;
			}
			for (int i = 0; i < ad1.Description.Length; i++)
			{
				if (ad1.Description[i] != ad2.Description[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool Compare(byte[] ad1, byte[] ad2)
		{
			if (ad1.Length != ad1.Length)
			{
				return false;
			}
			for (int i = 0; i < ad1.Length; i++)
			{
				if (ad1[i] != ad2[i])
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void OnDescriptionChanged()
		{
		}

		public void SetDescription(byte[] description)
		{
			if (Avatar.Compare(this._avatarDescription.Description, description))
			{
				return;
			}
			this._invisible = false;
			this.OnDescriptionChanged();
			this._avatarDescription = new AvatarDescription(description);
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription);
			this._skeletonBuilt = false;
		}

		public void SetDescription(AvatarDescription description)
		{
			if (Avatar.Compare(this._avatarDescription, description))
			{
				return;
			}
			this._invisible = false;
			this.OnDescriptionChanged();
			this._avatarDescription = description;
			this._avatarRenderer = new AvatarRenderer(this._avatarDescription);
			this._skeletonBuilt = false;
		}

		private void DrawWireframeBones(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
		{
			if (this._wireFrameWorldTransforms == null)
			{
				this._wireFrameWorldTransforms = new Matrix[this.Skeleton.Count];
			}
			Matrix.CreateRotationY(3.14159274f);
			this._wireFrameWorldTransforms[0] = this._boneTransformBuffer[0] * Avatar.DefaultBindPose[0];
			for (int i = 1; i < this._wireFrameWorldTransforms.Length; i++)
			{
				Matrix matrix = this._boneTransformBuffer[i];
				matrix.Translation = Avatar.DefaultBindPose[i].Translation;
				this._wireFrameWorldTransforms[i] = matrix;
			}
			this._wireFrameWorldTransforms[0] *= base.LocalToWorld;
			for (int j = 1; j < this._wireFrameWorldTransforms.Length; j++)
			{
				Matrix.Multiply(ref this._wireFrameWorldTransforms[j], ref this._wireFrameWorldTransforms[Avatar.DefaultParentBones[j]], out this._wireFrameWorldTransforms[j]);
			}
			if (this._wireFrameVerts == null)
			{
				this._wireFrameVerts = new VertexPositionColor[this._wireFrameWorldTransforms.Length * 2];
			}
			this._wireFrameVerts[0].Color = Color.Blue;
			this._wireFrameVerts[0].Position = this._wireFrameWorldTransforms[0].Translation;
			this._wireFrameVerts[1] = this._wireFrameVerts[0];
			for (int k = 2; k < this._wireFrameWorldTransforms.Length * 2; k += 2)
			{
				this._wireFrameVerts[k].Position = this._wireFrameWorldTransforms[k / 2].Translation;
				this._wireFrameVerts[k].Color = Color.Red;
				this._wireFrameVerts[k + 1].Position = this._wireFrameWorldTransforms[Avatar.DefaultParentBones[k / 2]].Translation;
				this._wireFrameVerts[k + 1].Color = Color.Green;
			}
			if (this._wireFrameEffect == null)
			{
				this._wireFrameEffect = new BasicEffect(graphicsDevice);
			}
			this._wireFrameEffect.LightingEnabled = false;
			this._wireFrameEffect.TextureEnabled = false;
			this._wireFrameEffect.VertexColorEnabled = true;
			this._wireFrameEffect.Projection = projection;
			this._wireFrameEffect.View = view;
			this._wireFrameEffect.World = Matrix.Identity;
			for (int l = 0; l < this._wireFrameEffect.CurrentTechnique.Passes.Count; l++)
			{
				EffectPass effectPass = this._wireFrameEffect.CurrentTechnique.Passes[l];
				effectPass.Apply();
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, this._wireFrameVerts, 0, this._wireFrameWorldTransforms.Length);
			}
		}
	}
}
