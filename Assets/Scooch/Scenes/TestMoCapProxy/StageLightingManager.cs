using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

/// <summary>
/// Manages stage lighting, tweens between states.
/// </summary>
public class StageLightingManager : MonoBehaviour
{
  /// <summary>
  /// Parametization of a lighted environment.
  /// </summary>
  [System.Serializable]
  public struct LightingProfile
  {
    public FogMode fogMode;
    public float fogDensity;
    public Color backgroundColor;
    public float spotAngle;
  }

  /// <summary>
  /// The known (pre-baked) lighting profiles.
  /// </summary>
  public enum ProfileName
  {
    CloseQuarters,
    OpenSpaces
  }


  /// <summary>
  /// A serializable (i.e. can see in Unity Editor) dictionary of lighting profiles.
  /// </summary>
  [System.Serializable]
  private class LightingDictionary : SerializableDictionaryBase<ProfileName, LightingProfile> { }

  /// <summary>
  /// Pre-baked with known lighting profiles.
  /// </summary>
  [SerializeField]
  private LightingDictionary m_LightingProfiles = new LightingDictionary();

  private void Reset()
  {
    m_LightingProfiles = new LightingDictionary()
    {
      {
        ProfileName.CloseQuarters,
        new LightingProfile()
        {
          fogMode = FogMode.Exponential,
          fogDensity = 0.1f,
          backgroundColor = new Color32(20, 25, 30, 255),
          spotAngle = 125.0f
        }
      },
      {
        ProfileName.OpenSpaces,
        new LightingProfile()
        {
          fogMode = FogMode.Exponential,
          fogDensity = 0.05f,
          backgroundColor = new Color32(200, 200, 200, 255),
          spotAngle = 180.0f
        }
      }
    };
  }

  public ProfileName CurrentProfile { get; private set; }
  public ProfileName StartingProfile = ProfileName.CloseQuarters;

  /// <summary>
  /// Lights managed by changes in lighting profiles.
  /// </summary>
  public List<Light> ManagedLights = new List<Light>();

  private void Start()
  {
    // Switch to starting profile (really, really fast).
    StartCoroutine(EaseBetweenLightingProfiles(StartingProfile, StartingProfile, 0.0001f));
  }

  /// <summary>
  /// Eases to named lighting profile in t seconds.
  /// </summary>
  /// <param name="name">A profile name.</param>
  /// <param name="t">A time in seconds.</param>
  public void SetLightingProfile(ProfileName name, float t)
  {
    // Launch co-routine to ease between profiles.
    StartCoroutine(EaseBetweenLightingProfiles(CurrentProfile, name, t));
  }

  public IEnumerator EaseBetweenLightingProfiles(ProfileName a, ProfileName b, float t)
  {
    // Retrieve lighting profiles
    var src = m_LightingProfiles[a];
    var dst = m_LightingProfiles[b];

    // Lerp over t seconds
    var now = 0.0f;
    var dur = t;
    while (now < dur)
    {
      now = Mathf.Min(t, now + Time.deltaTime);
      float f = Mathf.Clamp01(now / dur); // interpolation parameter

      // Apply render settings
      RenderSettings.fog = true;
      RenderSettings.fogMode = dst.fogMode;
      RenderSettings.fogDensity = Mathf.Lerp(src.fogDensity, dst.fogDensity, f);
      RenderSettings.fogColor = Color.Lerp(src.backgroundColor, dst.backgroundColor, f);
      
      // Apply mainCamera settings
      Camera.main.backgroundColor = Color.Lerp(src.backgroundColor, dst.backgroundColor, f);

      // Apply light settings
      foreach (var light in ManagedLights)
      {
        if (light.type == LightType.Spot)
        {
          light.spotAngle = Mathf.Lerp(src.spotAngle, dst.spotAngle, f);
        }
      }

      // Wait for next frame
      yield return new WaitForEndOfFrame();
    }

    // Update current profile
    CurrentProfile = b;
  }

  [ContextMenu("Switch to CloseQuarters")]
  private void SetLightingProfile_CloseQuarters()
  {
    SetLightingProfile(ProfileName.CloseQuarters, 3.0f);
  }

  [ContextMenu("Switch to OpenSpaces")]
  private void SetLightingProfile_OpenSpaces()
  {
    SetLightingProfile(ProfileName.OpenSpaces, 3.0f);
  }
}
