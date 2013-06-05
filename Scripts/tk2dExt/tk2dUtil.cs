using UnityEngine;
using System.Collections;

namespace M8 {
    public struct tk2dUtil {
        public static int[] GenerateSpriteIds(tk2dSpriteAnimation anim, System.Type enumType, int defaultId = -1) {
            string[] names = System.Enum.GetNames(enumType);

            int[] ret = new int[names.Length];

            for(int i = 0; i < ret.Length; i++) {
                int id = anim.GetClipIdByName(names[i]);
                ret[i] = id == -1 ? defaultId : id;
            }

            return ret;
        }

        public static int[] GenerateSpriteIds(tk2dAnimatedSprite anim, System.Type enumType, int defaultId = -1) {
            if(anim.anim == null)
                return null;

            return GenerateSpriteIds(anim.anim, enumType, defaultId);
        }
    }
}