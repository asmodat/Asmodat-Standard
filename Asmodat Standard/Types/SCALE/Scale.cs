﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace AsmodatStandard.Types
{
    public struct CompactInteger
    {
        public static CompactInteger operator +(CompactInteger self, BigInteger value)
        {
            self.Value = self.Value + value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, BigInteger value)
        {
            self.Value = self.Value * value;
            return self;
        }

        public static CompactInteger operator +(CompactInteger self, long value)
        {
            self.Value += value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, long value)
        {
            self.Value *= value;
            return self;
        }

        public static CompactInteger operator +(CompactInteger self, int value)
        {
            self.Value += value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, int value)
        {
            self.Value *= value;
            return self;
        }

        public static CompactInteger operator +(CompactInteger self, uint value)
        {
            self.Value += value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, uint value)
        {
            self.Value *= value;
            return self;
        }

        public static CompactInteger operator +(CompactInteger self, byte value)
        {
            self.Value += value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, byte value)
        {
            self.Value *= value;
            return self;
        }

        public static CompactInteger operator +(CompactInteger self, CompactInteger value)
        {
            self.Value += value.Value;
            return self;
        }

        public static CompactInteger operator *(CompactInteger self, CompactInteger value)
        {
            self.Value *= value.Value;
            return self;
        }

        public BigInteger Value { get; set; }
    }

    /// <summary>
    /// https://substrate.dev/docs/en/conceptual/core/codec
    /// Js Implementation https://github.com/polkadot-js/api/tree/85ae6f949b45af9cea5eb819c2a1776bb4aea9e1/packages/types/src/codec
    /// </summary>
    public static partial class Scale
    {
        private static Exception CompactIntegerException(string v)
        {
            throw new InvalidCastException(v);
        }

        public static byte NextByte(ref string stringStream)
        {
            var bt = byte.Parse(stringStream.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            stringStream = stringStream.Substring(2);
            return bt;
        }

        public static ushort NextWord(ref string stringStream)
        {
            var minor = stringStream.Substring(0, 2);
            stringStream = stringStream.Substring(2);
            var major = stringStream.Substring(0, 2);
            stringStream = stringStream.Substring(2);

            return ushort.Parse(major + minor, System.Globalization.NumberStyles.HexNumber);
        }

        
        public static CompactInteger DecodeCompactInteger(ref string stringStream)
        {
            uint first_byte = NextByte(ref stringStream);
            uint flag = (first_byte) & 0b00000011u;
            uint number = 0u;

            switch (flag)
            {
                case 0b00u:
                    {
                        number = first_byte >> 2;
                        break;
                    }

                case 0b01u:
                    {
                        uint second_byte = NextByte(ref stringStream);

                        number = ((uint)((first_byte) & 0b11111100u) + (uint)(second_byte) * 256u) >> 2;
                        break;
                    }

                case 0b10u:
                    {
                        number = first_byte;
                        uint multiplier = 256u;

                        for (var i = 0u; i < 3u; ++i)
                        {
                            number += NextByte(ref stringStream) * multiplier;
                            multiplier = multiplier << 8;
                        }
                        number = number >> 2;
                        break;
                    }

                case 0b11:
                    {
                        uint bytes_count = ((first_byte) >> 2) + 4u;
                        CompactInteger multiplier = new CompactInteger { Value = 1u };
                        CompactInteger value = new CompactInteger { Value = 0 };

                        // we assured that there are m more bytes,
                        // no need to make checks in a loop
                        for (var i = 0u; i < bytes_count; ++i)
                        {
                            value += multiplier * NextByte(ref stringStream);
                            multiplier *= 256u;
                        }

                        return value;
                    }

                default:
                    throw CompactIntegerException("CompactInteger decode error: unknown flag");
            }

            return new CompactInteger { Value = number };
        }

        public struct CompactIntegerLEBytes
        {
            public long Length { get; set; }
            public byte[] Bytes { get; set; }
        }

        public static CompactIntegerLEBytes EncodeCompactInteger(BigInteger n)
        {
            CompactIntegerLEBytes b = new CompactIntegerLEBytes();
            b.Bytes = new byte[64];

            if (n <= 63)
            {
                b.Length = 1;
                b.Bytes[0] = (byte)(n << 2);
            }
            else if (n <= 0x3FFF)
            {
                b.Length = 2;
                b.Bytes[0] = (byte)(((n & 0x3F) << 2) | 0x01);
                b.Bytes[1] = (byte)((n & 0xFC0) >> 6);
            }
            else if (n <= 0x3FFFFFFF)
            {
                b.Length = 4;
                b.Bytes[0] = (byte)(((n & 0x3F) << 2) | 0x02);
                n >>= 6;
                for (int i = 1; i < 4; ++i)
                {
                    b.Bytes[i] = (byte)(n & 0xFF);
                    n >>= 8;
                }
            }
            else
            { // Big integer mode
                b.Length = 1;
                int byteNum = 1;
                while (n > 0)
                {
                    b.Bytes[byteNum++] = (byte)(n & 0xFF);
                    n >>= 8;
                }
                b.Length = byteNum;
                b.Bytes[0] = (byte)(((byteNum - 5) << 2) | 0x03);
            }

            b.Bytes = b.Bytes.AsMemory().Slice(0, (int)b.Length).ToArray();

            return b;
        }

        public static long WriteCompactToBuf(CompactIntegerLEBytes ci, ref byte[] buf, long offset)
        {
            for (int i = 0; i < ci.Length; ++i)
                buf[offset + i] = ci.Bytes[i];
            return ci.Length;
        }
    }
}