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
        public SkinnedMeshRenderer meshRenderer;

        // events 
        public UnityEvent OnProceduralTriggerEnter;
        public UnityEvent OnDeath;

        public Vector3 targetTrail { get; private set; }
        public Vector3 currentTrail { get; private set; }

        public Vector3 trailLeft { get; private set; }
        public Vector3 trailCenter { get; private set; }
        public Vector3 trailRight { get; private set; }

        public bool Freezed { get; private set; }

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
            _animatorInstance.speed = 1.2f;

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
            if (meshRenderer == null)
            {
                Debug.LogError("meshRenderer não definido");
            }

        }

        void Start()
        {
            var levelCtr = Scenes.ProceduralLevelController.Instance;
            trailLeft = levelCtr.trailLeft.transform.position;
            trailCenter = levelCtr.trailCenter.transform.position;
            trailRight = levelCtr.trailRight.transform.position;
            currentTrail = trailCenter;
        }

        public void Walk()
        {
             Freezed = false;
            _animatorInstance.runtimeAnimatorController = walkController;
            forwardSpeed = 5f; 
        }

        public void Run()
        {
             Freezed = false;
            _animatorInstance.runtimeAnimatorController = runController;
            forwardSpeed = 20f;
        }

        public void Jump()
        {
            _animatorInstance.runtimeAnimatorController = jumpController;
        }

        public void Freeze()
        {
            Freezed = true;
            _animatorInstance.speed = 0f;
        }

        public void Unfreeze()
        {
            Freezed = false;
            _animatorInstance.speed = 1.2f;
        }

        public void Reset()
        {
            Freezed = false;
            _animatorInstance.speed = 1.2f;
            _animatorInstance.runtimeAnimatorController = idleController;
            transform.position = trailCenter;
        }

        void Update()
        {
            if (Freezed)
            {
                return;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (forwardSpeed * Time.deltaTime));

            if (!Scenes.CameraController.Instance.IntroductionDone)
            {
                return;
            }
            
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
            if (currentTrail == trailCenter)
            {
                currentTrail = trailLeft;
            }
            else if (currentTrail == trailRight)
            {
                currentTrail = trailCenter;
            }

            float t = 0f;
            float moveTime = 0.6f;

            var startXPos = transform.position.x;
            var targetXPos = currentTrail.x;

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
            if (currentTrail == trailLeft)
            {
                currentTrail = trailCenter;
            }
            else if (currentTrail == trailCenter)
            {
                currentTrail = trailRight;
            }

            float t = 0f;
            float moveTime = 0.6f;

            var startXPos = transform.position.x;
            var targetXPos = currentTrail.x;

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

        void OnCollisionEnter(Collision other) {
            if (other.transform.tag.Equals(Scenes.ProceduralLevelController.Instance.enemyTag))
            {
                // transform.position = trailCenter;
                StopAllCoroutines();
                Freeze();

                StartCoroutine(_Blink());
                StartCoroutine(_Wait(1.5f));
            }
        }

        IEnumerator _Wait(float seconds)
		{
			yield return new WaitForSeconds (seconds);
            StopAllCoroutines();
            OnDeath.Invoke();
            meshRenderer.enabled = true;
		}

        IEnumerator _Blink()
        {
            while (true)
            {
                meshRenderer.enabled = false;
                yield return new WaitForSeconds (0.1f);
                meshRenderer.enabled = true;
                 yield return new WaitForSeconds (0.1f);
            }
        }


    }

}

