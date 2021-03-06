﻿using NUnit.Framework;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class IntExTest
    {
        [Test]
        public void IsPowerOf2()
        {
            Assert.IsTrue(2.IsPowerOf2());
            Assert.IsFalse(3.IsPowerOf2());
            Assert.IsTrue(4.IsPowerOf2());
            Assert.IsFalse(5.IsPowerOf2());
            Assert.IsFalse(6.IsPowerOf2());
            Assert.IsFalse(7.IsPowerOf2());
            Assert.IsTrue(8.IsPowerOf2());
        }

        [Test]
        public void GetBitShiftPositions()
        {
            void Test(int v, params int[] positions)
            {
                var arr = IntEx.GetBitShiftPositions(v);
                Assert.IsTrue(arr.ContainsAll<int>(positions), $"Failed V: {v}, Expected: {positions.JsonSerialize()}, Got: {arr.JsonSerialize()}");
            }

            Test(0, new int[0]);

            void Nest(int v, int level, int[] state)
            {
                for (int i = 0; i < 32; i++)
                {
                    var nextState = state.Merge(i);
                    Test(1 << i, nextState);

                    if(level <= 2) //31 is max
                        Nest((v | (1 << i)), level + 1, nextState);
                }
            }

            Nest(0, 0, new int[0]);
        }
    }
}
