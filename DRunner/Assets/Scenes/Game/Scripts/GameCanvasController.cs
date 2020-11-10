using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviour
{
    public Image pausePanelImage;
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

        mainPanelImage.gameObject.SetActive(false);
        pausePanelImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void StartClicked()
    {
        mainPanelImage.gameObject.SetActive(true);
        mainPanelImage.enabled = false;
        pausePanelImage.gameObject.SetActive(false);
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
    
}
