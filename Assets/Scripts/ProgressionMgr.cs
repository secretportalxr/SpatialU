using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionMgr : MonoBehaviour
{
    public static ProgressionMgr instance;

    [Header("Data Objects")]
    //public List<TutorialData> Tutorials = new List<TutorialData>();
    public Transform spawnPoint;

    [Header("Tutorial Objects")]
    public VolumetricPlayer volumetricPlayer;

    [Header("UI Objects")]
    public GameObject mainMenuPanel;
    public GameObject tutorialPanel;
    public List<GameObject> contentPanels = new List<GameObject>();

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void StartTutorial(int whichTutorial = 0)
    {
        Debug.Log("Starting Tutorial in Progression MGR");
        tutorialPanel.SetActive(true);
        SetUIElement();
        volumetricPlayer.OnStepChanged.AddListener(SetUIElement);
        //Tutorials[whichTutorial].PlayTutorial();
    }

    public void SetUIElement()
    {
        foreach (GameObject content in contentPanels)
        {
            if (content.activeSelf)
            {
                content.SetActive(false);
            }
        }

        contentPanels[volumetricPlayer.CurStep].SetActive(true);
    }


}
