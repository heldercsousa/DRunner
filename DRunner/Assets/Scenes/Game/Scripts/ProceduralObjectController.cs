using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DRunner.Scenes
{
    public class ProceduralObjectController : MonoBehaviour
    {
        public float ySpawnOffset = 0f;
        public Vector3 dimension;
        public ColliderInfo[] colliders;

        public int Depth;

        [System.Serializable]
        public class ColliderInfo
        {
            public Collider collider;
            public float ySpawnOffset;
            public float zSpawnOffset;
        }
    }
}