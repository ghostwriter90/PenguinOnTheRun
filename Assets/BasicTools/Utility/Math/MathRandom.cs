using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BasicTools.Utility
{
    public static partial class MathPlus
    {

        /// <summary>
        /// Returns a random success based on X% of chance.
        /// Example : I have 20% of chance to do X, Chance(20) > true, yay!
        /// </summary>
        /// <param name="percent">Percent of chance.</param>
        public static bool Chance(int percent)
        {
            return (UnityEngine.Random.Range(0, 100) < percent);
        }

        /// <summary>
        /// Returns a random success based on X% of chance.
        /// Example : I have 20% of chance to do X, Chance(0.2f) > true, yay!
        /// </summary>
        /// <param name="percent">Percent of chance.</param>
        public static bool Chance(float rate)
        {
            return (UnityEngine.Random.Range(0f, 1f) < rate);
        }

        /// <summary>
        /// Returns the result of rolling a dice of the specified number of sides
        /// </summary>
        /// <returns>The result of the dice roll.</returns>
        /// <param name="numberOfSides">Number of sides of the dice.</param>
        public static int RollADice(int numberOfSides)
        {
            return (UnityEngine.Random.Range(1, numberOfSides+1));
        }


        /// <summary>
        /// Returns 1 random vector between 2 defined vector
        /// </summary>
        public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
        {
            return new Vector3(
                UnityEngine.Random.Range(minimum.x, maximum.x),
                UnityEngine.Random.Range(minimum.y, maximum.y),
                UnityEngine.Random.Range(minimum.z, maximum.z));
        }

        /// <summary>
        /// Returns 1 random vector between 2 defined vector
        /// </summary>
        public static Vector2 RandomVector2(Vector2 minimum, Vector2 maximum)
        {
            return new Vector2(
                UnityEngine.Random.Range(minimum.x, maximum.x),
                UnityEngine.Random.Range(minimum.y, maximum.y));
        }

        public static Vector2 Clamp01(Vector2 areaPos)
        {
            if (areaPos.x < 0) { areaPos.x = 0; }
            if (areaPos.y < 0) { areaPos.y = 0; }
            if (areaPos.x > 1) { areaPos.x = 1; }
            if (areaPos.y > 1) { areaPos.y = 1; }
            return areaPos;
        }
    }
}
