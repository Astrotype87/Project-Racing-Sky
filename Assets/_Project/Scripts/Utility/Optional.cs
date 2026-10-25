using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Utility
{
    [Serializable]
    public struct Optional<T>
    {
        public T value;
        public bool enabled;
        
        public static implicit operator T(Optional<T> value) => value.value;
    }
}
