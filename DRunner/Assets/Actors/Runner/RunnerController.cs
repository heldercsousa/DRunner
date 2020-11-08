using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DRunner.Actors
{
    /// <summary>
    /// Singleton type. Stands for the main player actor (plays animations, listen to user inputs and reacts to them, and reacts to environment events) 
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class RunnerController : MonoBehaviour
    {
        // static fields
        public static RunnerController Instance;

        // private fieds
        private Animator _animatorInstance;
        
        // instance fieds
        public RuntimeAnimatorController idleController;
        public RuntimeAnimatorController walkController;
        public RuntimeAnimatorController walkBackController;
        public RuntimeAnimatorController runController;
        public RuntimeAnimatorController sprintController;
        public RuntimeAnimatorController jumpController;
        public float forwardSpeed = 0f;
        public float sideMovementOffset = 1.25f;

        // events 
        public UnityEvent OnProceduralTriggerEnter;

        void Awake() {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (!Object.ReferenceEquals(Instance, this))
            {
                Debug.LogError("Instância de RunnerController já existe");
            }

            _animatorInstance = GetComponent<Animator>();

            if (idleController == null)
            {
                Debug.LogError("idleController não definido");
            }
            if (walkController == null)
            {
                Debug.LogError("walkController não definido");
            }
            if (walkBackController == null)
            {
                Debug.LogError("walkBackController não definido");
            }
            if (runController == null)
            {
                Debug.LogError("runController não definido");
            }
            if (sprintController == null)
            {
                Debug.LogError("sprintController não definido");
            }
            if (jumpController == null)
            {
                Debug.LogError("jumpController não definido");
            }
        }

        public void Walk()
        {
            _animatorInstance.runtimeAnimatorController = walkController;
            forwardSpeed = 5f; 
        }

        public void Run()
        {
            _animatorInstance.runtimeAnimatorController = runController;
            forwardSpeed = 12.5f;
        }

        public void Jump()
        {
            _animatorInstance.runtimeAnimatorController = jumpController;
        }

        void Update()
        {
            // if (!GameController.Instance.IsPlaying)
            // {
            //     return;
            // }

            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (forwardSpeed * Time.deltaTime));

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StopAllCoroutines();
                StartCoroutine(_MoveLeft());
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StopAllCoroutines();
                StartCoroutine(_MoveRight());
            }
#endif

#if UNITY_ANDROID
            // if (Input.touchCount == 1)
            // {
            //     var touch = Input.touches[0];
            //     if (touch.phase == TouchPhase.Began)
            //     {
            //     }
            // }
#endif
        }

        IEnumerator _MoveLeft()
        {
            float t = 0f;
            float moveTime = 0.5f;

            var startXPos = transform.position.x;
            var targetXPos = transform.position.x - sideMovementOffset;

            while (t < moveTime)
            {
                t += Time.deltaTime / moveTime;

                var startPos = transform.position;
                var targetPosition = new Vector3(targetXPos, transform.position.y, transform.position.z);

                transform.position = Vector3.Lerp(startPos, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));

                yield return null;
            }
            
        }

        IEnumerator _MoveRight()
        {
            float t = 0f;
            float moveTime = 0.5f;

            var startXPos = transform.position.x;
            var targetXPos = transform.position.x + sideMovementOffset;

            while (t < moveTime)
            {
                t += Time.deltaTime / moveTime;

                var startPos = transform.position;
                var targetPosition = new Vector3(targetXPos, transform.position.y, transform.position.z);

                transform.position = Vector3.Lerp(startPos, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));

                yield return null;
            }
        }

        void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "ProceduralTrigger")
            {
                OnProceduralTriggerEnter.Invoke();
            }
        }

    }

}

