namespace AsmodatStandard.Types
{
    public class ScaleOption<T>
    {
        public ScaleOption(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.Value = value;
        }

        public bool HasValue { get; set; }

        public T Value { get; set; }
    }
}
