using System;
using System.Collections.Generic;
using System.IO;

namespace DNA.IO
{
	public class HTFDocument
	{
		private HTFElement _root;

		public HTFElement Root
		{
			get =>
				this._root;

			set =>
				this._root = value;
		}

		public List<HTFElement> Children =>
			this._root.Children;

		public HTFDocument() =>
			this._root = new HTFElement();

		public HTFDocument(string fileName)
		{
			using (FileStream fileStream = File.Open(
				fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				this.Load(fileStream);
			}
		}

		public HTFDocument(Stream stream)
		{
			this.Load(stream);
		}

		public void Load(Stream stream)
		{
			this._root = new HTFElement(stream);
		}

		public void Load(string path)
		{
			using (FileStream fileStream = File.Open(
				path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				this._root = new HTFElement(fileStream);
			}
		}

		public void Save(string filename)
		{
			using (FileStream fileStream = File.Open(
				filename, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				this.Save(fileStream);
			}
		}

		public void Save(Stream stream)
		{
			StreamWriter writer = new StreamWriter(stream);
			this.Save(writer);
		}

		public void Save(StreamWriter writer)
		{
			this._root.Save(writer);
			writer.Flush();
		}

		public override string ToString()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(memoryStream);
			this.Save(writer);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			return streamReader.ReadToEnd();
		}

		public void LoadFromString(string data)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(data);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			this.Load(memoryStream);
		}
	}
}
