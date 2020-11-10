using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace DRunner.Scenes
{
    /// <summary>
    /// Procedurally generates obstacles, enemies, collectibles and any object directly related to level, adjusting dificulty along gameplay.
    /// Based on following DRunner procedural level planning: https://docs.google.com/spreadsheets/d/1n3o6eK7pBYpVd7zxlNwid8qd67-AbC3-I25X963jS5k/edit?usp=sharing
    /// </summary>
    public class ProceduralLevelController : MonoBehaviour
    {
        public static ProceduralLevelController Instance;
        public ProceduralIncrementalController trailLeft;
        public ProceduralIncrementalController trailCenter;
        public ProceduralIncrementalController trailRight;

        public QuadrantTrailsCombination[] quadrantTrailsCombinations;
        public QuadrantCombination[] quadrantCombinations;
        public float quadrantZDimension;
        public string enemyTag = "Enemy";

        // defines how many times the quadrant combination must be rendered
        private QuadrantTrailsCombination _currentQuadrantTrailsCombination;
        // defines how many times the quadrant combination must be rendered
        private int _currentQuadrantTrailsCombinationRepetitions;
        // defines how many times the quadrant combination was rendered
        private int _currentQuadrantTrailsCombinationRepetitionsDone;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (!System.Object.ReferenceEquals(Instance, this))
            {
                Debug.LogError("Instância de ProceduralLevelController já existe");
            }

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

            // full fill navigation properties (quadrant1Instance/quadrant2Instance) of quadrantCombinations
           quadrantCombinations = quadrantCombinations
            .Join(quadrantTrailsCombinations, qc => qc.quadrant1, qtc => qtc.Id, (qc, qtc) => {
                qc.quadrant1Instance = qtc;
                return qc;
            })
            .Join(quadrantTrailsCombinations, qc => qc.quadrant2, qtc => qtc.Id, (qc, qtc) => {
                qc.quadrant2Instance = qtc;
                return qc;
            })
            .ToArray();
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

            // only QuadrantTrailsCombination which Center is free to run by the player
            _DefineNewQuadrantCombinationRepetition(x => x.quadrant2Instance.trailLeftLocked 
                && !x.quadrant2Instance.trailCenterLocked 
                && x.quadrant2Instance.trailRightLocked);

            // forces engine to create 5 equals quadrants, standing for the first obstacles player will head in the gameplay
            _currentQuadrantTrailsCombinationRepetitions = 6;
            
            foreach (var item in _InsertQuadrantRepetition())
            {
                yield return item;
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

            foreach (var obj in _InsertQuadrantRepetition())
            {
                yield return obj;
            }
        }

        /// <summary>
        /// inserts an object in each of 3 trails, according to _currentQuadrantTrailsCombination
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ProceduralObjectController> _InsertQuadrantRepetition()
        {
            // full fill 5 quadrants per detph
            var qtdQuadrantsThisDepth = 0;

            while (_currentQuadrantTrailsCombinationRepetitions - _currentQuadrantTrailsCombinationRepetitionsDone > 0)
            {
                var objLeft = _InsertObject(trailLeft, _currentQuadrantTrailsCombination.trailLeftLocked);
                if (objLeft != null)
                {
                    yield return objLeft;
                }
                var objCenter = _InsertObject(trailCenter, _currentQuadrantTrailsCombination.trailCenterLocked);
                if (objCenter != null)
                {
                    yield return objCenter;
                }
                var objRight = _InsertObject(trailRight, _currentQuadrantTrailsCombination.trailRightLocked);
                if (objRight != null)
                {
                    yield return objRight;
                }
                qtdQuadrantsThisDepth++;
                _currentQuadrantTrailsCombinationRepetitionsDone++;
                if (_currentQuadrantTrailsCombinationRepetitions - _currentQuadrantTrailsCombinationRepetitionsDone == 0)
                {
                    // time to change the quadrantTrailsCombination repetition, where combination differs from the last one rendered
                    _DefineNewQuadrantCombinationRepetition(x => x.quadrant2 != _currentQuadrantTrailsCombination.Id);
                }
                if (qtdQuadrantsThisDepth == 6) // 6 per Depth
                {
                    break;
                }
                // Debug.Log($"_InsertQuadrantRepetition done-- depth: {trailLeft.Depth} combi: {_currentQuadrantTrailsCombination.ToString()} -- currentQuadrantReptDone:{_currentQuadrantTrailsCombinationRepetitionsDone.ToString()}");
            }
        }

        /// <summary>
        /// inserts an object in a trail
        /// </summary>
        /// <param name="trail">instance of ProceduralIncrementalController</param>
        /// <param name="trailLocked">indicates whether trail is blocked</param>
        /// <returns></returns>
        private ProceduralObjectController _InsertObject(ProceduralIncrementalController trail, bool trailLocked)
        {
            if (trailLocked)
            {
                var ob = trail.InsertNewObject();
                return ob;
            }
            trail.SkipZSpace(quadrantZDimension);
            return null;
        }

        // private void print(string txt, QuadrantCombination[] array)
        // {
        //     return txt + string.Join(System.Environment.NewLine, array.Select(x => x.ToArray()));
        // }

        /// <summary>
        /// defines a new QuadrantCombination to be repeated in a random number of times, according to matching criterias
        /// </summary>
        /// <param name="filter">function to filter the QuadrantCombination list. Pass null to this param if you want method to pick an object in the entire list of QuadrantCombination</param>
        private void _DefineNewQuadrantCombinationRepetition(Func<QuadrantCombination,bool> filter = null)
        {
            // Picks quadrants which matchs to last one rendered
            var matchingQuadrants = quadrantCombinations
            .Where(x => _currentQuadrantTrailsCombination == null || 
                ( x.quadrant1.Equals(_currentQuadrantTrailsCombination.Id) && x.quadrant1 != x.quadrant2))
            .ToArray();

            if (filter!=null)
            {
                matchingQuadrants = matchingQuadrants
                .Where(filter)
                .ToArray();
            }
            
            var randomQuadrantIdx = UnityEngine.Random.Range(0, matchingQuadrants.Length);
            var quadrantTrailsComb = matchingQuadrants[randomQuadrantIdx].quadrant2Instance; // quadrant which combines to _currentQuadrantTrailsCombination

            _currentQuadrantTrailsCombinationRepetitions = UnityEngine.Random.Range(1, 4); // this quadrant will repeat from 1 to 3 times
            if (!quadrantTrailsComb.trailLeftLocked && !quadrantTrailsComb.trailCenterLocked && !quadrantTrailsComb.trailRightLocked)
            {
                _currentQuadrantTrailsCombinationRepetitions = 1;
            }
            _currentQuadrantTrailsCombinationRepetitionsDone = 0;
            _currentQuadrantTrailsCombination = quadrantTrailsComb;

            // Debug.Log($"_DefineNewQuadrantCombinationRepetition -- currentQuadrant:{_currentQuadrantTrailsCombination.ToString()} -- currenRepetition:{_currentQuadrantTrailsCombinationRepetitions}");
        }

        [System.Serializable]
        public class QuadrantTrailsCombination
        {
            public string Id;
            public bool trailLeftLocked;
            public bool trailCenterLocked;
            public bool trailRightLocked;

            public override string ToString()
            {
                Func<bool,string> txt = (locked) => locked ? "Locked" : "Free"; 
                return $"{Id}|{txt(trailLeftLocked)}|{txt(trailCenterLocked)}|{txt(trailRightLocked)}";
            }
        }
               
        [System.Serializable]
        public class QuadrantCombination
        {
            public string name;
            public string quadrant1;
            public string quadrant2;
            public QuadrantTrailsCombination quadrant1Instance { get; set; }
            public QuadrantTrailsCombination quadrant2Instance { get; set; }
        }

    }
}

