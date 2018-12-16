using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class HumanPosePlayback : MonoBehaviour {

  public Avatar avatar;
  public Transform target;
  public List<float> timestamps = new List<float>();
  public List<HumanPose> poses = new List<HumanPose>();

  private Vector3[] linepos;
  private Texture2D linetex;

  public bool useScrub = false;
  [Range(0, 1)]
  public float scrub = -1f;

  [SerializeField]
  private StageDive.AnimClipDatabase m_AnimClipDatabase;
  private HumanPoseHandler m_PoseHandler;

  private LineRenderer lineRenderer { get { return GetComponent<LineRenderer>(); } }

  private void Start()
  {
    m_PoseHandler = new HumanPoseHandler(avatar, target);

    // DEBUG load first clip.
    var clip = m_AnimClipDatabase.GetAnimClipAt(0);
    foreach (var frame in clip.frames)
    {
      var t = frame.time;
      var p = frame.pose;
      poses.Add(new HumanPose
      {
        bodyPosition = new Vector3(p.px, p.py, p.pz),
        bodyRotation = new Quaternion(p.rx, p.ry, p.rz, p.rw),
        muscles = (float[])p.muscles.Clone()
      });

      timestamps.Add(t);
    }

    linepos = new Vector3[poses.Count];
    linetex = new Texture2D(poses.Count, 1, TextureFormat.RGB24, false);

    for (int i = 0; i < poses.Count; i++)
    {
      linepos[i] = poses[i].bodyPosition;
      linetex.SetPixel(i, 0, Color.green);
    }
    linetex.Apply();
    lineRenderer.positionCount = poses.Count;
    lineRenderer.SetPositions(linepos);
    lineRenderer.material = new Material(Shader.Find("Self-Illumin/Diffuse"));
    lineRenderer.material.mainTexture = linetex;
  }

  private void Update()
  {
    float endt = timestamps[timestamps.Count - 1];
    int n = (int)(Time.time / endt);

    float time;
    if (!useScrub)
    {
      time = Time.time - (n * endt);
      scrub = time / endt;
    }
    else
    {
      time = endt * scrub;
    }

    int j = 1;
    for (; timestamps[j] < time; ++j) { }
    int i = j - 1;

    float t = (time - timestamps[i]) / (timestamps[j] - timestamps[i]);
    HumanPose a = poses[i];
    HumanPose b = poses[j];
    float[] muscles = new float[a.muscles.Length];
    for (int k = 0; k < a.muscles.Length; k++)
    {
      muscles[k] = Mathf.Lerp(a.muscles[k], b.muscles[k], t);
    }
    float[] err = new float[poses.Count];
    float max = -1;
    for (int k = 0; k < poses.Count; k++)
    {
      err[k] = 0.0f;
      for (int l = 0; l < muscles.Length; l = l + 1)
      {
        err[k] += Mathf.Abs(muscles[l] - poses[k].muscles[l]);
      }
      max = Mathf.Max(max, err[k]);
    }
    for (int k = 0; k < poses.Count; k++)
    {
      err[k] /= max;
      if (err[k] < 0.3f)
      {
        float tt = err[k] / 0.3f;
        linetex.SetPixel(k, 0, Color.Lerp(Color.green, Color.yellow, tt));
      }
      else
      {
        float tt = (err[k] - 0.3f) / 0.5f;  // may be >1 
        linetex.SetPixel(k, 0, Color.Lerp(Color.yellow, Color.red, tt));//err[k]));
      }
    }
    linetex.Apply();
    HumanPose c = new HumanPose()
    {
      bodyPosition = Vector3.Lerp(a.bodyPosition, b.bodyPosition, t),
      bodyRotation = Quaternion.Lerp(a.bodyRotation, b.bodyRotation, t),
      muscles = muscles
    };
    m_PoseHandler.SetHumanPose(ref c);
  }
}
