using System;

namespace BarRaider.SdTools.Utilities
{
    /// <summary>
    /// Helper class for generating random numbers
    /// </summary>
    public static class RandomGenerator
    {
        private static readonly Random Random = new();

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range. Value will be less than the specified maximum.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }
    }
}
