using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace StageDive
{
  /// <summary>
  /// A database of animation clips.
  /// </summary>
  public class AnimClipDatabase : MonoBehaviour
  {
    public string DatabaseDirectory
    {
      get { return Path.Combine(Application.dataPath, "AnimClipDatabase"); }
    }

    public string[] AnimClipNames
    {
      get { return m_AnimClips.Keys.ToArray(); }
    }

    private void Awake()
    {
      // Load from disk
      if (Directory.Exists(DatabaseDirectory))
      {
        var filenames = Directory.GetFiles(DatabaseDirectory);
        foreach (var filename in filenames)
        {
          // Skip non-JSON files
          if (Path.GetExtension(filename) != ".json") continue;

          Debug.Log("AnimClipDatabase loading " + filename);
          var key = Path.GetFileNameWithoutExtension(filename);
          using (var file = new StreamReader(filename))
          {
            var clip = JsonConvert.DeserializeObject<AnimClip>(file.ReadToEnd());
            m_AnimClips[key] = clip;
          }
        }
      }
      else
      {
        Debug.Log("AnimClipDatabase database directory does not exist, zero animations loaded.");
      }
    }

    private void OnDestroy()
    {
      // Serialize to disk
      if (!Directory.Exists(DatabaseDirectory))
      {
        Debug.Log("AnimClipDatabase creating database directory at " + DatabaseDirectory);
        Directory.CreateDirectory(DatabaseDirectory);
      }
      foreach (var clip in m_AnimClips)
      {
        var filename = Path.Combine(DatabaseDirectory, clip.Key + ".json");
        Debug.Log("AnimClipDatabase saving " + clip.Key + " to " + filename);
        using (var file = new StreamWriter(filename))
        {
          file.Write(JsonConvert.SerializeObject(clip.Value));
        }
      }
    }

    public void AddAnimClip(string name, AnimClip clip)
    {
      m_AnimClips.Add(name, clip);
    }

    public AnimClip GetAnimClip(string name)
    {
      return m_AnimClips[name];
    }

    public AnimClip GetAnimClipAt(int index)
    {
      return m_AnimClips[AnimClipNames[index]];
    }

    Dictionary<string, AnimClip> m_AnimClips = new Dictionary<string, AnimClip>();
  }
}