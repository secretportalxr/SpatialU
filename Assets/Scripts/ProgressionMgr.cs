using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionMgr : MonoBehaviour
{
    public static ProgressionMgr instance;

    [Header("Data Objects")]
    public List<TutorialData> Tutorials = new List<TutorialData>();
    public Transform spawnPoint;

    [Header("Tutorial Objects")]
    public VolumetricPlayer volumetricPlayer;


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
        Tutorials[whichTutorial].PlayTutorial();
    }


}
