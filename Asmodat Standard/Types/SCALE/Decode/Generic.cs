using AsmodatStandard.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace AsmodatStandard.Types
{
    public static partial class Scale
    {
        public static T Decode<T>(ref string str)
            => str == null ? throw new ArgumentException("Scale source object can't be null") : (T)DecodeObject(ref str, typeof(T));

        public static object DecodeObject(ref string str, Type type)
        {
            if (str.IsNullOrEmpty())
                return null;

            if (type.IsValueType || 
                type.IsEnum ||
                type.IsString() || 
                type.IsBigInteger() ||
                (type.IsGenericType && 
                    (type.GetGenericTypeDefinition() == typeof(ScaleOption<>) ||
                    type.GetGenericTypeDefinition() == typeof(ScaleEnum<>)
                )))
                return DecodeBasicType(ref str, type);
            else if (type.IsArray)
            {
                var l = DecodeCompactInteger(ref str).Value;

                if (str.Length < l)
                    throw new Exception($"Scale Failed to DecodeObject, decoded array size of {l} items, exceeds ecoded data size of {str.Length/2}B");

                var elementType = type.GetElementType();
                var array = Array.CreateInstance(elementType, (int)l);
                for (int i = 0; i < l; i++)
                    array.SetValue(DecodeObject(ref str, elementType), i);

                return Convert.ChangeType(array, type);
            }
            else if (type.IsClass)
            {
                var result = Activator.CreateInstance(type);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                    field.SetValue(result, DecodeObject(ref str, field.FieldType));

                return result;
            }
            else
                throw new NotImplementedException($"Scale can't process '{type?.FullName ?? "null"}'");
        }

        public static object DecodeBasicType(ref string str, Type type)
        {
            object obj = null;

            if (str.IsNullOrEmpty())
                return obj;

            if (type.IsString())
                obj = DecodeString(ref str);
            else if (type.IsByte())
                obj = NextByte(ref str);
            else if (type.IsBool())
                obj = DecodeBool(ref str);
            else if (type.IsUInt16())
                obj = DecodeUInt16(ref str);
            else if (type.IsUInt32())
                obj = DecodeUInt32(ref str);
            else if (type.IsUInt64())
                obj = DecodeUInt64(ref str);
            else if (type.IsBigInteger())
                obj = DecodeUInt128(ref str);
            else if (type.IsEnum)
                obj = DecodeEnum(ref str, type);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ScaleOption<>))
                obj = DecodeScaleOption(ref str, type);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ScaleEnum<>))
                obj = DecodeScaleEnum(ref str, type);
            else if (type == typeof(ScaleDeprecated))
                throw new Exception("Failed to decode, type was marked as depricieted and should not be used.");
            else
                throw new NotSupportedException($"Type '{type.FullName}' is a not supported yet by this Scale implementation.");

            return obj;
        }
    }
}