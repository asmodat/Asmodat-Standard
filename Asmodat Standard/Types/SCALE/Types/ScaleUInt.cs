using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AsmodatStandard.Types
{
    struct ScaleUInt128
    {
        private BigInteger value;

        public static implicit operator ScaleUInt128(BigInteger i)
        {
            return new ScaleUInt128 { value = i };
        }

        public static implicit operator BigInteger(ScaleUInt128 p)
        {
            return p.value;
        }
    }

    struct ScaleUInt256
    {
        private BigInteger value;

        public static implicit operator ScaleUInt256(BigInteger i)
        {
            return new ScaleUInt256 { value = i };
        }

        public static implicit operator BigInteger(ScaleUInt256 p)
        {
            return p.value;
        }
    }
}
