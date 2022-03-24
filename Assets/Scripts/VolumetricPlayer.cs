//
// playback a volumetric clip
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricPlayer : MonoBehaviour
{

   float _playbackSpeed = 1.0f;
   PlaybackState _playbackState = PlaybackState.Stopped;

   public enum PlaybackState
   {
      Stopped,
      Playing, 
      Paused
   }

}
