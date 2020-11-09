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
        public UnityEvent OnPause;
        public UnityEvent OnResume;

        public bool Playing { get; private set; } = false;
        public bool Paused { get; private set; } = false;

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

        [ContextMenu("Play Game")]
        public void Play()
        {
            if (Playing)
            {
                return;
            }
            Playing = true;
            OnPlay.Invoke();
        }

        public void Pause()
        {
            if (Paused)
            {
                return;
            }
            Paused = true;
            OnPause.Invoke();
        }

        public void Resume()
        {
            if (!Paused)
            {
                return;
            }
            Paused = false;
            OnResume.Invoke();
        }

    }
}
