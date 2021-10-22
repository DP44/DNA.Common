using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DNA.IO;
using DNA.Reflection;

namespace DNA.Runtime.Serialization.Formatters.HTF
{
	public class HTFFormatter : IFormatter
	{
		private class FormatConverter : IFormatterConverter
		{
			public object Convert(object value, TypeCode typeCode)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public object Convert(object value, Type type)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public bool ToBoolean(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public byte ToByte(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public char ToChar(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public DateTime ToDateTime(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public decimal ToDecimal(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public double ToDouble(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public short ToInt16(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public int ToInt32(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public long ToInt64(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public sbyte ToSByte(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public float ToSingle(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public string ToString(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public ushort ToUInt16(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public uint ToUInt32(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}

			public ulong ToUInt64(object value)
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		private static Dictionary<Type, string> PrimitiveLookup;
		private Dictionary<object, int> _objectLookup;
		private Dictionary<int, object> _objectIDLookup;
		private static int Version = 1;
		private int _nextID;
		private SerializationBinder _binder = new DefaultSerializationBinder();
		private StreamingContext _context = new StreamingContext(StreamingContextStates.All);

		static HTFFormatter()
		{
			HTFFormatter.PrimitiveLookup = new Dictionary<Type, string>();
			HTFFormatter.PrimitiveLookup[typeof(bool)] = "bool";
			HTFFormatter.PrimitiveLookup[typeof(byte)] = "byte";
			HTFFormatter.PrimitiveLookup[typeof(sbyte)] = "sbyte";
			HTFFormatter.PrimitiveLookup[typeof(short)] = "short";
			HTFFormatter.PrimitiveLookup[typeof(ushort)] = "ushort";
			HTFFormatter.PrimitiveLookup[typeof(int)] = "int";
			HTFFormatter.PrimitiveLookup[typeof(uint)] = "uint";
			HTFFormatter.PrimitiveLookup[typeof(long)] = "long";
			HTFFormatter.PrimitiveLookup[typeof(ulong)] = "ulong";
			HTFFormatter.PrimitiveLookup[typeof(char)] = "char";
			HTFFormatter.PrimitiveLookup[typeof(double)] = "double";
			HTFFormatter.PrimitiveLookup[typeof(float)] = "float";
			HTFFormatter.PrimitiveLookup[typeof(string)] = "string";
		}

		public HTFDocument Serialize(object graph)
		{
			this._nextID = 0;
			this._objectLookup = new Dictionary<object, int>();
			this._objectIDLookup = new Dictionary<int, object>();
			HTFDocument htfdocument = new HTFDocument();
			htfdocument.Root.Children.Add(new HTFElement("DocType", "HTFGraph"));
			htfdocument.Root.Children.Add(new HTFElement("Version", HTFFormatter.Version.ToString()));
			HTFElement htfelement = new HTFElement();
			htfelement.ID = "Graph";
			htfdocument.Root.Children.Add(htfelement);
			this.Serialize(htfelement, graph);
			return htfdocument;
		}

		public void Serialize(Stream serializationStream, object graph)
		{
			HTFDocument htfdocument = this.Serialize(graph);
			htfdocument.Save(serializationStream);
		}

		private void SerializeArray(HTFElement parentElement, Array graph)
		{
			parentElement.Value = "Array";
			Type type = graph.GetType();
			int arrayRank = type.GetArrayRank();
			Type elementType = type.GetElementType();
			string value;
			string value2;
			this.Binder.BindToName(elementType, out value, out value2);
			parentElement.Children.Add(new HTFElement("ElementTypeAssembly", value));
			parentElement.Children.Add(new HTFElement("ElementTypeName", value2));
			HTFElement htfelement = new HTFElement();
			htfelement.ID = "Lengths";
			parentElement.Children.Add(htfelement);
			int[] array = new int[arrayRank];
			
			for (int i = 0; i < arrayRank; i++)
			{
				int length = graph.GetLength(i);
				array[i] = length;
				htfelement.Children.Add(new HTFElement(length.ToString()));
			}
			
			int[] indices = new int[arrayRank];
			HTFElement htfelement2 = new HTFElement();
			htfelement2.ID = "Values";
			parentElement.Children.Add(htfelement2);
			this.SerializeArrayRank(htfelement2, graph, array, indices, 0);
		}

		private void SerializeArrayRank(HTFElement element, Array graph, int[] lengths, int[] indices, int rank)
		{
			if (rank == lengths.Length - 1)
			{
				for (int i = 0; i < lengths[rank]; i++)
				{
					indices[rank] = i;
					HTFElement htfelement = new HTFElement();
					element.Children.Add(htfelement);
					this.Serialize(htfelement, graph.GetValue(indices));
				}
				
				return;
			}
			
			for (int j = 0; j < lengths[rank]; j++)
			{
				HTFElement htfelement2 = new HTFElement();
				element.Children.Add(htfelement2);
				indices[rank] = j;
				this.SerializeArrayRank(htfelement2, graph, lengths, indices, rank + 1);
			}
		}

		private void SerializeObject(HTFElement element, object graph)
		{
			element.Value = "Class";
			Type type = graph.GetType();
			this.SerializeObjectData(element, graph, type);
		}

		private void SerializeObjectData(HTFElement element, object graph, Type graphType)
		{
			string value;
			string value2;
			this.Binder.BindToName(graphType, out value, out value2);
			element.Children.Add(new HTFElement("TypeAssembly", value));
			element.Children.Add(new HTFElement("TypeName", value2));
			
			if (graph is ISerializable)
			{
				ISerializable serializable = (ISerializable)graph;
				SerializationInfo serializationInfo = new SerializationInfo(graphType, new HTFFormatter.FormatConverter());
				serializable.GetObjectData(serializationInfo, this.Context);
				SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
				HTFElement htfelement = new HTFElement("ClassData");
				element.Children.Add(htfelement);
				
				while (enumerator.MoveNext())
				{
					HTFElement htfelement2 = new HTFElement();
					htfelement2.ID = enumerator.Name;
					htfelement.Children.Add(htfelement2);
					this.Serialize(htfelement2, enumerator.Value);
				}
				
				return;
			}
			
			HTFElement htfelement3 = new HTFElement("Fields");
			Type type = graphType;
			
			do
			{
				HTFElement htfelement4 = new HTFElement(type.FullName);
				FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = false;
				
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0)
					{
						HTFElement htfelement5 = new HTFElement();
						htfelement5.ID = fieldInfo.Name;
						htfelement4.Children.Add(htfelement5);
						object value3 = fieldInfo.GetValue(graph);
						this.Serialize(htfelement5, value3);
						flag = true;
					}
				}
				
				if (flag)
				{
					htfelement3.Children.Add(htfelement4);
				}
				
				type = type.BaseType;
			}
			while (type != typeof(object));
			
			element.Children.Add(htfelement3);
		}

		public void Serialize(HTFElement element, object graph)
		{
			if (graph == null)
			{
				element.Value = "Null";
				return;
			}
			
			Type type = graph.GetType();
			string value;
			
			if (HTFFormatter.PrimitiveLookup.TryGetValue(type, out value))
			{
				element.Value = value;
				element.Children.Add(new HTFElement(graph.ToString()));
				return;
			}
			
			int num;
			
			if (!type.IsValueType && this._objectLookup.TryGetValue(graph, out num))
			{
				element.Value = "Reference";
				element.Children.Add(new HTFElement(num.ToString()));
				return;
			}
			
			if (!type.IsValueType)
			{
				this._objectLookup[graph] = this._nextID++;
			}
			
			if (type.IsArray)
			{
				this.SerializeArray(element, (Array)graph);
				return;
			}
			
			this.SerializeObject(element, graph);
		}

		public object Deserialize(HTFElement element)
		{
			this._nextID = 0;
			this._objectIDLookup = new Dictionary<int, object>();
			this._objectLookup = new Dictionary<object, int>();
			
			if (element.Children[0].ID != "DocType" || element.Children[0].Value != "HTFGraph")
			{
				throw new FileLoadException("Not a HTF Graph");
			}
			
			if (element.Children[1].ID != "Version")
			{
				throw new FileLoadException("Not a HTF Graph Format Error");
			}
			
			int num = int.Parse(element.Children[1].Value);
			
			if (num > HTFFormatter.Version)
			{
				throw new FileLoadException("This reader cannot read this version " + num.ToString());
			}
			
			return this.DeserializeFrom(element.Children[2]);
		}

		public object Deserialize(Stream serializationStream)
		{
			HTFDocument htfdocument = new HTFDocument();
			htfdocument.Load(serializationStream);
			return this.Deserialize(htfdocument.Root);
		}

		private object DeserializeFrom(HTFElement element)
		{
			string value;
			
			if ((value = element.Value) != null)
			{
				if (value == "Null")
				{
					return null;
				}
				
				if (value == "Reference")
				{
					return this.DeserializeReference(element);
				}
				
				if (value == "Class")
				{
					return this.DeserializeClass(element);
				}
				
				if (value == "Array")
				{
					return this.DeserializeArray(element);
				}
			}
			
			return HTFFormatter.DeserializePrimitive(element);
		}

		private static object DeserializePrimitive(HTFElement element)
		{
			HTFElement htfelement = element.Children[0];
			string value = element.Value;
			string value2 = element.Children[0].Value;
			string key;
			
			switch (key = value)
			{
				case "bool":
				{
					return bool.Parse(value2);
				}
				
				case "byte":
				{
					return byte.Parse(value2);
				}
				
				case "sbyte":
				{
					return sbyte.Parse(value2);
				}
				
				case "short":
				{
					return short.Parse(value2);
				}
				
				case "ushort":
				{
					return ushort.Parse(value2);
				}
				
				case "int":
				{
					return int.Parse(value2);
				}
				
				case "uint":
				{
					return uint.Parse(value2);
				}
				
				case "long":
				{
					return long.Parse(value2);
				}
				
				case "ulong":
				{
					return ulong.Parse(value2);
				}
				
				case "char":
				{
					return char.Parse(value2);
				}
				
				case "double":
				{
					return double.Parse(value2);
				}
				
				case "float":
				{
					return float.Parse(value2);
				}
				
				case "string":
				{
					return value2;
				}
			}

			throw new FileLoadException("Unknown Primitive Type: " + value);
		}

		private object DeserializeReference(HTFElement element)
		{
			string value = element.Children[0].Value;
			int key = int.Parse(value);
			return this._objectIDLookup[key];
		}

		private object DeserializeClass(HTFElement element)
		{
			string value = element.Children[0].Value;
			string value2 = element.Children[1].Value;
			
			Type type = this.Binder.BindToType(value, value2);
			
			object obj = null;
			
			if (type.ImplementsInterface(typeof(ISerializable)))
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
				SerializationInfo serializationInfo = new SerializationInfo(type, new HTFFormatter.FormatConverter());
				HTFElement htfelement = element.Children[2];
				
				int key = 0;
				
				if (!type.IsValueType)
				{
					key = this._nextID++;
				}
				
				foreach (HTFElement htfelement2 in htfelement.Children)
				{
					object obj2 = this.DeserializeFrom(htfelement2);
					serializationInfo.AddValue(htfelement2.ID, obj2, obj2.GetType());
				}
				
				obj = constructor.Invoke(new object[] { serializationInfo, this.Context });
				
				if (!type.IsValueType)
				{
					this._objectIDLookup[key] = obj;
				}
			}
			else
			{
				bool flag = false;
				
				try
				{
					obj = Activator.CreateInstance(type);
					flag = true;
				}
				catch
				{
					obj = FormatterServices.GetSafeUninitializedObject(type);
				}
				
				if (!type.IsValueType)
				{
					this._objectIDLookup[this._nextID++] = obj;
				}
				
				bool flag2 = this.DeserializeClassData(element, obj, type);
				
				if (flag2 && !flag)
				{
					throw new FileLoadException("Cannot Default Values to a class without a default constructor");
				}
			}
			
			if (obj is IDeserializationCallback)
			{
				IDeserializationCallback deserializationCallback = (IDeserializationCallback)obj;
				deserializationCallback.OnDeserialization(null);
			}
			
			return obj;
		}

		private bool DeserializeClassData(HTFElement element, object graph, Type graphType)
		{
			bool result = false;
			
			HTFElement htfelement = element.Children[2];
			
			if (htfelement.Value != "Fields")
			{
				throw new FileLoadException("HTF Graph Format Error");
			}
			
			Dictionary<string, List<FieldInfo>> fields = this.GetFields(graphType);
			
			foreach (HTFElement htfelement2 in htfelement.Children)
			{
				foreach (HTFElement htfelement3 in htfelement2.Children)
				{
					string id = htfelement3.ID;
					
					bool flag = false;
					
					List<FieldInfo> list;
					
					if (fields.TryGetValue(id, out list))
					{
						object obj = this.DeserializeFrom(htfelement3);
						
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].FieldType == obj.GetType())
							{
								list[i].SetValue(graph, obj);
								list.RemoveAt(i);
								flag = true;
								break;
							}
						}
					}

					if (!flag)
					{
						result = true;
					}
				}
			}

			return result;
		}

		private Dictionary<string, List<FieldInfo>> GetFields(Type graphType)
		{
			Dictionary<string, List<FieldInfo>> dictionary = new Dictionary<string, List<FieldInfo>>();
			Type type = graphType;
			
			do
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0)
					{
						List<FieldInfo> list;
						
						if (!dictionary.TryGetValue(fieldInfo.Name, out list))
						{
							list = new List<FieldInfo>();
							dictionary[fieldInfo.Name] = list;
						}
						
						list.Add(fieldInfo);
					}
				}
				
				type = type.BaseType;
			}
			while (type != typeof(object));
			
			return dictionary;
		}

		private Array DeserializeArray(HTFElement element)
		{
			string value = element.Children[0].Value;
			string value2 = element.Children[1].Value;
			Type elementType = this.Binder.BindToType(value, value2);
			HTFElement htfelement = element.Children[2];
			int count = htfelement.Children.Count;
			int[] array = new int[count];
			
			for (int i = 0; i < count; i++)
			{
				array[i] = int.Parse(htfelement.Children[i].Value);
			}
			
			HTFElement element2 = element.Children[3];
			Array array2 = Array.CreateInstance(elementType, array);
			this._objectIDLookup[this._nextID++] = array2;
			this.DeserializeArrayRank(element2, array2, array, new int[count], 0);
			return array2;
		}

		private void DeserializeArrayRank(HTFElement element, Array array, int[] lengths, int[] indicies, int rank)
		{
			if (rank == lengths.Length - 1)
			{
				for (int i = 0; i < lengths[rank]; i++)
				{
					HTFElement element2 = element.Children[i];
					indicies[rank] = i;
					object value = this.DeserializeFrom(element2);
					array.SetValue(value, indicies);
				}
				
				return;
			}
			
			for (int j = 0; j < lengths[rank]; j++)
			{
				HTFElement element3 = element.Children[j];
				indicies[rank] = j;
				this.DeserializeArrayRank(element3, array, lengths, indicies, rank + 1);
			}
		}

		public SerializationBinder Binder
		{
			get
			{
				return this._binder;
			}
			set
			{
				this._binder = value;
			}
		}

		public StreamingContext Context
		{
			get
			{
				return this._context;
			}
			set
			{
				this._context = value;
			}
		}

		public ISurrogateSelector SurrogateSelector
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}
	}
}
