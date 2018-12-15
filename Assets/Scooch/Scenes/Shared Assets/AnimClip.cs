using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  public struct HumanPose
  {
    public float px, py, pz;
    public float rx, ry, rz, rw;
    public float[] muscles;
  }

  /// <summary>
  /// Represents an animation keyframe (a time + a pose)
  /// </summary>
  public struct AnimFrame
  {
    public float time;
    public HumanPose pose;
  }

  /// <summary>
  /// Represents a full animation clip (a list of keyframes)
  /// </summary>
  public class AnimClip
  {
    public AnimClip()
    {
      frames = new List<AnimFrame>();
    }

    public List<AnimFrame> frames;
  }
}