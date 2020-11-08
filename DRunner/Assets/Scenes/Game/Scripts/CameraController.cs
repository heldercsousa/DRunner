using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using DRunner.Actors;

namespace DRunner.Scenes
{
    /// <summary>
    /// Controls main game camera according to game states 
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CameraController : MonoBehaviour
    {
        private Animator _cameraAnimator;
        private bool _introductionDone = false;

        // z offset from player
        public float zOffset = 15f;

        public UnityEvent OnIntroductionDone;

        void Awake()
        {
            _cameraAnimator = GetComponent<Animator>();
            // Introduce();
        }

        public void Introduce()
        {
            if (!_introductionDone)
            {
                _cameraAnimator.Play("CameraPlay");
            }
        }

        public void HandleCameraIntrodutionDone()
        {
            _introductionDone = true;
            _cameraAnimator.enabled = false; // allow transform to be moved
            StartCoroutine(_FollowRunner());
            OnIntroductionDone.Invoke();
        }

        IEnumerator _FollowRunner()
        {
            float t = 0f;
            float moveTime = 1f;

            var startPos = transform.position;
            Func<float> targetZPos = () => RunnerController.Instance.transform.position.z - zOffset;

            while (t < moveTime)
            {
                t += Time.deltaTime / moveTime;
                // t += Time.deltaTime;

                var goalPosition = new Vector3(startPos.x, startPos.y, targetZPos());
                
                // transform.position = Vector3.Lerp(startPos, goalPosition, t/moveTime);
                transform.position = Vector3.Lerp(startPos, goalPosition, Mathf.SmoothStep(0.0f, 1.0f, t));

                yield return null;
            }

            while (GameController.Instance.Playing)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, targetZPos());
                yield return null;
            }
        }

    }
}

