using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace DRunner.Scenes
{
    /// <summary>
        /// controls game play UI
        /// </summary>
    public class GameCanvasController : MonoBehaviour
    {
        public Image pausePanelImage;
        public Button pauseButton;
        public Image mainPanelImage;

        void Awake()
        {
            if (pausePanelImage == null)
            {
                Debug.LogError("pausePanelImage nao definido");
            }

            if (mainPanelImage == null)
            {
                Debug.LogError("mainPanelImage nao definido");
            }

            if (pauseButton == null)
            {
                Debug.LogError("pauseButton nao definido");
            }

            mainPanelImage.gameObject.SetActive(false);
            pausePanelImage.gameObject.SetActive(false);
        }

        // Update is called once per frame
        public void StartClicked()
        {
            mainPanelImage.gameObject.SetActive(false);
            mainPanelImage.enabled = false;
            pausePanelImage.gameObject.SetActive(false);
        }

        public void CameraIntroductionDone()
        {
            mainPanelImage.gameObject.SetActive(true);
            mainPanelImage.enabled = false;
        }

        public void QuitClicked()
        {
            mainPanelImage.gameObject.SetActive(false);
            pausePanelImage.gameObject.SetActive(false);
        
            SceneManager.LoadScene("MainMenu");
        }

        public void ResumeClicked()
        {
            mainPanelImage.gameObject.SetActive(true);
            mainPanelImage.enabled = false;
            pausePanelImage.gameObject.SetActive(false);
        }

        public void PauseClicked()
        {
            mainPanelImage.gameObject.SetActive(true);
            mainPanelImage.enabled = true;
            pausePanelImage.gameObject.SetActive(true);
        }

        void Update() {
            if (GameController.Instance.Playing && !GameController.Instance.Paused)
            {
                if (Input.GetKey("escape") && CameraController.Instance.IntroductionDone) // se estiver introduzindo a cena, não deixa pausar!
                {
                    pauseButton.onClick.Invoke(); // aciona o botão pause da UI
                }
            }    
        }
    }
    
}
