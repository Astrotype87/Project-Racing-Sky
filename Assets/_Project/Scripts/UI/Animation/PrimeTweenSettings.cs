using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

namespace ProjectRacingSky.UI
{
    public class PrimeTweenSettings : MonoBehaviour
    {
        [Header("PrimeTween Config")]
        [SerializeField] private Ease defaultEase;
        [SerializeField] private bool disableValidateCustomCurves;
        [SerializeField] private bool disableWarnBenchmarkWithAsserts;
        [SerializeField] private bool disableWarnEndValueEqualsCurrent;
        [SerializeField] private bool disableWarnStructBoxingAllocationInCoroutine;
        [SerializeField] private bool disableWarnTweenOnDisabledTarget;
        [SerializeField] private bool disableWarnZeroDuration;
        
        private void Awake()
        {
            PrimeTweenConfig.defaultEase = defaultEase;
            PrimeTweenConfig.validateCustomCurves = !disableValidateCustomCurves;
            PrimeTweenConfig.warnBenchmarkWithAsserts = !disableWarnBenchmarkWithAsserts;
            PrimeTweenConfig.warnEndValueEqualsCurrent = !disableWarnEndValueEqualsCurrent;
            PrimeTweenConfig.warnStructBoxingAllocationInCoroutine = !disableWarnStructBoxingAllocationInCoroutine;
            PrimeTweenConfig.warnTweenOnDisabledTarget = !disableWarnTweenOnDisabledTarget;
            PrimeTweenConfig.warnZeroDuration = !disableWarnZeroDuration;
        }
    }
}
