using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DRunner.Scenes
{
    public class MainMenuController : MonoBehaviour
    {
        public Image backgroundImage;
        public Image titleImage;
        public Text titleText;
        public Text tapText;

        void Awake()
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        }

        void Update()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                 Play(null);
            }
            #endif

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!GameController.Instance.Playing)
                {
                    Quit();
                }
            }

        }

        void Play(Lean.Touch.LeanFinger finger)
        {
            backgroundImage.enabled = false;
            titleImage.enabled = false;
            titleText.enabled = false;
            tapText.enabled = false;
            DRunner.Scenes.GameController.Instance.Play();
        }

        void Quit()
        {
            Application.Quit();
        }

        void OnEnable()
        {
            Lean.Touch.LeanTouch.OnFingerTap += Play;
        }

        void OnDisable()
        {
            Lean.Touch.LeanTouch.OnFingerTap -= Play;
        }
    } 
}

