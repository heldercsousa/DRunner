using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DRunner.Scenes
{
    /// <summary>
    /// Procedurally generates obstacles, enemies, collectibles and any object directly related to level, adjusting dificulty along gameplay.
    /// Based on following DRunner procedural level planning: https://docs.google.com/spreadsheets/d/1n3o6eK7pBYpVd7zxlNwid8qd67-AbC3-I25X963jS5k/edit?usp=sharing
    /// </summary>
    // [RequireComponent(typeof(ProceduralLevelController))]
    public class ProceduralLevelController : MonoBehaviour
    {
        public ProceduralIncrementalController trailLeft;
        public ProceduralIncrementalController trailCenter;
        public ProceduralIncrementalController trailRight;

        public QuadrantTrailsCombination[] quadrantTrailsCombinations;
        public QuadrantCombination[] quadrantCombinations;
        public float quadrantZDimension;

        private QuadrantTrailsCombination _lastQuadrantTrailsCombination;

        void Awake()
        {
            if (trailLeft == null)
            {
                Debug.LogError("trailLeft não definido");
            }

            if (trailCenter == null)
            {
                Debug.LogError("trailCenter não definido");
            }

            if (trailRight == null)
            {
                Debug.LogError("trailRight não definido");
            }
        }

        /// <summary>
        /// builds level elements specifically for the game start
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IEnumerable<ProceduralObjectController> BuildStartLevelElementsForDepth(int depth)
        {
            trailLeft.Depth = depth;
            trailCenter.Depth = depth;
            trailRight.Depth = depth;

            var initialQuadrantTrailsCombination = quadrantTrailsCombinations
            .Where(x => x.trailLeftLocked && !x.trailCenterLocked && x.trailRightLocked)
            .Single();

            _lastQuadrantTrailsCombination = initialQuadrantTrailsCombination;

            for (int i = 1; i <= 5; i++)// 5 quadrantes de 12 unidades (total 60 unidades (tamanho da road) )
            {
                var newObjLeft = trailLeft.InsertNewObject();
                yield return newObjLeft;

                trailCenter.SkipZSpace(quadrantZDimension);

                var newObjRight = trailRight.InsertNewObject();
                yield return newObjRight;
            }
        }

        /// <summary>
        /// builds level elements according to predicted combinations
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IEnumerable<ProceduralObjectController> BuildLevelForDepth(int depth)
        {
            trailLeft.Depth = depth;
            trailCenter.Depth = depth;
            trailRight.Depth = depth;
            
            for (int i = 1; i <= 5; i++)// 5 quadrantes de 12 (total 60 - tamanho da road)
            {
                // gets a quadrantCombination which combines to _lastQuadrantTrailsCombination
                var matchingQuadrants = quadrantCombinations
                .Where(x => x.quadrant1.Equals(_lastQuadrantTrailsCombination.Id))
                .ToArray();
                
                var randomQuadrantIdx = Random.Range(0, matchingQuadrants.Length);
                var quadrantTrailsCombID = quadrantCombinations[randomQuadrantIdx].quadrant2;
                var quadrantTrailsComb = quadrantTrailsCombinations.Where(x => x.Id.Equals(quadrantTrailsCombID)).Single();

                // trail left
                if (!quadrantTrailsComb.trailLeftLocked)
                {
                    var newOb = trailLeft.InsertNewObject();
                    yield return newOb;
                }
                else
                {
                    trailLeft.SkipZSpace(quadrantZDimension);
                }

                // trail center
                if (!quadrantTrailsComb.trailCenterLocked)
                {
                    var newOb = trailCenter.InsertNewObject();
                    yield return newOb;
                }
                else
                {
                    trailCenter.SkipZSpace(quadrantZDimension);
                }

                // trail right
                if (!quadrantTrailsComb.trailRightLocked)
                {
                    var newOb = trailRight.InsertNewObject();
                    newOb.Depth = depth;
                    yield return newOb;
                }
                else
                {
                    trailRight.SkipZSpace(quadrantZDimension);
                }

                _lastQuadrantTrailsCombination = quadrantTrailsComb;

            }

        }
 
        [System.Serializable]
        public class QuadrantTrailsCombination
        {
            public string Id;
            public bool trailLeftLocked;
            public bool trailCenterLocked;
            public bool trailRightLocked;
        }
               
        [System.Serializable]
        public class QuadrantCombination
        {
            public string name;
            public string quadrant1;
            public string quadrant2;
        }


    }
}

