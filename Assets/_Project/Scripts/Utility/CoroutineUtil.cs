using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Utility
{
    public static class CoroutineUtil
    {
        // source: https://stackoverflow.com/questions/59348515/call-parallel-coroutines-and-wait-for-all-of-them-to-be-over
        public static IEnumerator WaitForCoroutines(MonoBehaviour script, params IEnumerator[] iEnumerators)
        {
            if (iEnumerators != null & iEnumerators.Length > 0)
            {
                Coroutine[] coroutines = new Coroutine[iEnumerators.Length];
                for (int i = 0; i < iEnumerators.Length; i++)
                {
                    coroutines[i] = script.StartCoroutine(iEnumerators[i]);
                }
                for (int i = 0; i < coroutines.Length; i++)
                {
                    yield return coroutines[i];
                }
            }
            else 
            {
                yield return null;
            }
        }
    }
}
