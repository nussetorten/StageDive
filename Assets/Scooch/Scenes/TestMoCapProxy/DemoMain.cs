using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMain : MonoBehaviour
{
  public GameObject MoCapProxy;
  public MoCapProxyController MoCapProxyController;
  public FadeController MoCapProxyFade;
  public StageLightingManager LightingManager;

  public MouseOrbitImproved CameraController;
  public Transform CameraTargetProxy;
  public Transform CameraTargetCenter;

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
      // Lighting
      if (Input.GetKeyDown(KeyCode.PageDown))
      {
        StartCoroutine(LightingManager.EaseBetweenLightingProfiles(
          LightingManager.CurrentProfile,
          StageLightingManager.ProfileName.CloseQuarters,
          3.0f
        ));
      }
      if (Input.GetKeyDown(KeyCode.PageUp))
      {
        StartCoroutine(LightingManager.EaseBetweenLightingProfiles(
          LightingManager.CurrentProfile,
          StageLightingManager.ProfileName.OpenSpaces,
          3.0f
        ));
      }

      // Camera
      if (Input.GetKeyDown(KeyCode.C))
      {
        if (CameraController.target == CameraTargetCenter)
        {
          CameraController.target = CameraTargetProxy;
        }
        else
        {
          CameraController.target = CameraTargetCenter;
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

}