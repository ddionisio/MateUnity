using UnityEngine;
using System.Collections;

namespace M8 {
    public struct tk2dUtil {
        public static tk2dSpriteAnimationClip[] GetSpriteClips(tk2dSpriteAnimation anim, System.Type enumType, int defaultId = -1) {
            string[] names = System.Enum.GetNames(enumType);

            tk2dSpriteAnimationClip[] ret = new tk2dSpriteAnimationClip[names.Length];

            for(int i = 0; i < ret.Length; i++) {
                tk2dSpriteAnimationClip clip = anim.GetClipByName(names[i]);
                ret[i] = clip == null ? anim.GetClipById(defaultId) : clip;
            }

            return ret;
        }

        public static tk2dSpriteAnimationClip[] GetSpriteClips(tk2dSpriteAnimator animator, System.Type enumType, int defaultId = -1) {
            if(animator.Library == null)
                return null;

            return GetSpriteClips(animator.Library, enumType, defaultId);
        }
    }
}