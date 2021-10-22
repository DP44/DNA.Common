using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Exporting
{
	public abstract class Exporter
	{
		protected string _exportFilePath;

		public abstract void ExportMesh(Matrix worldPos, IList<ExportVertex> verts, 
			IList<TriangleVertexIndices> triangleIndicies, string name = "Mesh");

		public virtual void BeginExport(string exportPath)
		{
			if (this._exportFilePath != null)
			{
				throw new Exception("Begin Export Already Called");
			}
			
			this._exportFilePath = Path.GetFullPath(exportPath);
		}

		public virtual void EndExport() =>
			this._exportFilePath = null;
	}
}
