using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DRunner.Scenes
{
    /// <summary>
    /// Controls core gameplay. Game has to be started by using this class instance.
    /// </summary>
    [RequireComponent(typeof(ProceduralEnvironmentController))]
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        
        public UnityEvent OnPlay;

        public bool Playing { get; private set; } = false;

        void Awake()
        {
            if (GameController.Instance == null)
            {
                GameController.Instance = this;
            }
            else if (!Object.ReferenceEquals(GameController.Instance, this))
            {
                Debug.LogError("Instância de GameController já existe");
            }
        }

        public void Play()
        {
            if (Playing)
            {
                return;
            }
            Playing = true;
            OnPlay.Invoke();
        }

    }
}
