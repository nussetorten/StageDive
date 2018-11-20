using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class HumanPoseRecorder : MonoBehaviour {

  public Animator target;
  public Transform root;

  private HumanPoseHandler m_SourcePoseHandler;
  private bool m_Recording = false;

  private void Start()
  {
    if (target != null && root != null)
    {
      m_SourcePoseHandler = new HumanPoseHandler(target.avatar, root);
    }
  }

  [ContextMenu("Start recording")]
  public void StartRecording()
  {
    m_Recording = true;
    StartCoroutine(RecordRoutine());
  }

  [ContextMenu("Stop recording")]
  public void StopRecording()
  {
    m_Recording = false;
  }

  private IEnumerator RecordRoutine()
  {
    Debug.Log("Started recording ...");

    List<HumanPose> poses = new List<HumanPose>();
    while (m_Recording)
    {
      HumanPose pose = new HumanPose();
      m_SourcePoseHandler.GetHumanPose(ref pose);
      poses.Add(pose);
      yield return new WaitForEndOfFrame();
    }

    Debug.Log("Stopped recording, writing to file ...");
    List<ElementaryHumanPose> eposes = new List<ElementaryHumanPose>();
    foreach (var pose in poses)
    {
      eposes.Add(new ElementaryHumanPose
      {
        px = pose.bodyPosition.x,
        py = pose.bodyPosition.y,
        pz = pose.bodyPosition.z,
        rx = pose.bodyRotation.x,
        ry = pose.bodyRotation.y,
        rz = pose.bodyRotation.z,
        rw = pose.bodyRotation.w,
        muscles = pose.muscles        
      });
    }
    using (var file = new StreamWriter(System.IO.Path.Combine(Application.dataPath, "recording.hpl")))
    {
      file.WriteLine(JsonConvert.SerializeObject(eposes));
    }

    //using (var file = new StreamWriter(System.IO.Path.Combine(Application.dataPath, "recording.hpl")))
    //{
    //  foreach (var pose in poses)
    //  {
    //    file.WriteLine(string.Format("P {0:0.000} {1:0.000} {2:0.000}",
    //      pose.bodyPosition.x,
    //      pose.bodyPosition.y,
    //      pose.bodyPosition.z
    //    ));
    //    file.WriteLine(string.Format("R {0:0.000} {1:0.000} {2:0.000} {3:0.000}",
    //      pose.bodyRotation.x,
    //      pose.bodyRotation.y,
    //      pose.bodyRotation.z,
    //      pose.bodyRotation.w
    //    ));
    //    file.Write("M");
    //    for (int i = 0; i < pose.muscles.Length; ++i)
    //    {
    //      file.Write(string.Format(" {0:0.0000}", pose.muscles[i]));
    //    }
    //    file.WriteLine();
    //  }
    //}
  }
}

struct ElementaryHumanPose
{
  public float px, py, pz;
  public float rx, ry, rz, rw;
  public float[] muscles;
}