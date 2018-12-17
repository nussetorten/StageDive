using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
  public List<Renderer> ManagedRenderers = new List<Renderer>();

  private List<Color> m_StartingColors = new List<Color>();

  [ContextMenu("Fade out (quickly)")]
  public void FadeOutQuickly()
  {
    FadeOut(0.6f);
  }

  [ContextMenu("Fade in (quickly)")]
  public void FadeInQuickly()
  {
    FadeIn(0.6f);
  }


  [ContextMenu("Fade out (slowly)")]
  public void FadeOutSlowly()
  {
    FadeOut(2.0f);
  }

  [ContextMenu("Fade in (slowly)")]
  public void FadeInSlowly()
  {
    FadeIn(2.0f);
  }

  public void FadeOut(float t)
  {
    if (m_StartingColors.Count == 0)
    {
      StartCoroutine(FadeOutRoutine(t));
    }
  }

  public void FadeIn(float t)
  {
    if (m_StartingColors.Count > 0)
    {
      StartCoroutine(FadeInRoutine(t));
    }
  }

  public IEnumerator FadeOutRoutine(float t)
  {
    // Store all original color values
    m_StartingColors.Clear();
    for (int i = 0; i < ManagedRenderers.Count; i++)
    {
      var renderer = ManagedRenderers[i];
      for (int j = 0; j < renderer.materials.Length; j++)
      {
        m_StartingColors.Add(renderer.materials[j].color);

        // And set to transparent render mode
        StandardShaderUtils.ChangeRenderMode(renderer.materials[j], StandardShaderUtils.BlendMode.Fade);
      }
    }
    
    // Lerp to a=zero over t seconds
    var dur = t;
    var now = 0.0f;
    while (now < dur)
    {
      now = now + Time.deltaTime;
      var p = Mathf.Min(1.0f, now / dur);
      for (int i = 0; i < ManagedRenderers.Count; i++)
      {
        var renderer = ManagedRenderers[i];
        for (int j = 0; j < renderer.materials.Length; j++)
        {
          var c = m_StartingColors[i];
          var d = c;
          d.a = Mathf.Lerp(c.a, 0.0f, p);
          ManagedRenderers[i].materials[j].color = d;
        }
      }
      yield return new WaitForEndOfFrame();
    }
  }

  public IEnumerator FadeInRoutine(float t)
  {
    // Sanity check
    if (m_StartingColors.Count == 0) yield return null;
 
    // Lerp to a=original over t seconds
    var dur = t;
    var now = 0.0f;
    while (now < dur)
    {
      now = now + Time.deltaTime;
      var p = Mathf.Min(1.0f, now / dur);
      for (int i = 0; i < ManagedRenderers.Count; i++)
      {
        var renderer = ManagedRenderers[i];
        for (int j = 0; j < renderer.materials.Length; j++)
        {
          var c = m_StartingColors[i];
          var d = c;
          d.a = Mathf.Lerp(0.0f, c.a, p);
          ManagedRenderers[i].materials[j].color = d;
        }
      }
      yield return new WaitForEndOfFrame();
    }

    // Switch to opaque
    for (int i = 0; i < ManagedRenderers.Count; i++)
    {
      var renderer = ManagedRenderers[i];
      for (int j = 0; j < renderer.materials.Length; j++)
      {
        StandardShaderUtils.ChangeRenderMode(renderer.materials[j], StandardShaderUtils.BlendMode.Opaque);
      }
    }

    // Reset state.
    m_StartingColors.Clear();
  }
}

// Ack @bellicapax on http://answers.unity.com/answers/1265884/view.html
public static class StandardShaderUtils
{
  public enum BlendMode
  {
    Opaque,
    Cutout,
    Fade,
    Transparent
  }

  public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
  {
    switch (blendMode)
    {
      case BlendMode.Opaque:
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        standardShaderMaterial.SetInt("_ZWrite", 1);
        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = -1;
        break;
      case BlendMode.Cutout:
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        standardShaderMaterial.SetInt("_ZWrite", 1);
        standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = 2450;
        break;
      case BlendMode.Fade:
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        standardShaderMaterial.SetInt("_ZWrite", 0);
        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = 3000;
        break;
      case BlendMode.Transparent:
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        standardShaderMaterial.SetInt("_ZWrite", 1);
        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = 3000;
        break;
      }
    }
}