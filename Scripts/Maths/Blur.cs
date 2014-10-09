using UnityEngine;

namespace M8 {
    public struct Blur {
        
        public static float[,] GaussianApprox(float[,] src, float radius) {
            int w = src.GetLength(0), h = src.GetLength(1);

            float[] _src = new float[src.Length];
            for(int y = 0; y < h; y++)
                for(int x = 0; x < w; x++)
                    _src[M8.Util.CellToIndex(y, x, w)] = src[x, y];

            float[] _dst = new float[src.Length];
                        
            GaussianApprox(_src, _dst, w, h, radius);

            float[,] ret = new float[w, h];
            for(int y = 0; y < h; y++)
                for(int x = 0; x < w; x++)
                    ret[x, y] = _dst[M8.Util.CellToIndex(y, x, w)];

            return ret;
        }

        public static void GaussianApprox(float[] src, float[] dst, int width, int height, float radius) {
            int[] boxes = GaussBoxes(radius, 3);
            GaussBoxBlur(src, dst, width, height, (boxes[0] - 1)/2);
            GaussBoxBlur(dst, src, width, height, (boxes[1] - 1)/2);
            GaussBoxBlur(src, dst, width, height, (boxes[2] - 1)/2);
        }

        //Box blur for gaussian, ideally you want 3
        static int[] GaussBoxes(float sigma, int count) {
            float n = count;

            //ideal avg. width filter
            float wIdeal = Mathf.Sqrt((12f*sigma*sigma/n)+1);
            
            int wl = Mathf.FloorToInt(wIdeal);
            if(wl%2 == 0)
                wl--;

            int wu = wl+2;

            float mIdeal = (12f*sigma*sigma - n*wl*wl - 4f*n*wl - 3f*n)/(-4f*wl - 4f);
            
            int m = Mathf.RoundToInt(mIdeal);

            int[] ret = new int[count];
            for(int i = 0; i < count; i++)
                ret[i] = i < m ? wl : wu;

            return ret;
        }

        static void GaussBoxBlur(float[] src, float[] dst, int width, int height, int radius) {
            src.CopyTo(dst, 0);
            GaussBoxH(dst, src, width, height, radius);
            GaussBoxW(src, dst, width, height, radius);
        }

        static void GaussBoxH(float[] src, float[] dst, int width, int height, int radius) {
            float iarr = 1f/(radius+radius+1);
            for(int i = 0; i < height; i++) {
                int ti = i*width, li = ti, ri = ti+radius;
                float fv = src[ti], lv = src[ti+width-1], val = (radius+1)*fv;
                for(int j = 0; j < radius; j++) val += src[ti+j];
                for(int j = 0; j <= radius; j++) { val += src[ri++] - fv; dst[ti++] = Mathf.Round(val*iarr); }
                for(int j = radius+1; j < width - radius; j++) { val += src[ri++] - src[li++]; dst[ti++] = Mathf.Round(val*iarr); }
                for(int j = width-radius; j < width; j++) { val += lv - src[li++]; dst[ti] = Mathf.Round(val*iarr); }
            }
        }

        static void GaussBoxW(float[] src, float[] dst, int width, int height, int radius) {
            float iarr = 1f/(radius+radius+1);
            for(int i = 0; i < width; i++) {
                int ti = i, li = ti, ri = ti+radius*width;
                float fv = src[ti], lv = src[ti+width*(height-1)], val = (radius+1)*fv;
                for(int j = 0; j < radius; j++) val += src[ti+j*width];
                for(int j = 0; j <= radius; j++) { val += src[ri] - fv; dst[ti] = Mathf.Round(val*iarr); ri += width; ti += width; }
                for(int j = radius+1; j < height-radius; j++) { val += src[ri] - src[li]; dst[ti] = Mathf.Round(val*iarr); li += width; ri += width; ti += width; }
                for(int j = height-radius; j < height; j++) { val += lv - src[li]; dst[ti] = Mathf.Round(val*iarr); li += width; ti += width; }
            }
        }
    }
}