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
}
