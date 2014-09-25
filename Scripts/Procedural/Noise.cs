//using UnityEngine;

namespace M8 {
    public struct Noise {
        ///
        //

        /// <summary>
        /// generate a consistent noise of range [-1, 1]
        /// </summary>
        public static float Generate(int x) {
            x = (x<<13) ^ x;
            return (float)(1.0 - ((x*(x*x*15731 + 789221) + 1376312589)&0x7fffffff) / 1073741824.0);
        }

        public static float Generate(int x, int y, int width) {
            return Generate(Util.CellToIndex(y, x, width));
        }

        public static float Generate(int x, int y, int z, int width, int height) {
            return Generate(Util.CellToIndex(z, y, x, width, height));
        }

        public static float GenerateSmooth(int x) {
            return Generate(x)*0.5f + Generate(x-1)*0.25f + Generate(x+1)*0.25f;
        }

        public static float GenerateSmooth(int x, int y, int width) {
            float corners = (Generate(x-1, y-1, width)+Generate(x+1, y-1, width)+Generate(x-1, y+1, width)+Generate(x+1, y+1, width))*0.0625f; // 1/16
            float sides = (Generate(x-1, y, width)+Generate(x+1, y, width)+Generate(x, y-1, width)+Generate(x, y+1, width))*0.125f; // 1/8
            float center = Generate(x, y, width)*0.25f;
            return corners + sides + center;
        }

        public static float GenerateSmooth(int x, int y, int z, int width, int height) {
            float corners = (Generate(x-1, y-1, z-1, width, height)+Generate(x+1, y-1, z-1, width, height)+Generate(x-1, y+1, z-1, width, height)+Generate(x+1, y+1, z-1, width, height)+Generate(x-1, y-1, z+1, width, height)+Generate(x+1, y-1, z+1, width, height)+Generate(x-1, y+1, z+1, width, height)+Generate(x+1, y+1, z+1, width, height))/32.0f;
            float sides = (Generate(x-1, y, z-1, width, height)+Generate(x+1, y, z-1, width, height)+Generate(x, y-1, z-1, width, height)+Generate(x, y+1, z-1, width, height)+Generate(x-1, y, z+1, width, height)+Generate(x+1, y, z+1, width, height)+Generate(x, y-1, z+1, width, height)+Generate(x, y+1, z+1, width, height)+Generate(x-1, y-1, z, width, height)+Generate(x+1, y-1, z, width, height)+Generate(x+1, y+1, z, width, height)+Generate(x-1, y+1, z, width, height))/48.0f;
            float faces = (Generate(x-1, y, z, width, height)+Generate(x+1, y, z, width, height)+Generate(x, y+1, z, width, height)+Generate(x, y-1, z, width, height))/16.0f;
            float center = Generate(x, y, z, width, height)*0.25f;
            return corners + sides + faces + center;
        }

        /*public static float Perlin(int x, float persistence, int numOctave) {

        }*/
    }
}
