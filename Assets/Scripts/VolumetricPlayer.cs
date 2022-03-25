//
// playback a volumetric clip
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[System.Serializable]
public class VolumetricPlayEvent : UnityEvent<VolumetricPlayer.PlaybackState, VolumetricPlayer.PlaybackState> { } //old state, new state

[System.Serializable]
public class VolumetricPlayerStepEvent : UnityEvent<int> { } //new step

public class VolumetricPlayer : MonoBehaviour
{
    [Header("Basic Config")]
    public bool PlayAtStart = true;
    //public string AnnotationsPath = "";
    public SequenceType SeqType = SequenceType.MeshSequence;
    public GameObject[] MeshSequence = new GameObject[0];
    public PlayableDirector Timeline;
    public VolumetricPlayer SequenceToSyncWith;
    public int FPS = 30;
    public float PlaybackSpeed = 1.0f;

    public enum SequenceType
    {
        MeshSequence,
        Timeline,
        SyncWithOtherSequencer
    }


    [Header("Steps")]
    [Tooltip("Set CurStep to -1 if you want to play thru ")]
    public int CurStep = -1;
    public Step[] Steps = new Step[0];


    [System.Serializable]
    public class Step
    {
        public float StartProgress = 0.0f;
        public float EndProgress = 1.0f;
    }

    [Header("Outputs")]
    public int CurFrame = 0;
    public float CurProgress = 0.0f;

    [Header("Debug")]
    [InspectorButton("_DebugPlay")]
    public bool Play;
    [InspectorButton("_DebugPause")]
    public bool Pause;
    [InspectorButton("_DebugStop")]
    public bool Stop;

    [Space(10)]

    [InspectorButton("_DebugStepForward")]
    public bool StepForward;
    [InspectorButton("_DebugStepBackward")]
    public bool StepBackward;

    [Space(10)]

    public Material MaterialToApply;
    [InspectorButton("_DebugApplyMaterial")]
    public bool ApplyMaterial;

    //events
    public VolumetricPlayEvent OnPlaybackStateChanged = new VolumetricPlayEvent();
    public VolumetricPlayerStepEvent OnStepChanged = new VolumetricPlayerStepEvent();


    PlaybackState _playbackState = PlaybackState.Stopped;
    float _lastFrame = 0.0f;
    int _lastFrameIdx = 0;

    float _startTime = 0.0f;

    int _curStepShowing = -1;

    public int GetLastFrameIdx() { return _lastFrameIdx; }

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
        if (SeqType == SequenceType.Timeline)
        {
            //we need to scrub the timeline manually because we want to control loop points and speed ourselves
            Timeline.timeUpdateMode = DirectorUpdateMode.Manual;
        }
    }

    public PlaybackState GetPlaybackState() { return _playbackState; }

    public void SetPlaybackState(PlaybackState newState)
    {
        if (_playbackState == newState)
            return;

        var oldState = _playbackState;
        _playbackState = newState;

        //restart clip when going into play state
        if ((oldState == PlaybackState.Stopped) && (newState == PlaybackState.Playing))
        {
            _startTime = 0.0f;
            _lastFrame = 0.0f;
        }

        if (SeqType == SequenceType.Timeline)
        {
            if (newState == PlaybackState.Playing)
                Timeline.Play();
            else if (newState == PlaybackState.Paused)
                Timeline.Pause();
            else if (newState == PlaybackState.Stopped)
                Timeline.Stop();
        }

        OnPlaybackStateChanged.Invoke(oldState, newState);
    }

    void _ShowFrame(int frameIdx)
    {
        if (frameIdx < 0 || frameIdx >= MeshSequence.Length)
            return;

        CurFrame = frameIdx;
        CurProgress = Mathf.InverseLerp(0, MeshSequence.Length - 1, frameIdx);

        if (GetPlaybackState() == PlaybackState.Paused)
        {
            _lastFrameIdx = frameIdx;
            _lastFrame = (float)frameIdx;
        }

        for (int i = 0; i < MeshSequence.Length; i++)
        {
            bool active = i == frameIdx;
            if (MeshSequence[i].activeSelf != active)
                MeshSequence[i].SetActive(active);
        }
    }

    void Update()
    {
        if (_curStepShowing != CurStep)
        {
            OnStepChanged.Invoke(CurStep);
            _curStepShowing = CurStep;
        }

        if (_playbackState == PlaybackState.Playing)
        {
            if (SeqType == SequenceType.MeshSequence)
            {
                int startFrameIdx = 0;
                int endFrameIdx = MeshSequence.Length - 1;
                if ((CurStep >= 0) && (CurStep < Steps.Length))
                {
                    var stepInfo = Steps[CurStep];
                    startFrameIdx = Mathf.FloorToInt(stepInfo.StartProgress * (float)(MeshSequence.Length - 1));
                    endFrameIdx = Mathf.FloorToInt(stepInfo.EndProgress * (float)(MeshSequence.Length - 1));
                }

                _lastFrame += Time.deltaTime * (float)FPS * PlaybackSpeed;

                int len = Mathf.Max(0, (endFrameIdx - startFrameIdx));
                int curFrameIdx = Mathf.FloorToInt(_lastFrame % len);
                curFrameIdx += startFrameIdx;

                //int curFrameIdx = Mathf.FloorToInt(_lastFrame % MeshSequence.Length);
                _ShowFrame(curFrameIdx);

                _lastFrameIdx = curFrameIdx;
            }
            else if (SeqType == SequenceType.SyncWithOtherSequencer)
            {
                if (SequenceToSyncWith)
                {
                    int frameIdx = SequenceToSyncWith.GetLastFrameIdx();
                    _ShowFrame(frameIdx);
                }
            }
            else if (SeqType == SequenceType.Timeline)
            {
                float curTime = (float)Timeline.time;
                curTime += Time.deltaTime * PlaybackSpeed;
                curTime = (curTime % (float)Timeline.duration); //loop

                Timeline.time = curTime;
            }
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

    void _DebugStepForward()
    {
        SetPlaybackState(PlaybackState.Paused);
        _ShowFrame(_lastFrameIdx + 1);
    }

    void _DebugStepBackward()
    {
        SetPlaybackState(PlaybackState.Paused);
        _ShowFrame(_lastFrameIdx - 1);
    }

    void _DebugApplyMaterial()
    {
        if (!MaterialToApply)
            return;

        List<Renderer> rnds = new List<Renderer>();
        foreach (var m in MeshSequence)
        {
            Renderer r = m.GetComponentInChildren<Renderer>();
            if (r)
                rnds.Add(r);
        }

        foreach (var r in rnds)
        {
            r.material = MaterialToApply;
        }
    }
}
