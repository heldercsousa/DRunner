using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DRunner.Scenes
{
    /// <summary>
    /// Procedurally instantiate/spawn ProceduralObjectControllers in a straight direction  
    /// </summary>
    public class ProceduralIncrementalController : MonoBehaviour
    {
        public ProceduralObjectController[] proceduralObjects;
        /// <summary>
        /// current incrementation depth. Objects spawned are marked with this value. Usefull to control which object to remove when controlling procedural generation
        /// </summary>
        /// <value></value>
        public int Depth { get; set; } = 0;
        
        // position to spawn next object
        private Vector3 _CurrentSpawnPosition { get; set; }
        // every object once instantiated
        private List<ProceduralObjectController> _objectsPool;
        // objects been shown in scene
        private Queue<ProceduralObjectController> _built;

        protected virtual void Awake()
        {
            if (proceduralObjects.Length == 0 || proceduralObjects.Any(x => x == null))
            {
                Debug.LogError("proceduralObjects não definido");
            }
            _CurrentSpawnPosition = transform.position;
            _objectsPool = new List<ProceduralObjectController>();
            _built = new Queue<ProceduralObjectController>();
        }

        /// <summary>
        /// removes procedural objects at the oldest depth still active, and returns a lazy list of removed objects 
        /// </summary>
        public IEnumerable<ProceduralObjectController> RemoveObjectsAtOldestDepth()
        {
            if (_built.Any())
            {
                var oldestDepth = _built.Peek().Depth;
                var currentDepth = oldestDepth;
                while (currentDepth == oldestDepth)
                {
                    var poAtDepth = _built.Dequeue();
                    poAtDepth.gameObject.SetActive(false);
                    currentDepth = _built.Peek().Depth;
                    yield return poAtDepth;
                }
            }
        }

        /// <summary>
        /// inserts a new object right after the last one
        /// </summary>
        /// <returns></returns>
        public ProceduralObjectController InsertNewObject(string objectName = null)
        {
            var poIdx = 0;
            ProceduralObjectController po = null;

            if (string.IsNullOrEmpty(objectName))
            {
                poIdx = Random.Range(0, proceduralObjects.Length);
                po = proceduralObjects[poIdx];
            }
            else
            {
                po = proceduralObjects
                    .Where(x => x.gameObject.name.Equals(objectName))
                    .SingleOrDefault();

                if (po == null)
                {
                    throw new System.Exception("parametro objectName com valor inválido");
                }
            }

            var availableInPool = _objectsPool
                        .Where(x => x.gameObject.name.Equals(po.gameObject.name) &&
                                    !x.gameObject.activeSelf);

            var poAvailable = availableInPool.FirstOrDefault();
            if (poAvailable != null)
            {
                _ShowInstance(poAvailable);
            }
            else
            {
                _Instantiate(po);
            }

            return po;
        }

        /// <summary>
        /// adds an empty space on generation sequence
        /// </summary>
        /// <param name="space"></param>
        public void SkipZSpace(float space)
        {
            _CurrentSpawnPosition = new Vector3(_CurrentSpawnPosition.x, _CurrentSpawnPosition.y, _CurrentSpawnPosition.z + space);
            // _CurrentSpawnPosition = new Vector3(_CurrentSpawnPosition.x, _CurrentSpawnPosition.y, _CurrentSpawnPosition.z + lastObjectShown.dimension.z);
        }

        /// <summary>
        /// Instantiate a new procedural object
        /// </summary>
        /// <param name="po"></param>
        private void _Instantiate(ProceduralObjectController po)
        {
            var spawnPosition = _GetSpawnPosition(po);
            var newPo = Instantiate<ProceduralObjectController>(po, spawnPosition, Quaternion.identity);
            newPo.gameObject.name = po.gameObject.name;
            newPo.Depth = Depth;
            _objectsPool.Add(newPo);
            _built.Enqueue(newPo);

            // instantiate proceduralObject colliders
            if (newPo.colliders.Length > 0)
            {
                foreach (var colliderInfo in newPo.colliders)
                {
                    var colliderPosition = new Vector3(spawnPosition.x, spawnPosition.y + colliderInfo.ySpawnOffset, spawnPosition.z + colliderInfo.zSpawnOffset);
                    var colliderInstance = Instantiate<Collider>(colliderInfo.collider, colliderPosition, Quaternion.identity);
                    colliderInstance.transform.parent = newPo.transform;
                }
            }

            _UpdateSpawnPosition(newPo);
        }

        /// <summary>
        /// Show an inactivated instance 
        /// </summary>
        /// <param name="po"></param>
        private void _ShowInstance(ProceduralObjectController po)
        {
            po.gameObject.transform.position = _GetSpawnPosition(po);
            po.Depth = Depth;
            po.gameObject.SetActive(true);
            _built.Enqueue(po);
            _UpdateSpawnPosition(po);
        }

        /// <summary>
        /// Update the position where next object has to be spawned
        /// </summary>
        /// <param name="lastObjectShown"></param>
        private void _UpdateSpawnPosition(ProceduralObjectController lastObjectShown)
        {
            _CurrentSpawnPosition = new Vector3(_CurrentSpawnPosition.x, _CurrentSpawnPosition.y, _CurrentSpawnPosition.z + lastObjectShown.dimension.z);
        }

        /// <summary>
        /// Spawn position considering yOffset
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        private Vector3 _GetSpawnPosition(ProceduralObjectController po)
        {
            return new Vector3(_CurrentSpawnPosition.x, _CurrentSpawnPosition.y + po.ySpawnOffset, _CurrentSpawnPosition.z);
        }

    }
}

