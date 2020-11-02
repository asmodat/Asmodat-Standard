using System;
using System.Numerics;
using System.Reflection;

namespace AsmodatStandard.Extensions
{
    public static class TypeEx
    {
        public static bool IsSimple(TypeInfo type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //in case of nullable type, check if the nested type is simple.
                return IsSimple((type.GetGenericArguments()[0]).GetTypeInfo());
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string).GetTypeInfo())
              || type.Equals(typeof(decimal).GetTypeInfo());
        }

        public static bool IsString(this Type type) => type == typeof(string);
        public static bool IsBool(this Type type) => type == typeof(bool);
        public static bool IsByte(this Type type) => type == typeof(byte);
        public static bool IsUInt16(this Type type) => type == typeof(UInt16);
        public static bool IsUInt32(this Type type) => type == typeof(UInt32);
        public static bool IsUInt64(this Type type) => type == typeof(UInt64);
        public static bool IsBigInteger(this Type type) => type == typeof(BigInteger);

        public static object GetDefaultObject(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
