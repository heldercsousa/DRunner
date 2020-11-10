using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DRunner.Scenes
{
    /// <summary>
    /// Procedurally generates an environment where the level happens
    /// </summary>
    [RequireComponent(typeof(ProceduralLevelController))]
    public class ProceduralEnvironmentController : ProceduralIncrementalController
    {
        private ProceduralLevelController _levelController;

        protected override void Awake()
        {
            base.Awake();
            
            _levelController = GetComponent<ProceduralLevelController>();

            // build 4 pieces of same road for the game start
            Depth = 0;
            StartCoroutine("_Start");
        }

        /// <summary>
        /// Removes oldest road and level elements, and inserts a new road and level elements ahead the last one created
        /// </summary>
        public void Next()
        {
            StartCoroutine("_Next");
        }

        /// <summary>
        /// Lazy adds 3 roads for the early game
        /// </summary>
        /// <returns></returns>
        private IEnumerator _Start()
        {
            ProceduralObjectController insertedRoad = null;
            for (int i = 1; i <= 3; i++)
            {
                Depth++;

                // lazy inserts a road
                string roadName = null;
                if (insertedRoad != null)
                {
                    roadName = insertedRoad.gameObject.name;
                } 
                insertedRoad = InsertNewObject(roadName);
                yield return null;

                // lazy builds level elements over the road just created
                if (i == 1)
                {
                    foreach (var obj in _levelController.BuildStartLevelElementsForDepth(Depth))
                    {
                        yield return null;
                    } 
                } 
                else
                {
                    foreach (var obj in _levelController.BuildLevelForDepth(Depth))
                    {
                        yield return null;
                    }
                }
            }
        }

         /// <summary>
        /// Lazy removes and adds roads, futher calling levelController to lazy remove and create level related elements
        /// </summary>
        /// <returns></returns>
        private IEnumerator _Next()
        {
            Depth++;

            // lazy removes
            foreach (var removedObj in RemoveObjectsAtOldestDepth())
            {
                yield return null;
            }
            
            // lazy inserts a road
           InsertNewObject();
            yield return null;

            // lazy builds level elements over the road just created
            foreach (var removedObj in _levelController.BuildLevelForDepth(Depth))
            {
                yield return null;
            }
        }

    }
}
