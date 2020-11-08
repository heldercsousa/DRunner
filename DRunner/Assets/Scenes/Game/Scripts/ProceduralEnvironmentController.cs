using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DRunner.Scenes
{
    /// <summary>
    /// Procedurally generates an environment where the level happens
    /// </summary>
    public class ProceduralEnvironmentController : MonoBehaviour
    {
        public ProceduralObjectController[] roads;

        // every road once instantiated
        public List<ProceduralObjectController> _objectsPool;
        private Vector3 _currentRoadSpawnPosition;
        // roads been shown in scene
        private Queue<ProceduralObjectController> _built;

        void Awake()
        {
            if (roads.Length == 0 || roads.Any(x => x == null))
            {
                Debug.LogError("Roads não definido");
            }
            _currentRoadSpawnPosition = transform.position;
            _objectsPool = new List<ProceduralObjectController>();
            _built = new Queue<ProceduralObjectController>();

            // build 4 pieces of same road for the game start
            var po = _PickRandomProceduralObject();
            for (var i = 0; i < 4; i++)
            {
                _Instantiate(po);
            }
        }

        void _Instantiate(ProceduralObjectController po)
        {
            var newPo = Instantiate<ProceduralObjectController>(po, _currentRoadSpawnPosition, Quaternion.identity);
            newPo.gameObject.name = po.gameObject.name;
            _UpdateRoadSpawnPosition(newPo);
            _objectsPool.Add(newPo);
            _built.Enqueue(newPo);
        }

        void _ShowInstance(ProceduralObjectController po)
        {
            po.gameObject.transform.position = _currentRoadSpawnPosition;
            po.gameObject.SetActive(true);
            _built.Enqueue(po);
            _UpdateRoadSpawnPosition(po);
        }

        void _UpdateRoadSpawnPosition(ProceduralObjectController lastObjectShown)
        {
            _currentRoadSpawnPosition = new Vector3(_currentRoadSpawnPosition.x, _currentRoadSpawnPosition.y, _currentRoadSpawnPosition.z + lastObjectShown.zSize);
        }

        ProceduralObjectController _PickRandomProceduralObject()
        {
            var poIdx = Random.Range(0, roads.Length);
            var po = roads[poIdx];
            return po;
        }

        /// <summary>
        /// Removes oldest (behind player) and insert other proceduralObject ahead, using object pool
        /// </summary>
        public void Next()
        {
            // removes the oldest
            var oldestPO = _built.Dequeue();
            oldestPO.gameObject.SetActive(false);

            // add one forward
            var po = _PickRandomProceduralObject();
            var availableInPool = _objectsPool
                .Where(x => x.gameObject.name.Equals(po.gameObject.name) && !x.gameObject.activeSelf);

            var poAvailable = availableInPool.FirstOrDefault();
            if (poAvailable!=null)
            {
                _ShowInstance(poAvailable);
            }
            else
            {
                _Instantiate(po);
            }
        }
    }
}
