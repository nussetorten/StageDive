using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MoCapProxyController : MonoBehaviour
{
  public Animator Animator { get; set; }

  private void Awake()
  {
    Animator = GetComponent<Animator>();
  }

  [ContextMenu("Idle")]
  public void PlayIdle()
  {
    Animator.Play("Idle");
  }

  [ContextMenu("Ready")]
  public void PlayReady()
  {
    Animator.Play("Ready");
  }

  [ContextMenu("Fuss with campfire")]
  public void PlayFussWithCampfire()
  {
    Animator.Play("FussWithCampfire");
  }

  [ContextMenu("Walk across")]
  public void PlayWalkAcross()
  {
    Animator.Play("WalkAcross");
  }

}
