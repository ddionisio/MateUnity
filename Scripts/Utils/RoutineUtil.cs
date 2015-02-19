using UnityEngine;
using System.Collections;

//general helper functions

namespace M8 {
    public struct RoutineUtil {
        /// <summary>
        /// Allow for a routine to be yielded within another Coroutine without calling StartCoroutine
        /// e.g. yield return RoutineUtil.Sub(DoSomething());
        /// </summary>
        public static IEnumerator Sub(IEnumerator routine) {
            while(true) {
                if(!routine.MoveNext())
                    yield break;

                yield return routine.Current;
            }
        }

        /// <summary>
        /// Allow for sequential routines in one fell swoop.
        /// e.g. yield return RoutineUtil.Sequence(DoSomething(), DoAnotherThing());
        /// </summary>
        public static IEnumerator Sequence(params IEnumerator[] routines) {
            for(int i = 0; i < routines.Length; i++)
                yield return Sub(routines[i]);
        }
    }
}