//
// playback a volumetric clip
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class VolumetricPlayEvent : UnityEvent<VolumetricPlayer.PlaybackState, VolumetricPlayer.PlaybackState> { } //old state, new state

public class VolumetricPlayer : MonoBehaviour
{
   [Header("Config")]
   public bool PlayAtStart = true;
   public string AnnotationsPath = "";
   public GameObject[] MeshSequence = new GameObject[0];
   public int FPS = 30;
   public float PlaybackSpeed = 1.0f;

   [Header("Debug")]
   [InspectorButton("_DebugPlay")]
   public bool Play;
   [InspectorButton("_DebugPause")]
   public bool Pause;
   [InspectorButton("_DebugStop")]
   public bool Stop;

   //events
   public VolumetricPlayEvent OnPlaybackStateChanged = new VolumetricPlayEvent();


   PlaybackState _playbackState = PlaybackState.Stopped;
   float _lastFrame = 0.0f;

   public enum PlaybackState
   {
      Stopped,
      Playing, 
      Paused
   }

   void Start()
   {
      if (PlayAtStart)
         SetPlaybackState(PlaybackState.Playing);
   }

   public void SetPlaybackState(PlaybackState newState)
   {
      if (_playbackState == newState)
         return;

      var oldState = _playbackState;
      _playbackState = newState;

      //restart clip when going into play state
      if ((oldState == PlaybackState.Stopped) && (newState == PlaybackState.Playing))
         _lastFrame = 0.0f;

      OnPlaybackStateChanged.Invoke(oldState, newState);
   }

   void _ShowFrame(int frameIdx)
   {
      if (frameIdx < 0 || frameIdx >= MeshSequence.Length)
         return;

      for(int i = 0; i < MeshSequence.Length;i++)
      {
         bool active = i == frameIdx;
         if (MeshSequence[i].activeSelf != active)
            MeshSequence[i].SetActive(active);
      }
   }

   void Update()
   {
      if(_playbackState == PlaybackState.Playing)
      {
         _lastFrame += Time.deltaTime * (float)FPS * PlaybackSpeed;
         int curFrameIdx = Mathf.FloorToInt(_lastFrame % MeshSequence.Length);
         _ShowFrame(curFrameIdx);
      }
   }

   void _DebugPlay()
   {
      SetPlaybackState(PlaybackState.Playing);
   }

   void _DebugStop()
   {
      SetPlaybackState(PlaybackState.Stopped);
   }
   void _DebugPause()
   {
      SetPlaybackState(PlaybackState.Paused);
   }
}
