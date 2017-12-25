namespace AsmodatStandard.Extensions
{
    public static class NullableEx
    {
        public static T? ToNullable<T>(this T value) where T : struct => (T?)value;
    }
}
