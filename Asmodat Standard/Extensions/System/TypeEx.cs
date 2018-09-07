using System;
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
    }
}
