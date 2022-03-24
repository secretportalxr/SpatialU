using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMgr : MonoBehaviour
{
   public CamMode CamType = CamMode.Oculus;
   public GameObject OculusRig;
   public GameObject HololensRig;
   public GameObject ScreenRig;

   public enum CamMode
   {
      Oculus,
      Hololens,
      Screen
   }

   void Start()
   {
      if (OculusRig)
         OculusRig.SetActive(CamType == CamMode.Oculus);
      if (HololensRig)
         OculusRig.SetActive(CamType == CamMode.Hololens);
      if (ScreenRig)
         OculusRig.SetActive(CamType == CamMode.Screen);
   }
}
