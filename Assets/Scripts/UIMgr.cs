using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    public GameObject volumeSlider;

    public void ToggleVolumeSlider()
    {
        volumeSlider.SetActive(volumeSlider.activeSelf);
    }
    public void SetVolume()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;

    }

    public void SetPlaySpeed()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;
        ProgressionMgr.instance.volumetricPlayer.PlaybackSpeed = 0.5f;
    }

    public void NextTutorialElement()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;
    }
    public void PreviousTutorialElement()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;
    }

    public void TogglePaused()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;

        if (ProgressionMgr.instance.volumetricPlayer.GetPlaybackState() == VolumetricPlayer.PlaybackState.Playing)
        {
            ProgressionMgr.instance.volumetricPlayer.SetPlaybackState(VolumetricPlayer.PlaybackState.Paused);
        }
        if (ProgressionMgr.instance.volumetricPlayer.GetPlaybackState() == VolumetricPlayer.PlaybackState.Paused)
        {
            ProgressionMgr.instance.volumetricPlayer.SetPlaybackState(VolumetricPlayer.PlaybackState.Playing);
        }
    }

    public void Restart()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;
        //ProgressionMgr.instance.volumetricPlayer.SetStep(0);
    }

    public void StartTutorial()
    {
        ProgressionMgr.instance.StartTutorial();
    }
    public void ExitTutorial()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;

    }



    public void TogglePlaythrough()
    {
        if (!ProgressionMgr.instance.volumetricPlayer) return;

    }





}
