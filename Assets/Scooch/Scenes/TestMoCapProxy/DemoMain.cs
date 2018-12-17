using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageDive;

public class DemoMain : MonoBehaviour
{
  public GameObject MoCapProxy;
  public MoCapProxyController MoCapProxyController;
  public FadeController MoCapProxyFade;
  public Renderer MoCapProxyRenderer;
  public HumanPoseRecorder MoCapRecorder;
  public AnimMultiTrack MoCapPlaybackTrack;
  public AnimClipDatabase MoCapDatabase;
  public AnimTrackPlayback MoCapPlayback;
  public GameObject MoCapPlaybackAvatar;
  public GameObject MoCapPlaybackGui;
  public FadeController CageFade;
  public Transform Cage;
  public StageLightingManager LightingManager;

  public MouseOrbitImproved CameraController;
  public Transform CameraTargetProxy;
  public Transform CameraTargetCenter;
  public Transform CameraTargetPlayback;

  public void Update()
  {
    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
    {
      if (Input.GetKeyDown(KeyCode.Alpha1))
      {
        MoCapProxyController.PlayIdle();
      }
      if (Input.GetKeyDown(KeyCode.Alpha2))
      {
        MoCapProxyController.PlayReady();
      }
      if (Input.GetKeyDown(KeyCode.Alpha3))
      {
        MoCapProxyController.PlayFussWithCampfire();
      }
      if (Input.GetKeyDown(KeyCode.Alpha4))
      {
        MoCapProxyController.PlayWalkAcross();
      }
    }
    else
    {
      // Stage and lighting
      if (Input.GetKeyDown(KeyCode.PageDown))
      {
        StartCoroutine(LightingManager.EaseBetweenLightingProfiles(
          LightingManager.CurrentProfile,
          StageLightingManager.ProfileName.CloseQuarters,
          3.0f
        ));
        StartCoroutine(CageFade.FadeInRoutine(2.0f));
      }
      if (Input.GetKeyDown(KeyCode.PageUp))
      {
        StartCoroutine(LightingManager.EaseBetweenLightingProfiles(
          LightingManager.CurrentProfile,
          StageLightingManager.ProfileName.OpenSpaces,
          3.0f
        ));
        StartCoroutine(CageFade.FadeOutRoutine(2.0f));
      }

      // Camera
      if (Input.GetKeyDown(KeyCode.C))
      {
        if (CameraController.target == CameraTargetCenter)
        {
          CameraController.target = CameraTargetProxy;
        }
        else if (CameraController.target == CameraTargetProxy)
        {
          CameraController.target = CameraTargetPlayback;
        }
        else
        {
          CameraController.target = CameraTargetCenter;
        }
      }

      // Recording
      if (Input.GetKeyDown(KeyCode.R))
      {
        if (MoCapProxyRenderer.material.color == Color.white)
        {
          // Start recording
          MoCapProxyRenderer.material.color = Color.red; // highlight body red
          MoCapRecorder.StartRecording();
        }
        else
        {
          // Stop recording
          StartCoroutine(Action_StopRecording());
        }
      }
      if (Input.GetKeyDown(KeyCode.T))
      {
        StartCoroutine(Action_StopRecording());
      }
      if (Input.GetKeyDown(KeyCode.V))
      {
        MoCapPlaybackTrack.OnValidate();
      }

      // Playback
      if (Input.GetKeyDown(KeyCode.P))
      {
        if (MoCapPlaybackAvatar.activeSelf)
        {
          // Deactivate
          MoCapPlaybackAvatar.SetActive(false);
          MoCapPlaybackGui.SetActive(false);
        }
        else
        {
          // Activate
          MoCapPlaybackAvatar.SetActive(true);
          MoCapPlaybackGui.SetActive(true);
        }
      }

      // Reposition proxy
      if (Input.GetKeyDown(KeyCode.Alpha1))
      {
        StartCoroutine(Action_RepositionCenter());
      }
      if (Input.GetKeyDown(KeyCode.Alpha2))
      {
        StartCoroutine(Action_RepositionRight());
      }
      if (Input.GetKeyDown(KeyCode.Alpha3))
      {
        StartCoroutine(Action_RepositionBackRightCorner());
      }
      if (Input.GetKeyDown(KeyCode.Alpha4))
      {
        StartCoroutine(Action_RepositionBack());
      }
      if (Input.GetKeyDown(KeyCode.Alpha5))
      {
        StartCoroutine(Action_RepositionLeft());
      }
    }
  }

  private IEnumerator Action_RepositionCenter()
  {
    // Fade out
    yield return StartCoroutine(MoCapProxyFade.FadeOutRoutine(1.0f));

    MoCapProxy.transform.position = Vector3.zero;
    MoCapProxy.transform.rotation = Quaternion.identity;
    MoCapProxyController.PlayIdle();
    
    // Fade in
    yield return StartCoroutine(MoCapProxyFade.FadeInRoutine(1.0f));
  }

  private IEnumerator Action_RepositionRight()
  {
    // Fade out
    yield return StartCoroutine(MoCapProxyFade.FadeOutRoutine(1.0f));

    MoCapProxy.transform.position = new Vector3(-1.5f, 0.0f, 0.0f);
    MoCapProxy.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
    MoCapProxyController.PlayIdle();
    
    // Fade in
    yield return StartCoroutine(MoCapProxyFade.FadeInRoutine(1.0f));
  }

  private IEnumerator Action_RepositionLeft()
  {
    // Fade out
    yield return StartCoroutine(MoCapProxyFade.FadeOutRoutine(1.0f));

    MoCapProxy.transform.position = new Vector3(1.5f, 0.0f, 0.0f);
    MoCapProxy.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
    MoCapProxyController.PlayIdle();
    
    // Fade in
    yield return StartCoroutine(MoCapProxyFade.FadeInRoutine(1.0f));
  }

  private IEnumerator Action_RepositionBack()
  {
    // Fade out
    yield return StartCoroutine(MoCapProxyFade.FadeOutRoutine(1.0f));

    MoCapProxy.transform.position = new Vector3(0.0f, 0.0f, -1.5f);
    MoCapProxy.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    MoCapProxyController.PlayIdle();
    
    // Fade in
    yield return StartCoroutine(MoCapProxyFade.FadeInRoutine(1.0f));
  }

  private IEnumerator Action_RepositionBackRightCorner()
  {
    // Fade out
    yield return StartCoroutine(MoCapProxyFade.FadeOutRoutine(1.0f));

    MoCapProxy.transform.position = new Vector3(-1.5f, 0.0f, -1.5f);
    MoCapProxy.transform.rotation = Quaternion.Euler(0.0f, 45.0f, 0.0f);
    MoCapProxyController.PlayIdle();
    
    // Fade in
    yield return StartCoroutine(MoCapProxyFade.FadeInRoutine(1.0f));
  }

  private IEnumerator Action_StopRecording()
  {
    MoCapProxyRenderer.material.color = Color.white; // remove highlight
    MoCapRecorder.StopRecording();

    // Stop is asynchronous; wait a few frames.
    yield return new WaitForSeconds(0.5f);
    
    // Instantiate playback track.
    var n = MoCapDatabase.AnimClipNames.Length;
    var name = MoCapDatabase.AnimClipNames[n - 1];
    var gobj = new GameObject("track_" + name);
    var track = gobj.AddComponent<AnimTrack>();
    track.animClip = MoCapDatabase.GetAnimClip(name);

    // Register with multi-track.
    MoCapPlaybackTrack.AddTrack(track);
  }
}