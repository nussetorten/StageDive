using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndicator : MonoBehaviour
{
  private LineRenderer m_HaloRenderer = null;
  private MeshRenderer m_DotRenderer = null;

  public Color runningColor = Color.green;
  public Color resetColor = Color.cyan;

  [Range(0, 1)]
  public float t = 0;

  [SerializeField]
  private Material m_Material;

  private void Start()
  {
    if (m_HaloRenderer == null)
    {
      // Construct a parameterized circle in the xy-plane
      const int n = 18;
      Vector3[] verts = new Vector3[n];
      for (int i = 0; i < n; i++)
      {
        int j = i % (n - 2); // wrap around
        float t = (2.0f * Mathf.PI * j) / (n - 2);
        verts[i] = new Vector3(Mathf.Sin(t), Mathf.Cos(t), 0.0f);
      }

      // Instantiate line renderer
      var gobj = new GameObject("Halo");
      gobj.transform.parent = transform;
      gobj.transform.localPosition = Vector3.zero;
      gobj.transform.localRotation = Quaternion.identity;
      gobj.transform.localScale = Vector3.one;
      m_HaloRenderer = gobj.AddComponent<LineRenderer>();
      m_HaloRenderer.useWorldSpace = false;
      m_HaloRenderer.material = m_Material;
      m_HaloRenderer.widthCurve = new AnimationCurve(new Keyframe(0.0f, 0.01f));
      m_HaloRenderer.positionCount = n;
      m_HaloRenderer.SetPositions(verts);
    }

    if (m_DotRenderer == null)
    {
      // Instantiate a sphere
      var gobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      gobj.transform.parent = transform;
      gobj.transform.localRotation = Quaternion.identity;
      gobj.transform.localScale = Vector3.one * 0.4f;
      m_DotRenderer = gobj.GetComponent<MeshRenderer>();
      m_DotRenderer.material = m_Material;
    }

    StartCoroutine(AnimNoisyStatusIndicator());
  }

  private void Update()
  {
    var u = 2.0f * Mathf.PI * t;
    var p = new Vector3(Mathf.Sin(u), Mathf.Cos(u), 0.0f);
    m_DotRenderer.transform.localPosition = p;
  }

  private IEnumerator AnimNoisyStatusIndicator()
  {
    while (true)
    {
      // Set to running color
      m_DotRenderer.material.color = runningColor;
      m_HaloRenderer.material.color = runningColor;
      // Ease clockwise incrementally (exponential)
      while (t < 0.999f)
      {
        var src = t;
        var dst = Mathf.Min(src + Random.Range(0.1f, 0.5f), 1.0f);
        var now = 0.0f;
        var dur = Random.Range(1.0f, 4.0f);
        Debug.Log(dst);
        while (now < dur)
        {
          now = Mathf.Min(now + Time.deltaTime, dur);
          t = Easing.ExpoOut(now, src, dst, dur);
          yield return new WaitForEndOfFrame();
        }
      }
      // Set to ready color
      m_DotRenderer.material.color = resetColor;
      m_HaloRenderer.material.color = resetColor;
      {
        //  Ease linearly back to zero position
        var dur = 3.0f;
        var src = 1.0f;
        var dst = 0.0f;
        var now = 0.0f;
        while (t > 0.0f)
        {
          now = Mathf.Min(now + Time.deltaTime, dur);
          t = Easing.Linear(now, src, dst, dur);
          yield return new WaitForEndOfFrame();
        }
      }
      // Repeat (yield to avoid while(true){} on accident ...)
      yield return new WaitForEndOfFrame();
    }
  }
}
 
public static class Easing
{

  public static float ExpoOut(float t, float b, float e, float d)
  {
    t = Mathf.Clamp(t, 0, d);
    return (e - b) * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
  }

  public static float Linear(float t, float b, float e, float d)
  {
    t = Mathf.Clamp(t, 0, d);
    return Mathf.Lerp(b, e, t / d);
  }
}