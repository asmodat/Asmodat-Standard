using NUnit.Framework;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class EnumExTest
    {
        enum EnumAlphabet
        {
            None = 0,
            A = 1,
            B = 1 << 1,
            C = 1 << 2,
            D = 1 << 3,
            E = 1 << 4,
            F = 1 << 5,
            G = 1 << 6,
            H = 1 << 7,
            I = 1 << 8,
            J = 1 << 9,
            K = 1 << 10,
            L = 1 << 11,
            M = 1 << 12,
            N = 1 << 13,
            O = 1 << 14,
            P = 1 << 15,
            R = 1 << 16,
            S = 1 << 17,
            T = 1 << 18,
            U = 1 << 19,
            W = 1 << 20,
            X = 1 << 21,
            Y = 1 << 22,
            Z = 1 << 23,
            AF = A | B | C | D | E | F,
            GL = G | H | I | J | K | L,
            MS = M | N | O | P | R | S,
            TZ = T | U | W | X | Y | Z,
            All = AF | GL | MS | TZ
        }

        [Test]
        public void ToStringFlagArrayTest()
        {
            var expected = new string[]
            {
               "A",  "B",  "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "W", "X", "Y", "Z",
            };
            var result = EnumEx.ToStringFlagArray(EnumAlphabet.All);

            CollectionAssert.AreEqual(expected, result);

            CollectionAssert.AreEqual(
                new string[0], 
                EnumEx.ToStringFlagArray(EnumAlphabet.None));

            CollectionAssert.AreEqual(
                new string[]
            {
               "A",  "B",  "C", "D", "E", "F",
            },
                EnumEx.ToStringFlagArray(EnumAlphabet.AF));
        }
    }
}
