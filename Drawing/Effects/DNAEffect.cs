using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Effects
{
	public class DNAEffect : Effect, IEffectMatrices, IEffectTime, IEffectColor, IEffectTextured
	{
		public enum EffectValueTypes : byte
		{
			intValue,
			stringValue,
			boolValue,
			floatValue,
			Vector2Value,
			Vector3Value,
			Vector4Value,
			MatrixValue
		}

		public class Reader : ContentTypeReader<DNAEffect>
		{
			protected override DNAEffect Read(ContentReader input, DNAEffect existingInstance)
			{
				Effect cloneSource = input.ReadExternalReference<Effect>();
				int num1 = input.ReadInt32();
				for (int index = 0; index < num1; ++index)
				{
					string name = input.ReadString();
					Texture texture = input.ReadExternalReference<Texture>();
					cloneSource.Parameters[name].SetValue(texture);
				}
				int num2 = input.ReadInt32();
				for (int index1 = 0; index1 < num2; ++index1)
				{
					string name = input.ReadString();
					EffectParameter parameter = cloneSource.Parameters[name];
					DNAEffect.EffectValueTypes effectValueTypes = (DNAEffect.EffectValueTypes) input.ReadByte();
					int length = input.ReadInt32();
					if (parameter == null)
					{
						if (length == 0)
						{
							switch (effectValueTypes)
							{
								case DNAEffect.EffectValueTypes.intValue:
									input.ReadInt32();
									continue;
								case DNAEffect.EffectValueTypes.stringValue:
									input.ReadString();
									continue;
								case DNAEffect.EffectValueTypes.boolValue:
									input.ReadBoolean();
									continue;
								case DNAEffect.EffectValueTypes.floatValue:
									double num3 = (double) input.ReadSingle();
									continue;
								case DNAEffect.EffectValueTypes.Vector2Value:
									input.ReadVector2();
									continue;
								case DNAEffect.EffectValueTypes.Vector3Value:
									input.ReadVector3();
									continue;
								case DNAEffect.EffectValueTypes.Vector4Value:
									input.ReadVector4();
									continue;
								case DNAEffect.EffectValueTypes.MatrixValue:
									input.ReadMatrix();
									continue;
								default:
									throw new Exception("Unsupported Value Type");
							}
						}
						else
						{
							switch (effectValueTypes)
							{
								case DNAEffect.EffectValueTypes.intValue:
									foreach (int num4 in new int[length])
										num4 = input.ReadInt32();
									continue;
								case DNAEffect.EffectValueTypes.boolValue:
									foreach (bool flag in new bool[length])
										flag = input.ReadBoolean();
									continue;
								case DNAEffect.EffectValueTypes.floatValue:
									foreach (float num5 in new float[length])
										num5 = input.ReadSingle();
									continue;
								case DNAEffect.EffectValueTypes.Vector2Value:
									foreach (Vector2 vector2 in new Vector2[length])
										vector2 = input.ReadVector2();
									continue;
								case DNAEffect.EffectValueTypes.Vector3Value:
									foreach (Vector3 vector3 in new Vector3[length])
										vector3 = input.ReadVector3();
									continue;
								case DNAEffect.EffectValueTypes.Vector4Value:
									foreach (Vector4 vector4 in new Vector4[length])
										vector4 = input.ReadVector4();
									continue;
								case DNAEffect.EffectValueTypes.MatrixValue:
									foreach (Matrix matrix in new Matrix[length])
										matrix = input.ReadMatrix();
									continue;
								default:
									throw new Exception("Unsupported Value Type");
							}
						}
					}
					else if (length == 0)
					{
						switch (effectValueTypes)
						{
							case DNAEffect.EffectValueTypes.intValue:
								parameter.SetValue(input.ReadInt32());
								continue;
							case DNAEffect.EffectValueTypes.stringValue:
								parameter.SetValue(input.ReadString());
								continue;
							case DNAEffect.EffectValueTypes.boolValue:
								parameter.SetValue(input.ReadBoolean());
								continue;
							case DNAEffect.EffectValueTypes.floatValue:
								parameter.SetValue(input.ReadSingle());
								continue;
							case DNAEffect.EffectValueTypes.Vector2Value:
								parameter.SetValue(input.ReadVector2());
								continue;
							case DNAEffect.EffectValueTypes.Vector3Value:
								parameter.SetValue(input.ReadVector3());
								continue;
							case DNAEffect.EffectValueTypes.Vector4Value:
								Vector4 vector4_1 = input.ReadVector4();
								if (parameter.ColumnCount == 2)
								{
									parameter.SetValue(new Vector2(vector4_1.X, vector4_1.Y));
									continue;
								}
								if (parameter.ColumnCount == 3)
								{
									parameter.SetValue(new Vector3(vector4_1.X, vector4_1.Y, vector4_1.Z));
									continue;
								}
								parameter.SetValue(vector4_1);
								continue;
							case DNAEffect.EffectValueTypes.MatrixValue:
								cloneSource.Parameters[name].SetValue(input.ReadMatrix());
								continue;
							default:
								throw new Exception("Unsupported Value Type");
						}
					}
					else
					{
						switch (effectValueTypes)
						{
							case DNAEffect.EffectValueTypes.intValue:
								int[] numArray1 = new int[length];
								for (int index2 = 0; index2 < numArray1.Length; ++index2)
									numArray1[index2] = input.ReadInt32();
								parameter.SetValue(numArray1);
								continue;
							case DNAEffect.EffectValueTypes.boolValue:
								bool[] flagArray = new bool[length];
								for (int index3 = 0; index3 < flagArray.Length; ++index3)
									flagArray[index3] = input.ReadBoolean();
								parameter.SetValue(flagArray);
								continue;
							case DNAEffect.EffectValueTypes.floatValue:
								float[] numArray2 = new float[length];
								for (int index4 = 0; index4 < numArray2.Length; ++index4)
									numArray2[index4] = input.ReadSingle();
								parameter.SetValue(numArray2);
								continue;
							case DNAEffect.EffectValueTypes.Vector2Value:
								Vector2[] vector2Array = new Vector2[length];
								for (int index5 = 0; index5 < vector2Array.Length; ++index5)
									vector2Array[index5] = input.ReadVector2();
								parameter.SetValue(vector2Array);
								continue;
							case DNAEffect.EffectValueTypes.Vector3Value:
								Vector3[] vector3Array = new Vector3[length];
								for (int index6 = 0; index6 < vector3Array.Length; ++index6)
									vector3Array[index6] = input.ReadVector3();
								parameter.SetValue(vector3Array);
								continue;
							case DNAEffect.EffectValueTypes.Vector4Value:
								Vector4[] vector4Array = new Vector4[length];
								for (int index7 = 0; index7 < vector4Array.Length; ++index7)
									vector4Array[index7] = input.ReadVector4();
								parameter.SetValue(vector4Array);
								continue;
							case DNAEffect.EffectValueTypes.MatrixValue:
								Matrix[] matrixArray = new Matrix[length];
								for (int index8 = 0; index8 < matrixArray.Length; ++index8)
									matrixArray[index8] = input.ReadMatrix();
								parameter.SetValue(matrixArray);
								continue;
							
							default:
							{
								throw new Exception("Unsupported Value Type");
							}
						}
					}
				}
				return new DNAEffect(cloneSource);
			}
		}

		[Flags]
		private enum ParamFlags : uint
		{
			None = 0U,
			World = 1U,
			View = 2U,
			Projection = 4U,
			Time = 8U,
			ElaspedTime = 16U,
			Diffuse = 32U,
			Ambient = 64U,
			Emissive = 128U,
			Specular = 256U,
			DiffuseMap = 512U,
			OpacityMap = 1024U,
			SpecularMap = 2048U,
			NormalMap = 4096U,
			DisplacementMap = 8192U,
			LightMap = 16384U,
			ReflectionMap = 32768U,
			MatrixFlags = 7U,
			TimeFlags = 24U,
			ColorFlags = 480U,
			MapFlags = 65024U,
			AllFlags = 4294967295U
		}

		private DNAEffect.ParamFlags _alteredParams;

		private EffectParameter _worldParam;

		private EffectParameter _worldInvParam;

		private EffectParameter _worldInvTrnParam;

		private EffectParameter _worldTrnParam;

		private EffectParameter _viewParam;

		private EffectParameter _viewTrnParam;

		private EffectParameter _viewInvParam;

		private EffectParameter _viewInvTrnParam;

		private EffectParameter _projParam;

		private EffectParameter _projTrnParam;

		private EffectParameter _projInvParam;

		private EffectParameter _projInvTrnParam;

		private EffectParameter _worldViewParam;

		private EffectParameter _worldViewInvParam;

		private EffectParameter _worldViewInvTrnParam;

		private EffectParameter _worldViewProjParam;

		private EffectParameter _worldViewProjInvParam;

		private EffectParameter _worldViewProjInvTrnParam;

		private EffectParameter _totalTimeParam;

		private EffectParameter _elaspedTimeParam;

		private EffectParameter _diffuseColorParam;

		private EffectParameter _ambientColorParam;

		private EffectParameter _emissiveColorParam;

		private EffectParameter _specularColorParam;

		private EffectParameter _diffuseMapParam;

		private EffectParameter _opacityMapParam;

		private EffectParameter _specularMapParam;

		private EffectParameter _normalMapParam;

		private EffectParameter _displacementMapParam;

		private EffectParameter _lightMapParam;

		private EffectParameter _reflectionMapParam;

		private Matrix _world;

		private Matrix _view;

		private Matrix _proj;

		private TimeSpan _elaspedTime;

		private TimeSpan _totalTime;

		private ColorF _diffuseColor = Color.White;

		private ColorF _ambientColor = Color.Gray;

		private ColorF _specularColor = Color.White;

		private ColorF _emissiveColor;

		private Texture _diffuseMap;

		private Texture _opacityMap;

		private Texture _specularMap;

		private Texture _normalMap;

		private Texture _displacementMap;

		private Texture _lightMap;

		private Texture _reflectionMap;

		public DNAEffect(DNAEffect cloneSource) : base(cloneSource)
		{
			base.GraphicsDevice.DeviceReset += this.GraphicsDevice_DeviceReset;
			this.SetupParams();
		}

		public DNAEffect(Effect cloneSource) : base(cloneSource)
		{
			base.GraphicsDevice.DeviceReset += this.GraphicsDevice_DeviceReset;
			this.SetupParams();
		}

		private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
		{
			this._alteredParams = (DNAEffect.ParamFlags)4294967295U;
		}

		public Matrix Projection
		{
			get
			{
				return this._proj;
			}
			set
			{
				this._proj = value;
				this._alteredParams |= DNAEffect.ParamFlags.Projection;
			}
		}

		public Matrix View
		{
			get
			{
				return this._view;
			}
			set
			{
				this._view = value;
				this._alteredParams |= DNAEffect.ParamFlags.View;
			}
		}

		public Matrix World
		{
			get
			{
				return this._world;
			}
			set
			{
				this._world = value;
				this._alteredParams |= DNAEffect.ParamFlags.World;
			}
		}

		public TimeSpan TotalTime
		{
			get
			{
				return this._totalTime;
			}
			set
			{
				this._totalTime = value;
				if (this._totalTimeParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.Time;
				}
			}
		}

		public TimeSpan ElaspedTime
		{
			get
			{
				return this._elaspedTime;
			}
			set
			{
				this._elaspedTime = value;
				if (this._elaspedTimeParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.ElaspedTime;
				}
			}
		}

		public ColorF DiffuseColor
		{
			get
			{
				return this._diffuseColor;
			}
			set
			{
				this._diffuseColor = value;
				if (this._diffuseColorParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.Diffuse;
				}
			}
		}

		public ColorF AmbientColor
		{
			get
			{
				return this._ambientColor;
			}
			set
			{
				this._ambientColor = value;
				if (this._ambientColorParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.Ambient;
				}
			}
		}

		public ColorF SpecularColor
		{
			get
			{
				return this._specularColor;
			}
			set
			{
				this._specularColor = value;
				if (this._specularColorParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.Specular;
				}
			}
		}

		public ColorF EmissiveColor
		{
			get
			{
				return this._emissiveColor;
			}
			set
			{
				this._emissiveColor = value;
				if (this._emissiveColorParam != null)
				{
					this._alteredParams |= DNAEffect.ParamFlags.Emissive;
				}
			}
		}

		public Texture DiffuseMap
		{
			get
			{
				return this._diffuseMap;
			}
			set
			{
				this._diffuseMap = value;
				if (this._diffuseMap != null)
				{
					if (this._diffuseMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.DiffuseMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.DiffuseMap;
				}
			}
		}

		public Texture OpacityMap
		{
			get
			{
				return this._opacityMap;
			}
			set
			{
				this._opacityMap = value;
				if (this._opacityMap != null)
				{
					if (this._opacityMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.OpacityMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.OpacityMap;
				}
			}
		}

		public Texture SpecularMap
		{
			get
			{
				return this._specularMap;
			}
			set
			{
				this._specularMap = value;
				if (this._specularMap != null)
				{
					if (this._specularMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.SpecularMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.SpecularMap;
				}
			}
		}

		public Texture NormalMap
		{
			get
			{
				return this._normalMap;
			}
			set
			{
				this._normalMap = value;
				if (this._normalMap != null)
				{
					if (this._normalMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.NormalMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.NormalMap;
				}
			}
		}

		public Texture DisplacementMap
		{
			get
			{
				return this._displacementMap;
			}
			set
			{
				this._displacementMap = value;
				if (this._displacementMap != null)
				{
					if (this._displacementMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.DisplacementMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.DisplacementMap;
				}
			}
		}

		public Texture LightMap
		{
			get
			{
				return this._lightMap;
			}
			set
			{
				this._lightMap = value;
				if (this._lightMap != null)
				{
					if (this._lightMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.LightMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.LightMap;
				}
			}
		}

		public Texture ReflectionMap
		{
			get
			{
				return this._reflectionMap;
			}
			set
			{
				this._reflectionMap = value;
				if (this._reflectionMap != null)
				{
					if (this._reflectionMapParam != null)
					{
						this._alteredParams |= DNAEffect.ParamFlags.ReflectionMap;
						return;
					}
				}
				else
				{
					this._alteredParams &= ~DNAEffect.ParamFlags.ReflectionMap;
				}
			}
		}

		public void SetupParams()
		{
			this._worldParam = base.Parameters.GetParameterBySemantic("WORLD");
			this._worldInvParam = base.Parameters.GetParameterBySemantic("WORLDI");
			this._worldInvTrnParam = base.Parameters.GetParameterBySemantic("WORLDIT");
			this._worldTrnParam = base.Parameters.GetParameterBySemantic("WORLDT");
			this._viewParam = base.Parameters.GetParameterBySemantic("VIEW");
			this._viewTrnParam = base.Parameters.GetParameterBySemantic("VIEWT");
			this._viewInvParam = base.Parameters.GetParameterBySemantic("VIEWI");
			this._viewInvTrnParam = base.Parameters.GetParameterBySemantic("VIEWIT");
			this._projParam = base.Parameters.GetParameterBySemantic("PROJECTION");
			this._projTrnParam = base.Parameters.GetParameterBySemantic("PROJECTIONT");
			this._projInvParam = base.Parameters.GetParameterBySemantic("PROJECTIONI");
			this._projInvTrnParam = base.Parameters.GetParameterBySemantic("PROJECTIONIT");
			this._worldViewParam = base.Parameters.GetParameterBySemantic("WORLDVIEW");
			this._worldViewInvParam = base.Parameters.GetParameterBySemantic("WORLDVIEWI");
			this._worldViewInvTrnParam = base.Parameters.GetParameterBySemantic("WORLDVIEWIT");
			this._worldViewProjParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJ");
			if (this._worldViewProjParam == null)
			{
				this._worldViewProjParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJECTION");
			}
			this._worldViewProjInvParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJI");
			if (this._worldViewProjInvParam == null)
			{
				this._worldViewProjInvParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJECTIONI");
			}
			this._worldViewProjInvTrnParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJIT");
			if (this._worldViewProjInvTrnParam == null)
			{
				this._worldViewProjInvTrnParam = base.Parameters.GetParameterBySemantic("WORLDVIEWPROJECTIONIT");
			}
			this._totalTimeParam = base.Parameters.GetParameterBySemantic("TIMETOTAL");
			if (this._totalTimeParam == null)
			{
				this._totalTimeParam = base.Parameters.GetParameterBySemantic("TIME");
			}
			this._elaspedTimeParam = base.Parameters.GetParameterBySemantic("TIMEELASPED");
			this._diffuseColorParam = base.Parameters.GetParameterBySemantic("DIFFUSECOLOR");
			this._ambientColorParam = base.Parameters.GetParameterBySemantic("AMBIENTCOLOR");
			this._emissiveColorParam = base.Parameters.GetParameterBySemantic("EMISSIVECOLOR");
			this._specularColorParam = base.Parameters.GetParameterBySemantic("SPECULARCOLOR");
			if (this._diffuseColorParam != null)
			{
				this._diffuseColor = this.GetColor(this._diffuseColorParam);
			}
			if (this._ambientColorParam != null)
			{
				this._ambientColor = this.GetColor(this._ambientColorParam);
			}
			if (this._emissiveColorParam != null)
			{
				this._emissiveColor = this.GetColor(this._emissiveColorParam);
			}
			if (this._specularColorParam != null)
			{
				this._specularColor = this.GetColor(this._specularColorParam);
			}
			this._diffuseColorParam = base.Parameters.GetParameterBySemantic("DIFFUSECOLOR");
			this._diffuseMapParam = base.Parameters.GetParameterBySemantic("DIFFUSEMAP");
			this._opacityMapParam = base.Parameters.GetParameterBySemantic("OPACITYMAP");
			this._specularMapParam = base.Parameters.GetParameterBySemantic("SPECULARMAP");
			this._normalMapParam = base.Parameters.GetParameterBySemantic("NORMALMAP");
			if (this._normalMapParam == null)
			{
				this._normalMapParam = base.Parameters.GetParameterBySemantic("BUMPMAP");
			}
			this._displacementMapParam = base.Parameters.GetParameterBySemantic("DISPLACEMENTMAP");
			this._lightMapParam = base.Parameters.GetParameterBySemantic("LIGHTMAP");
			this._reflectionMapParam = base.Parameters.GetParameterBySemantic("REFLECTIONMAP");
			this._diffuseMap = this.GetTexture(this._diffuseMapParam);
			this._opacityMap = this.GetTexture(this._opacityMapParam);
			this._specularMap = this.GetTexture(this._specularMapParam);
			this._normalMap = this.GetTexture(this._normalMapParam);
			this._displacementMap = this.GetTexture(this._displacementMapParam);
			this._lightMap = this.GetTexture(this._lightMapParam);
			this._reflectionMap = this.GetTexture(this._reflectionMapParam);
		}

		private ColorF GetColor(EffectParameter param)
		{
			if (param.ColumnCount == 3)
			{
				return ColorF.FromVector3(param.GetValueVector3());
			}
			if (param.ColumnCount == 4)
			{
				return ColorF.FromVector4(param.GetValueVector4());
			}
			throw new Exception("Bad Color Value:" + param.ColumnCount.ToString());
		}

		private Texture GetTexture(EffectParameter param)
		{
			if (param == null)
			{
				return null;
			}
			switch (param.ParameterType)
			{
			case EffectParameterType.Texture:
				return param.GetValueTexture2D();
			case EffectParameterType.Texture1D:
				return null;
			case EffectParameterType.Texture2D:
				return param.GetValueTexture2D();
			case EffectParameterType.Texture3D:
				return param.GetValueTexture3D();
			case EffectParameterType.TextureCube:
				return param.GetValueTextureCube();
			default:
				return null;
			}
		}

		public override Effect Clone()
		{
			return new DNAEffect(this);
		}

		protected override void OnApply()
		{
			if ((this._alteredParams & DNAEffect.ParamFlags.MatrixFlags) != DNAEffect.ParamFlags.None)
			{
				if (this._worldParam != null)
				{
					this._worldParam.SetValue(this._world);
				}
				if (this._worldInvParam != null || this._worldInvTrnParam != null)
				{
					Matrix matrix;
					Matrix.Invert(ref this._world, out matrix);
					if (this._worldInvParam != null)
					{
						this._worldInvParam.SetValue(matrix);
					}
					if (this._worldInvTrnParam != null)
					{
						this._worldInvTrnParam.SetValue(Matrix.Transpose(matrix));
					}
				}
				if (this._worldTrnParam != null)
				{
					this._worldTrnParam.SetValue(Matrix.Transpose(this._world));
				}
				if (this._viewParam != null)
				{
					this._viewParam.SetValue(this._view);
				}
				if (this._viewInvParam != null || this._viewInvTrnParam != null)
				{
					Matrix matrix2;
					Matrix.Invert(ref this._view, out matrix2);
					if (this._viewInvParam != null)
					{
						this._viewInvParam.SetValue(matrix2);
					}
					if (this._viewInvTrnParam != null)
					{
						this._viewInvTrnParam.SetValue(Matrix.Transpose(matrix2));
					}
				}
				if (this._viewTrnParam != null)
				{
					this._viewTrnParam.SetValue(Matrix.Transpose(this._view));
				}
				if (this._projParam != null)
				{
					this._projParam.SetValue(this._proj);
				}
				if (this._projInvParam != null || this._projInvTrnParam != null)
				{
					Matrix matrix3;
					Matrix.Invert(ref this._proj, out matrix3);
					if (this._projInvParam != null)
					{
						this._projInvParam.SetValue(matrix3);
					}
					if (this._projInvTrnParam != null)
					{
						this._projInvTrnParam.SetValue(Matrix.Transpose(matrix3));
					}
				}
				if (this._projTrnParam != null)
				{
					this._projTrnParam.SetValue(Matrix.Transpose(this._proj));
				}
				if (this._worldViewParam != null || this._worldViewInvParam != null || this._worldViewInvTrnParam != null || this._worldViewProjParam != null || this._worldViewProjInvParam != null || this._worldViewProjInvTrnParam != null)
				{
					Matrix matrix4;
					Matrix.Multiply(ref this._world, ref this._view, out matrix4);
					if (this._worldViewParam != null)
					{
						this._worldViewParam.SetValue(matrix4);
					}
					if (this._worldViewInvParam != null || this._worldViewInvTrnParam != null)
					{
						Matrix matrix5 = Matrix.Invert(matrix4);
						if (this._worldViewInvParam != null)
						{
							this._worldViewInvParam.SetValue(matrix5);
						}
						if (this._worldViewInvTrnParam != null)
						{
							this._worldViewInvTrnParam.SetValue(Matrix.Transpose(matrix5));
						}
					}
					if (this._worldViewProjParam != null || this._worldViewProjInvParam != null || this._worldViewProjInvTrnParam != null)
					{
						Matrix matrix6;
						Matrix.Multiply(ref matrix4, ref this._proj, out matrix6);
						if (this._worldViewProjParam != null)
						{
							this._worldViewProjParam.SetValue(matrix6);
						}
						if (this._worldViewProjInvParam != null || this._worldViewProjInvTrnParam != null)
						{
							Matrix matrix7 = Matrix.Invert(matrix6);
							if (this._worldViewProjInvParam != null)
							{
								this._worldViewProjInvParam.SetValue(matrix7);
							}
							if (this._worldViewProjInvTrnParam != null)
							{
								this._worldViewProjInvTrnParam.SetValue(Matrix.Transpose(matrix7));
							}
						}
					}
				}
			}
			if (this._totalTimeParam != null)
			{
				this._totalTimeParam.SetValue((float)this._totalTime.TotalSeconds);
			}
			if (this._elaspedTimeParam != null)
			{
				this._elaspedTimeParam.SetValue((float)this._elaspedTime.TotalSeconds);
			}
			if ((this._alteredParams & DNAEffect.ParamFlags.ColorFlags) != DNAEffect.ParamFlags.None)
			{
				if ((this._alteredParams & DNAEffect.ParamFlags.Diffuse) != DNAEffect.ParamFlags.None)
				{
					this.SetColor(this._diffuseColorParam, this._diffuseColor);
				}
				if ((this._alteredParams & DNAEffect.ParamFlags.Ambient) != DNAEffect.ParamFlags.None)
				{
					this.SetColor(this._ambientColorParam, this._ambientColor);
				}
				if ((this._alteredParams & DNAEffect.ParamFlags.Specular) != DNAEffect.ParamFlags.None)
				{
					this.SetColor(this._specularColorParam, this._specularColor);
				}
				if ((this._alteredParams & DNAEffect.ParamFlags.Emissive) != DNAEffect.ParamFlags.None)
				{
					this.SetColor(this._emissiveColorParam, this._emissiveColor);
				}
			}
			if ((this._alteredParams & DNAEffect.ParamFlags.MapFlags) != DNAEffect.ParamFlags.None)
			{
				if (this._diffuseMapParam != null && this._diffuseMap != null)
				{
					this._diffuseMapParam.SetValue(this._diffuseMap);
				}
				if (this._opacityMapParam != null && this._opacityMap != null)
				{
					this._opacityMapParam.SetValue(this._opacityMap);
				}
				if (this._specularMapParam != null && this._specularMap != null)
				{
					this._specularMapParam.SetValue(this._specularMap);
				}
				if (this._normalMapParam != null && this._normalMap != null)
				{
					this._normalMapParam.SetValue(this._normalMap);
				}
				if (this._displacementMapParam != null && this._displacementMap != null)
				{
					this._displacementMapParam.SetValue(this._displacementMap);
				}
				if (this._lightMapParam != null && this._lightMap != null)
				{
					this._lightMapParam.SetValue(this._lightMap);
				}
				if (this._reflectionMapParam != null && this._reflectionMap != null)
				{
					this._reflectionMapParam.SetValue(this._reflectionMap);
				}
			}
			base.OnApply();
			this._alteredParams = DNAEffect.ParamFlags.None;
		}

		private void SetColor(EffectParameter param, ColorF color)
		{
			if (param == null)
			{
				return;
			}
			if (param.ColumnCount == 3)
			{
				param.SetValue(color.ToVector3());
				return;
			}
			if (param.ColumnCount == 4)
			{
				param.SetValue(color.ToVector4());
				return;
			}
			throw new Exception("Bad Color Value:" + param.Name);
		}
	}
}
