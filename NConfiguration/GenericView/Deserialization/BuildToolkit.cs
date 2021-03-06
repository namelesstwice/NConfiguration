﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Reflection;

namespace NConfiguration.GenericView.Deserialization
{
	public static class BuildToolkit
	{
		internal static object CreatePrimitiveFunction(Type type)
		{
			var funcType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode), type);
			return Delegate.CreateDelegate(funcType, PrimitiveTargetMI.MakeGenericMethod(type));
		}

		private static readonly Dictionary<Type, AttributeState> DataContractAttributeStates = new Dictionary<Type, AttributeState>
		{
			{typeof(DataContractAttribute), AttributeState.Found}, // not used
			{typeof(DataMemberAttribute), AttributeState.Found}, // change name, set required
			{typeof(IgnoreDataMemberAttribute), AttributeState.Found}, // ignore
			{typeof(CollectionDataContractAttribute), AttributeState.NotImplemented} // not implemented
		};

		internal static AttributeState DataContractAvailable(Type targetType)
		{
			return CustomAttributesAvailable(targetType, DataContractAttributeStates);
		}

		private static readonly Dictionary<Type, AttributeState> XmlAttributeStates = new Dictionary<Type, AttributeState>
		{
			{typeof(XmlRootAttribute), AttributeState.Found}, // not used
			{typeof(XmlAttributeAttribute), AttributeState.Found}, // change name
			{typeof(XmlElementAttribute), AttributeState.Found}, // change name
			{typeof(XmlArrayAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlArrayItemAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlTextAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlAnyElementAttribute), AttributeState.NotImplemented}, // not implemented
			{typeof(XmlIgnoreAttribute), AttributeState.Found}, // ignore
		};

		public static AttributeState XmlAvailable(Type targetType)
		{
			return CustomAttributesAvailable(targetType, XmlAttributeStates);
		}

		private static AttributeState CustomAttributesAvailable(Type targetType, Dictionary<Type, AttributeState> attrStates)
		{
			AttributeState result = AttributeState.NotFound;
			CheckAttributes(targetType.GetCustomAttributes(true), ref result, attrStates);
			foreach (var mi in targetType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, (m, o) => true, null))
				CheckAttributes(mi.GetCustomAttributes(true), ref result, attrStates);
			return result;
		}

		private static void CheckAttributes(object[] attrs, ref AttributeState result, Dictionary<Type, AttributeState> attrStates)
		{
			if (result == AttributeState.NotImplemented)
				return;

			foreach (object attr in attrs)
			{
				AttributeState state;
				if (!attrStates.TryGetValue(attr.GetType(), out state))
					continue;

				if (state == AttributeState.NotImplemented)
				{
					result = AttributeState.NotImplemented;
					return;
				}

				result = AttributeState.Found;
			}
		}

		internal static readonly MethodInfo PrimitiveTargetMI = typeof(BuildToolkit).GetMethod("PrimitiveTarget", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T PrimitiveTarget<T>(ICfgNode node)
		{
			return node.As<T>();
		}

		internal static readonly MethodInfo OptionalPrimitiveFieldMI = typeof(BuildToolkit).GetMethod("OptionalPrimitiveField", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T OptionalPrimitiveField<T>(string name, ICfgNode node)
		{
			try
			{
				var field = node.GetChild(name);
				if (field == null)
					return default(T);
			
				return field.As<T>();
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo RequiredPrimitiveFieldMI = typeof(BuildToolkit).GetMethod("RequiredPrimitiveField", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T RequiredPrimitiveField<T>(string name, ICfgNode node)
		{
			try
			{
				var field = node.GetChild(name);
				if (field == null)
					throw new FormatException("field not found");

				return field.As<T>();
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo RequiredComplexFieldMI = typeof(BuildToolkit).GetMethod("RequiredComplexField", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T RequiredComplexField<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			try
			{
				var field = node.GetChild(name);
				if (field == null)
					throw new FormatException("field not found");

				return deserializer.Deserialize<T>(field);
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo OptionalComplexFieldMI = typeof(BuildToolkit).GetMethod("OptionalComplexField", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T OptionalComplexField<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			try
			{
				var field = node.GetChild(name);
				if (field == null)
					return default(T);

				return deserializer.Deserialize<T>(field);
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo ListMI = typeof(BuildToolkit).GetMethod("List", BindingFlags.Static | BindingFlags.NonPublic);

		internal static List<T> List<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			int i = 0;
			try
			{
				List<T> result = new List<T>();
				foreach (var chItem in node.GetCollection(name))
				{
					result.Add(deserializer.Deserialize<T>(chItem));
					i++;
				}

				return result;
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(string.Format("{0}[{1}]", name, i), ex);
			}
		}

		internal static readonly MethodInfo ArrayMI = typeof(BuildToolkit).GetMethod("Array", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T[] Array<T>(string name, ICfgNode node, IGenericDeserializer deserializer)
		{
			return List<T>(name, node, deserializer).ToArray();
		}

		internal static object CreateNativeFunction()
		{
			var funcType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode), typeof(ICfgNode));
			return Delegate.CreateDelegate(funcType, GetCfgNodeMI);
		}

		internal static readonly MethodInfo GetCfgNodeMI = typeof(BuildToolkit).GetMethod("GetCfgNode", BindingFlags.Static | BindingFlags.NonPublic);

		internal static ICfgNode GetCfgNode(ICfgNode node)
		{
			return node;
		}
	}
}
