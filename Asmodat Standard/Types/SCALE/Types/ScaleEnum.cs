using System;
using System.Linq;

namespace AsmodatStandard.Types
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScaleEnumTypeAttribute : Attribute
    {
        public ScaleEnumTypeAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; set; }
    }

    public class ScaleEnum<E> where E : Enum
    {
        public ScaleEnum(E @enum, object o)
        {
            this.Enum = @enum;
            this.Value = o;

            var attributes = (ScaleEnumTypeAttribute[])this.Enum
               .GetType()
               .GetField(this.Enum.ToString())
               .GetCustomAttributes(typeof(ScaleEnumTypeAttribute), false);

            if (attributes.Length != 1)
                throw new Exception($"Enum {@enum} requires a single attribute of type ScaleEnumAttribute.");

            this.Type = attributes.Single().Type;

            if (this.Type == null)
                throw new Exception("Type of the scale enum was not defined (null)");
        }

        public E Enum { get; set; }

        public Type Type { get; set; }

        public object Value { get; set; }
    }
}
