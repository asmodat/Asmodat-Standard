using AsmodatStandard.Types;

namespace AsmodatStandard.Extensions.Types
{
    public static class TickTimeoutEx
    {
        public static TickTimeout StartNew(int timeout)
        {
            var tt = new TickTimeout(timeout, TickTime.Unit.ms);
            tt.Reset();
            return tt;
        }

        public static TickTimeout Copy(this TickTimeout timeout)
        {
            if (timeout == null)
                return null;

            return (TickTimeout)timeout.Clone();
        }

        public static bool IsEnabled(this TickTimeout timeout)
        {
            if (timeout == null || !timeout.Enabled)
                return false;

            return true;
        }
    }
}