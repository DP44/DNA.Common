using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace DNA.Net.GamerServices
{
	public class AvatarRenderer : IDisposable
	{
		[CLSCompliant(false)]
		public const int BoneCount = 71;

		public Vector3 AmbientLightColor;

		private Matrix[] _bindPose = new Matrix[]
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
		};

		public Vector3 LightColor;
		public Vector3 LightDirection;

		private ReadOnlyCollection<int> _parentBones = new ReadOnlyCollection<int>(new int[]
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

		public Matrix Projection;
		public AvatarRendererState State = AvatarRendererState.Ready;
		public Matrix View;
		public Matrix World;

		public AvatarRenderer(AvatarDescription avatarDescription) {}
		public AvatarRenderer(AvatarDescription avatarDescription, bool useLoadingEffect) {}

		public ReadOnlyCollection<Matrix> BindPose =>
			new ReadOnlyCollection<Matrix>(this._bindPose);

		public bool IsDisposed =>
			false;

		public ReadOnlyCollection<int> ParentBones =>
			this._parentBones;

		public void Dispose() {}

		protected virtual void Dispose(bool disposing) {}

		public void Draw(IAvatarAnimation animation) {}
		public void Draw(IList<Matrix> bones, AvatarExpression expression) {}
	}
}
