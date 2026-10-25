using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectRacingSky.Camera
{
    using Utility;
    
    // COMPONENT CONFIGURATIONS
    
    /// <summary> Group of CameraView settings containing close, far, top, front, cockpit and rear. </summary>
    [Serializable] public struct ViewConfig
    {
        public CameraView closeView;
        public CameraView farView;
        public CameraView topView;
        public CameraView frontView;
        public CameraView cockpitView;
        public CameraView rearView;
    }
    
    /// <summary> Damping effects configuration for CameraDamping component. </summary>
    [Serializable] public struct DampingConfig
    {
        public FollowDamping followDamping;
        public OrbitDamping orbitDamping;
    }
    
    /// <summary> Dynamic offset effects configuration for CameraOffset component. </summary>
    [Serializable] public struct OffsetConfig
    {
        public OffsetLayer thrustOffset;
        public OffsetLayer afterburnerOffset;
        public OffsetLayer airbrakeOffset;
        public OffsetLayer steerOffset;
        public OffsetLayer strafeOffset;
        public OffsetLayer abTorqueOffset;
        public OffsetLayer boostOffset;
        public OffsetLayer speedPadOffset;
    }
    
    /// <summary> Dynamic FOV effects configuration for CameraFOV component. </summary>
    [Serializable] public struct FOVConfig
    {
        public FOVLayer speedFOV;
        public FOVLayer thrustFOV;
        public FOVLayer afterburnerFOV;
        public FOVLayer brakeFOV;
        public FOVLayer boostFOV;
        public FOVLayer speedPadFOV;
    }
    
    /// <summary> Camera shake effects configuration for CameraShake component. </summary>
    [Serializable] public struct ShakeConfig
    {
        public ShakeConstant idleShake;
        public ShakeConstant speedShake;
        public ShakeImpulse jumpShake;
    }
    
    
    // MAIN CONFIGURATIONS
    
    /// <summary> Fixed list of available camera view modes in the race (excluding rear view). </summary>
    public enum ViewMode {Close, Far, Top, Front, Cockpit}
    
    /// <summary> Camera view settings containing offset, rotation, pivot distance, FOV, follow model, and modifiers. </summary>
    [Serializable] public struct CameraView
    {
        public Vector3 offset;
        public Vector3 rotation;
        public Vector3 pivot;
        [Range(1e-05f, 179)] public float baseFOV;
        public bool followModel;
        public bool lookAtPivot;
        public ViewModifiers modifiers;
        
        public void SetCameraView(Vector3 offset, Vector3 rotation, Vector3 pivot,
            float baseFOV, bool followModel, bool lookAtPivot, ViewModifiers modifiers)
        {
            this.offset = offset;
            this.rotation = rotation;
            this.pivot = pivot;
            this.baseFOV = baseFOV;
            this.followModel = followModel;
            this.modifiers = modifiers;
        }
        
        public void SetCameraView(Vector3 offset, Vector3 rotation, Vector3 pivot,
            float baseFOV, bool followModel, bool lookAtPivot)
        {
            SetCameraView(offset, rotation, pivot, baseFOV, followModel, lookAtPivot,
                new ViewModifiers(new(-5, -5, -5), new(5, 5, 5), new(0, 0, 0), new(0, 0, 0), 1, 1, 1, 1, 1, 1, 1));
        }
        
        public void SetModifiers(ViewModifiers modifiers)
        {
            this.modifiers = modifiers;
        }
        
    }
    
    /// <summary> Modify the amount of each camera effects applied to the current camera view. </summary>
    [Serializable] public struct ViewModifiers
    {
        public Vector3 followMin;
        public Vector3 followMax;
        public Vector3 orbitMin;
        public Vector3 orbitMax;
        [Space(5)]
        [Range(0, 1)] public float followDampMix;
        [Range(0, 1)] public float orbitDampMix;
        [Range(0, 1)] public float offsetPosMix;
        [Range(0, 1)] public float offsetRotMix;
        [Range(0, 1)] public float shakePosMix;
        [Range(0, 1)] public float shakeRotMix;
        [Range(0, 1)] public float fOVOffsetMix;
        
        public ViewModifiers(Vector3 followMin, Vector3 followMax, Vector3 orbitMin, Vector3 orbitMax,
            float followDampMix, float orbitDampMix, float offsetPosMix, float offsetRotMix,
            float shakePosMix, float shakeRotMix, float fOVOffsetMix)
        {
            this.followMin = followMin;
            this.followMax = followMax;
            this.orbitMin = orbitMin;
            this.orbitMax = orbitMax;
            
            this.followDampMix = followDampMix;
            this.orbitDampMix = orbitDampMix;
            this.offsetPosMix = offsetPosMix;
            this.offsetRotMix = offsetRotMix;
            this.shakePosMix = shakePosMix;
            this.shakeRotMix = shakeRotMix;
            this.fOVOffsetMix = fOVOffsetMix;
        }
        
        public Vector3 ApplyFollowClamp(Vector3 followDamp)
            => followDamp.Clamp(followMin.ReplaceZero(Mathf.NegativeInfinity), followMax.ReplaceZero(Mathf.Infinity));
        
        public Vector3 ApplyOrbitClamp(Vector3 orbitDamp)
            => orbitDamp.Clamp(orbitMin.ReplaceZero(Mathf.NegativeInfinity), orbitMax.ReplaceZero(Mathf.Infinity));
        
    }
    
    /// <summary> Set the amount of follow damping applied and then scale the damped output. </summary>
    [Serializable] public struct FollowDamping
    {
        public bool enabled;
        [Range(0, 2)] public float xDamping;
        [Range(0, 2)] public float yDamping;
        [Range(0, 2)] public float zDamping;
        [Range(0, 1)] public float xScale;
        [Range(0, 1)] public float yScale;
        [Range(0, 1)] public float zScale;
        
        public Vector3 damping => new Vector3(xDamping, yDamping, zDamping);
        public Vector3 scale => new Vector3(xScale, yScale, zScale);
        
        public void SetDamping(float x, float y, float z) {xDamping = x; yDamping = y; zDamping = z;}
        public void SetScale(float x, float y, float z) {xScale = x; yScale = y; zScale = z;}
    }
    
    /// <summary> Set the amount of orbit damping applied and then scale the damped output. </summary>
    [Serializable] public struct OrbitDamping
    {
        public bool enabled;
        [Range(0, 2)] public float pitchDamping;
        [Range(0, 2)] public float yawDamping;
        [Range(0, 2)] public float rollDamping;
        [Range(0, 1)] public float pitchScale;
        [Range(0, 1)] public float yawScale;
        [Range(0, 1)] public float rollScale;
        
        public Vector3 damping => new Vector3(pitchDamping, yawDamping, rollDamping);
        public Vector3 scale => new Vector3(pitchScale, yawScale, rollScale);
        
        public void SetDamping(float pitch, float yaw, float roll) {pitchDamping = pitch; yawDamping = yaw; rollDamping = roll;}
        public void SetScale(float pitch, float yaw, float roll) {pitchScale = pitch; yawScale = yaw; rollScale = roll;}
    }
    
    
    
}
