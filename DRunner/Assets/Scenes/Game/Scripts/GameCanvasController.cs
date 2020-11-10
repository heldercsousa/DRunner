using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviour
{
    public Button pauseButton;
    public Image pauseButtonImage;
    public Button resumeButton;
    public Image resumeButtonImage;
    public Button quitButton;
    public Image quitButtonImage;
    public Image pausePanelImage;
    public Image mainPanelImage;

    void Awake()
    {
        if (pauseButton ==null)
        {
            Debug.LogError("pauseButton nao definido");
        }

        if (resumeButton == null)
        {
            Debug.LogError("resumeButton nao definido");
        }

        if (quitButton == null)
        {
            Debug.LogError("quitButton nao definido");
        }

        if (pauseButtonImage == null)
        {
            Debug.LogError("pauseButtonImage nao definido");
        }

        if (resumeButtonImage == null)
        {
            Debug.LogError("resumeButtonImage nao definido");
        }

        if (quitButtonImage == null)
        {
            Debug.LogError("quitButtonImage nao definido");
        }

        if (pausePanelImage == null)
        {
            Debug.LogError("pausePanelImage nao definido");
        }

        if (mainPanelImage == null)
        {
            Debug.LogError("mainPanelImage nao definido");
        }

        QuitClicked();
    }

    // Update is called once per frame
    public void StartClicked()
    {
        mainPanelImage.enabled = true;
        pausePanelImage.enabled = false;
        pauseButton.enabled = true;
        quitButton.enabled = false;
        resumeButton.enabled = false;
        pauseButtonImage.enabled = false;
        quitButtonImage.enabled = false;
        resumeButtonImage.enabled = false;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitClicked()
    {
        mainPanelImage.enabled = false;
        pausePanelImage.enabled = false;
        pauseButton.enabled = false;
        quitButton.enabled = false;
        resumeButton.enabled = false;
        pauseButtonImage.enabled = false;
        quitButtonImage.enabled = false;
        resumeButtonImage.enabled = false;

        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeClicked()
    {
        mainPanelImage.enabled = false;
        pausePanelImage.enabled = false;
        pauseButton.enabled = true;
        quitButton.enabled = false;
        resumeButton.enabled = false;
        pauseButtonImage.enabled = true;
        quitButtonImage.enabled = false;
        resumeButtonImage.enabled = false;
    }

    public void PauseClicked()
    {
        mainPanelImage.enabled = true;
        pausePanelImage.enabled = true;
        pauseButton.enabled = false;
        quitButton.enabled = true;
        resumeButton.enabled = true;
        pauseButtonImage.enabled = true;
        quitButtonImage.enabled = true;
        resumeButtonImage.enabled = true;
    }
    
}
