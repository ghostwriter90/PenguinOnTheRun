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
        /// Linear interpolation with borders
        /// </summary>
        /// <param name="input">The value to remap.</param>
        /// <param name="minOutput">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="maxOutput">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="minInput">the minimum bound of target interval [C,D]</param>
        /// <param name="maxInput">the maximum bound of target interval [C,D]</param>
        public static float LerpWithBorders(float input, float minOutput, float maxOutput, float minInput = 0, float maxInput = 1)
        {
            if (input < minInput) return minOutput;
            if (input > maxInput) return maxOutput;
            return minOutput+ (input - minInput) / (maxInput - minInput) * (maxOutput - minOutput);
        }

        /// <summary>
        /// Linear interpolation with borders
        /// </summary>
        /// <param name="input">The value to remap.</param>
        /// <param name="minOutput">the minimum bound of interval [A,B] that contains the x value</param>
        /// <param name="maxOutput">the maximum bound of interval [A,B] that contains the x value</param>
        /// <param name="minInput">the minimum bound of target interval [C,D]</param>
        /// <param name="maxInput">the maximum bound of target interval [C,D]</param>
        public static float LerpWithoutBorders(float input, float minOutput, float maxOutput, float minInput = 0, float maxInput = 1)
        {
            return minOutput + (input - minInput) / (maxInput - minInput) * (maxOutput - minOutput);
        }


        public static Vector2 LerpWithoutBorders(float input, Vector2 minOutput, Vector2 maxOutput, float minInput = 0, float maxInput = 1)
        {
            float outX = minOutput.x + (input - minInput) / (maxInput - minInput) * (maxOutput.x - minOutput.x);
            float outY = minOutput.y + (input - minInput) / (maxInput - minInput) * (maxOutput.y - minOutput.y);
            return new Vector2(outX, outY);
        }

        public static Vector2 LerpWithBorders(float input, Vector2 minOutput, Vector2 maxOutput, float minInput = 0, float maxInput = 1)
        {
            if (input < minInput) return minOutput;
            if (input > maxInput) return maxOutput;
            float outX = minOutput.x + (input - minInput) / (maxInput - minInput) * (maxOutput.x - minOutput.x);
            float outY = minOutput.y + (input - minInput) / (maxInput - minInput) * (maxOutput.y - minOutput.y);
            return new Vector2(outX, outY);
        }

        public static Vector3 LerpWithoutBorders(float input, Vector3 minOutput, Vector3 maxOutput, float minInput = 0, float maxInput = 1)
        {
            float outX = minOutput.x + (input - minInput) / (maxInput - minInput) * (maxOutput.x - minOutput.x);
            float outY = minOutput.y + (input - minInput) / (maxInput - minInput) * (maxOutput.y - minOutput.y);
            float outZ = minOutput.z + (input - minInput) / (maxInput - minInput) * (maxOutput.z - minOutput.z);
            return new Vector3(outX, outY, outZ);
        }

        public static Vector3 LerpWithBorders(float input, Vector3 minOutput, Vector3 maxOutput, float minInput = 0, float maxInput = 1)
        {
            if (input < minInput) return minOutput;
            if (input > maxInput) return maxOutput;
            float outX = minOutput.x + (input - minInput) / (maxInput - minInput) * (maxOutput.x - minOutput.x);
            float outY = minOutput.y + (input - minInput) / (maxInput - minInput) * (maxOutput.y - minOutput.y);
            float outZ = minOutput.z + (input - minInput) / (maxInput - minInput) * (maxOutput.z - minOutput.z);
            return new Vector3(outX, outY, outZ);
        }


    }
}
