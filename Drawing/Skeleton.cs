using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing
{
	public class Skeleton : ReadOnlyCollection<Bone>
	{
		public class Reader : ContentTypeReader<Skeleton>
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name=""></param>
			protected override Skeleton Read(ContentReader input, Skeleton existingInstance)
			{
				if (existingInstance != null)
				{
					throw new NotImplementedException();
				}

				return Skeleton.Load(input);
			}
		}

		public Dictionary<string, Bone> boneLookup = new Dictionary<string, Bone>();
		private Bone[] _bones;

		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Reset();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void Save(BinaryWriter writer)
		{
			writer.Write(base.Count);
			
			for (int i = 0; i < base.Count; i++)
			{
				Bone bone = base[i];
				writer.Write(bone.Name);
				writer.Write((bone.Parent == null) ? -1 : bone.Parent.Index);
				writer.Write(bone.Transform);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public static Skeleton Load(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			string[] names = new string[length];
			int[] hierarchy = new int[length];
			Matrix[] boneTransforms = new Matrix[length];
			
			for (int i = 0; i < length; i++)
			{
				names[i] = reader.ReadString();
				hierarchy[i] = reader.ReadInt32();
				boneTransforms[i] = reader.ReadMatrix();
			}
			
			return Bone.BuildSkeleton(boneTransforms, hierarchy, names);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Skeleton(IList<Bone> bones) 
			: base(bones)
		{
			this._bones = new Bone[bones.Count];
			bones.CopyTo(this._bones, 0);
			
			for (int i = 0; i < bones.Count; i++)
			{
				if (bones[i].Name != null)
				{
					this.boneLookup[bones[i].Name] = bones[i];
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public Bone this[string boneName] =>
			this.boneLookup[boneName];

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public int IndexOf(string boneName) =>
			this.boneLookup[boneName].Index;

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public IList<Bone> BonesFromNames(IList<string> boneNames)
		{
			Bone[] bones = new Bone[boneNames.Count];
			
			for (int i = 0; i < boneNames.Count; i++)
			{
				bones[i] = this.boneLookup[boneNames[i]];
			}
			
			return bones;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTransformsFrom(Matrix[] sourceBoneTransforms)
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].SetTransform(sourceBoneTransforms[i]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyTransformsTo(Matrix[] destinationBoneTransforms)
		{
			for (int i = 0; i < this._bones.Length; i++)
			{
				this._bones[i].GetTransform(out destinationBoneTransforms[i]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		public void CopyAbsoluteBoneTransformsTo(Matrix[] worldBoneTransforms, 
												 Matrix localToWorld)
		{
			for (int i = 0; i < base.Count; i++)
			{
				Bone bone = base[i];
			
				if (bone.Parent == null)
				{
					this.CopyAbsoluteBoneTransformsTo(
						bone, worldBoneTransforms, ref localToWorld);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name=""></param>
		private void CopyAbsoluteBoneTransformsTo(Bone bone, Matrix[] worldBoneTransforms, 
												  ref Matrix localToWorld)
		{
			bone.EnsureTransformComposed();
			
			if (bone.Parent == null)
			{
				Matrix.Multiply(
					ref bone._transform, ref localToWorld, out worldBoneTransforms[bone.Index]);
			}
			else
			{
				Matrix.Multiply(ref bone._transform, ref worldBoneTransforms[bone.Parent.Index], 
								out worldBoneTransforms[bone.Index]);
			}
			
			ReadOnlyCollection<Bone> children = bone.Children;
			int count = bone.Children.Count;
			
			for (int i = 0; i < count; i++)
			{
				this.CopyAbsoluteBoneTransformsTo(
					bone.Children[i], worldBoneTransforms, ref localToWorld);
			}
		}
	}
}
