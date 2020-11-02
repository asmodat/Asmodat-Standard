using AsmodatStandard.Extensions;
using System;

namespace AsmodatStandard.Types
{
    public static partial class Scale
    {
        /// <summary>
        /// https://github.com/polkadot-js/api/blob/044efbc6de4980dbabb5461c9d802c9c91634280/packages/types/src/primitive/Data.ts#L14
        /// </summary>
        public static byte[] DecodeData(ref string str)
        {
            if (str.IsNullOrEmpty())
                return null;

            var indicator = DecodeBytes(ref str, 1)[0];

            if(indicator == 0)
                return new byte[0];
            else if(indicator >= 1 && indicator <= 33)
                return DecodeBytes(ref str, indicator - 1);
            else if (indicator >= 34 && indicator <= 37)
                return DecodeBytes(ref str, 32);

            throw new Exception($"Scale.DecodeData => Invalid indicator value: {indicator}");
        }
    }
}