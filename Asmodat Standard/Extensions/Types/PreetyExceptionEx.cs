using AsmodatStandard.Types;
using System;

namespace AsmodatStandard.Extensions.Types
{
    public static class PreetyExceptionEx
    {
        public static PreetyException ToPreetyException(this Exception ex, int maxDepth = 1000, int stackTraceMaxDepth = 1000)
            => new PreetyException(ex, maxDepth: maxDepth, stackTraceMaxDepth: stackTraceMaxDepth);
    }
}