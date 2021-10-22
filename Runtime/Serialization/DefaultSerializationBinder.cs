using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DNA.Runtime.Serialization
{
	public class DefaultSerializationBinder : SerializationBinder
	{
		public Dictionary<Type, Type> TypeReplacements = new Dictionary<Type, Type>();
		public Dictionary<string, string> TypeNameReplacements = new Dictionary<string, string>();

		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = serializedType.Assembly.FullName;
			typeName = serializedType.FullName;
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			string text;
			
			if (this.TypeNameReplacements.TryGetValue(typeName, out text))
			{
				typeName = text;
			}
			
			Type type = null;
			string typeName2 = Assembly.CreateQualifiedName(assemblyName, typeName);
			
			try
			{
				type = Type.GetType(typeName2);
			}
			catch
			{
				type = Type.GetType(typeName);
			}
			
			Type type2;
			
			if (this.TypeReplacements.TryGetValue(type, out type2))
			{
				type = type2;
			}
			
			if (type == null)
			{
				throw new SerializationException(string.Concat(new string[]
				{
					"Type ",
					typeName,
					" ",
					assemblyName,
					" Not Found"
				}));
			}
			
			return type;
		}
	}
}
