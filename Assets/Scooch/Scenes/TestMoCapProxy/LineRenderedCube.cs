using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ack. https://docs.unity3d.com/ScriptReference/GL.html
public class LineRenderedCube : MonoBehaviour
{
  public Material material { get { return m_LineMaterial; } }

  [SerializeField]
  private Material m_LineMaterial = null;
  private Vector3[] m_Verts;

  private void CreateLineMaterial()
  {
    if (m_LineMaterial == null)
    {
      // Unity has a built-in shader that is useful for drawing
      // simple colored things.
      Shader shader = Shader.Find("Hidden/Internal-Colored");
      m_LineMaterial = new Material(shader);
      m_LineMaterial.hideFlags = HideFlags.HideAndDontSave;
      // Turn on alpha blending
      m_LineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
      m_LineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
      // Turn backface culling off
      m_LineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
      // Turn off depth writes
      m_LineMaterial.SetInt("_ZWrite", 0);
      // Set color something nice
      m_LineMaterial.color = new Color(1.0f, 0.8f, 0.0f);
    }
  }

  //   b-----c
  // a-----d |
  // | |   | |
  // | f---|-g
  // e-----h
  private void Start()
  {
    var a = new Vector3(-0.5f, 0.5f, -0.5f);
    var b = new Vector3(-0.5f, 0.5f, 0.5f);
    var c = new Vector3(0.5f, 0.5f, 0.5f);
    var d = new Vector3(0.5f, 0.5f, -0.5f);
    var e = new Vector3(-0.5f, -0.5f, -0.5f);
    var f = new Vector3(-0.5f, -0.5f, 0.5f);
    var g = new Vector3(0.5f, -0.5f, 0.5f);
    var h = new Vector3(0.5f, -0.5f, -0.5f);

    m_Verts = new Vector3[]
    {
      a, b, c, d, a, e, h, d, h, g, c, g, f, b, f, e
    };

    CreateLineMaterial();
  }

  private void OnRenderObject()
  {
    m_LineMaterial.SetPass(0);

    GL.PushMatrix();
    // Set transformation matrix for drawing to
    // match our transform
    GL.MultMatrix(transform.localToWorldMatrix);

    // Draw lines
    GL.Begin(GL.LINE_STRIP);
    for (int i = 0; i < m_Verts.Length; ++i)
    {
      GL.Vertex(m_Verts[i]);
    }
    GL.End();
    GL.PopMatrix();
  }
}

