using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AsmodatStandard.Extensions
{
    public static class ReflectionHelper
    {
        public static object GetUninitializedInstance(this object obj) => FormatterServices.GetUninitializedObject(obj.GetType());
        public static T GetUninitializedInstance<T>() => (T)FormatterServices.GetUninitializedObject(typeof(T));

        public static T DeepCopy<T>(this T source)
            => source == null ? throw new ArgumentException("source can't be null") : (T)DeepCopyObject(source);

        public static object DeepCopyObject(object obj)
        {
            if (obj == null)
                return null;
            
            var type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
                return obj;
            else if (type.IsArray)
            {
                var array = obj as Array;
                var copied = Array.CreateInstance(type.GetElementType(), array.Length);
                for (int i = 0; i < array.Length; i++)
                    copied.SetValue(DeepCopyObject(array.GetValue(i)), i);

                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {
                var result = obj.GetUninitializedInstance();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                    field.SetValue(result, DeepCopyObject(field.GetValue(obj)));

                return result;
            }
            else
                throw new NotImplementedException($"can't process '{nameof(type)}'");
        }

        public static bool DeepEquals<T>(this T s1, T s2) => DeepEqualsObject(s1, s2);

        private static bool DeepEqualsObject(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;
            else if (obj1 == null || obj2 == null)
                return false;

            var type1 = obj1.GetType();
            var type2 = obj2.GetType();

            if (type1.FullName != type2.FullName)
                return false;

            if (type1.IsValueType || type1 == typeof(string))
                return obj1.Equals(obj2);
            else if (type1.IsArray)
            {
                var array1 = obj1 as Array;
                var array2 = obj2 as Array;

                if (array1.Length != array2.Length)
                    return false;

                for (int i = 0; i < array1.Length; i++)
                    if (!DeepEqualsObject(array1.GetValue(i), array2.GetValue(i)))
                        return false;

                return true;
            }
            else if (type1.IsClass)
            {
                var fields = type1.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                    if (!DeepEqualsObject(field.GetValue(obj1), field.GetValue(obj2)))
                        return false;

                return true;
            }
            else
                throw new NotImplementedException($"can't process '{nameof(type1)}'");
        }
    }
}
