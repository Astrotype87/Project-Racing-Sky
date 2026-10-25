using System;
using UnityEngine;

namespace ProjectRacingSky.Ship
{
    [Serializable] public struct Sensor
    {
        public Vector3 offset;
        public Vector3 rotation;
        
        public Sensor(float offsetX, float offsetY, float offsetZ,
            float rotationX = 0, float rotationY = 0, float rotationZ = 0)
        {
            this.offset = new Vector3(offsetX, offsetY, offsetZ);
            this.rotation = new Vector3(rotationX, rotationY, rotationZ);
        }
        
        public Sensor(Vector3 offset, Vector3 rotation = new Vector3())
        {
            this.offset = offset;
            this.rotation = rotation;
        }
    }
    
    [Serializable] public struct MainSensors
    {
        public LayerMask groundLayer;
        public Sensor center;
        public Sensor front;
        public Sensor back;
        public Sensor left;
        public Sensor right;
    }
    
    [Serializable] public struct SensorInfo
    {
        public bool isHit;
        public Vector3 position;
        public Vector3 direction;
        public Vector3 endpoint;
        public Vector3 normal;
        public GameObject hitObject;
        
        public void UpdateSensor(Sensor sensor, (Transform parent, Direction direction, float range, LayerMask layer) settings)
        {
            this.position = settings.parent.position + settings.parent.TransformVector(sensor.offset);
            this.direction = settings.direction == Direction.World ? Vector3.down
                : Quaternion.Euler(sensor.rotation) * -settings.parent.up;
            
            if (settings.direction == Direction.Ground)
                if (UnityEngine.Physics.Raycast(this.position, this.direction, out RaycastHit hit1, settings.range, settings.layer))
                    this.direction = -hit1.normal;
            
            if (isHit = UnityEngine.Physics.Raycast(this.position, this.direction, out RaycastHit hit2, settings.range, settings.layer))
            {
                this.endpoint = hit2.point;
                this.normal = hit2.normal;
            }
            else
            {
                this.endpoint = this.position + this.direction * settings.range;
                this.normal = -this.direction;
            }
            
            this.hitObject = isHit ? hit2.collider.gameObject : null;
        }
    }
    
}

