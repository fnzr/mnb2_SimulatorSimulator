using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace BannerlordSimulator
{
    public static class Config
    {
        public static int BaseRoundTroopCount = 10;
        public static double NumericAdvantageFactor = 1;
        public static double TroopTierFactor = 1;
        public static readonly Normal[] TroopNormal;
        public static readonly double[] TroopPower;

        static Config()
        {
            TroopNormal = new Normal[]
            {
                new Normal(0, 4),
                new Normal(0, 3),
                new Normal(0, 2.5),
                new Normal(0, 2),
                new Normal(0, 1.5),
                new Normal(0, 1),
                new Normal(0, 0.6),
                new Normal(0, 0.4)
            };

            TroopPower = new double[]
            {
                1,
                1.7,
                2.5,
                3.2,
                3.9,
                4.6,
                5.3,
                6,
            };
        }

        public static int GetTroopsForNumericAdvantage(double ratio) =>
            (int)Math.Floor(NumericAdvantageFactor * 10 * ratio);
    }
}