using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SOURCE: TALES OF ELF 

public class SoundController : MonoBehaviour
{
  public AudioSource Audio;
  public List<Sound> Sounds = new List<Sound>();
  public static Dictionary<string, float> AudioQueueTimers = new Dictionary<string, float>();

  public void PlaySound(string clipName)
  {
    var sound = Sounds.Find(s => s.name.Equals(clipName));
    if (sound != null)
    {
      PlaySound(sound);
    }
    else
    {
      string sounds = "";
      Sounds.ForEach(s => sounds += s.name + ", ");
#if UNITY_EDITOR
      Debug.LogWarning($"XXXXXXX   ----> SOUND NOT FOUND \"{clipName}\" - {sounds}");
#endif
    }
  }

  public void PlaySound(string clipName, float delay)
  {
    StartCoroutine(PlaySoundDelay(clipName, delay));
  }

  private IEnumerator PlaySoundDelay(string clipName, float delay)
  {
    yield return new WaitForSeconds(delay);
    PlaySound(clipName);
  }

  public void PlayRandomSound(string clipName)
  {
    var names = clipName.Split(',');
    var name = names[UnityEngine.Random.Range(0, names.Length)];
    var sound = Sounds.Find(s => s.name.Equals(name));
    if (sound != null)
    {
      PlaySound(sound);
    }
    else
    {
#if UNITY_EDITOR
      Debug.LogWarning($"XXXXXXX   ----> RANDOM SOUND NOT FOUND \"{name}\"");
#endif
    }
  }

  public void PlaySound(AudioClip clip, float soundCooldown = 0.2f, float volume = 0.5f, string name = null)
  {
    var clipName = name ?? clip.name;
    var hasKey = AudioQueueTimers.ContainsKey(clipName);
    if (Audio == null) return;
    AudioQueueTimers.TryGetValue(clipName, out float audioInfo2);

    if (hasKey && AudioQueueTimers.TryGetValue(clipName, out float audioInfo) && audioInfo < Time.timeSinceLevelLoad)
    {
      Audio.PlayOneShot(clip, volume);
      AudioQueueTimers[clipName] = Time.timeSinceLevelLoad + soundCooldown;
    }
    else if (!hasKey)
    {
      Audio.PlayOneShot(clip, volume);
      AudioQueueTimers.Add(clipName, Time.timeSinceLevelLoad + soundCooldown);
    }
  }

  public void PlaySound(Sound sound)
  {
    if (Audio == null) return;
    if (sound == null || sound.audioClip == null) return;
    if (string.IsNullOrEmpty(sound.name)) sound.name = sound.audioClip.name;

    var clipName = sound.name;
    var hasKey = AudioQueueTimers.ContainsKey(clipName);

    if (hasKey && AudioQueueTimers.TryGetValue(clipName, out float audioInfo) && (sound.cooldown <= 0f || audioInfo < Time.timeSinceLevelLoad))
    {
      Audio.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
      AudioQueueTimers[clipName] = Time.timeSinceLevelLoad + sound.cooldown;
    }
    else if (!hasKey)
    {
      Audio.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
      AudioQueueTimers.Add(clipName, Time.timeSinceLevelLoad + sound.cooldown);
    }
  }
}

[Serializable]
public class Sound
{
  public AudioClip audioClip;
  public string name;
  [Range(0, 1)]
  public float volume = 0.5f;
  public float cooldown;
}

