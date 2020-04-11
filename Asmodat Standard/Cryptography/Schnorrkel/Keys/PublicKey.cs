using System;
using Schnorrkel.Ristretto;

namespace Schnorrkel
{
    public class PublicKey
    {
        public byte[] Key { get; set; }

        public PublicKey(byte[] keyBytes)
        {
            Key = keyBytes;
        }

        public EdwardsPoint GetEdwardsPoint()
        {
            return EdwardsPoint.Decompress(Key);
        }
    }
}
