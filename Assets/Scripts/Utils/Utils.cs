using System;
using UnityEngine;
public class Utils
{
  public static int ENEMIES = LayerMask.NameToLayer("Enemies");
  public static int MAP = LayerMask.NameToLayer("Map");

  public const float LOOK_AT_IMMEDIATLY = 10000f;

  public static string[] SKINS = { "Santa", "Cowboy", "SuperSanta", "Astronaut", "SuitSanta", "ScienceSanta" };
  public static string[] WEAPONS = { "Candy", "Lollipop", "Katana", "Hammer", "BeamSword" };

  public static bool IsMobile { get => SystemInfo.deviceType == DeviceType.Handheld; }
  public static string key = "MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgGo03+M4iToQpS52C/wvRXYCMlzjTSHvYJecobK1ntvVFGwMlkK8joBcHQ6dtmuV1O6jCZKLubRZM6RzHGotX9gqGrii0zF9fdpZNV2MpMR0+nAz11WT/l+yRQtPnK6bR8OTgCtZYPXxVCPfiQXV6TAA6URA1RXQhdMzQ1u/mPE7AgMBAAE=";
  public static string key2 = "MIGeMA0GCSqGSIb3DSADQW342342SdsxcSEE2XYCMlzjTSHvYJecobK1ntvVFGwMlkK8joBcHQ6dtmuV1O6jCZKLubRZM6RzHGotX9gqGrii0zF9fdpZNV2MpMR0+nAz11WT/l+yRQtPnK6bR8OTgCtZYPXxVCPfiQXV6TAA6URA1RXQhdMzQ1u/mPE7AgMBAAE=";
  public static string _publicKey = "<RSAKeyValue><Modulus>" + key + "</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

  public static bool IsPlayer(Collider other)
  {
    return GameController.Instance.Player != null && other.gameObject == GameController.Instance.Player.gameObject && GameController.Instance.Player.IHit.Alive;
  }
  public static bool IsPlayer(Collision other)
  {
    return GameController.Instance.Player != null && other.gameObject == GameController.Instance.Player.gameObject && GameController.Instance.Player.IHit.Alive;
  }
  public static Quaternion LookAt(Transform self, Transform target, float turnSpeed = LOOK_AT_IMMEDIATLY)
  {
    Vector3 targetDir = target.position - self.position;
    targetDir.y = 0;
    Vector3 newDir = Vector3.RotateTowards(self.forward, targetDir, turnSpeed * Time.deltaTime, .0f);

    return Quaternion.LookRotation(newDir);
  }

  public static Quaternion LookAtAll(Transform self, Transform target, float turnSpeed = LOOK_AT_IMMEDIATLY)
  {
    Vector3 targetDir = target.position - self.position;
    Vector3 newDir = Vector3.RotateTowards(self.forward, targetDir, turnSpeed * Time.deltaTime, .0f);

    return Quaternion.LookRotation(newDir);
  }
  public static Quaternion LookAt(Transform self, Vector3 target, float turnSpeed)
  {
    Vector3 targetDir = target - self.position;
    targetDir.y = 0;
    Vector3 newDir = Vector3.RotateTowards(self.forward, targetDir, turnSpeed * Time.deltaTime, .0f);

    return Quaternion.LookRotation(newDir);
  }

  public static Quaternion LookAtSmooth(Transform self, Vector3 target, float turnSpeed)
  {
    Vector3 targetDir = target - self.position;
    Vector3 newDir = Vector3.RotateTowards(self.forward, targetDir, turnSpeed * Time.deltaTime, .0f);

    return Quaternion.LookRotation(newDir);
  }

  public static Transform RecursiveFindChild(Transform parent, string childName)
  {
    foreach (Transform child in parent)
    {
      if (child.name == childName)
      {
        return child;
      }
      else
      {
        Transform found = RecursiveFindChild(child, childName);
        if (found != null)
        {
          return found;
        }
      }
    }
    return null;
  }

  public static bool HasInternet() => !(Application.internetReachability == NetworkReachability.NotReachable);

  public static string SecondsToScore(int sec)
  {
    int s = sec % 60;
    sec /= 60;
    int mins = sec % 60;
    int hours = sec / 60;
    return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, mins, s);
  }

  public static float Clamp0360(float eulerAngles)
  {
    float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
    if (result < 0)
    {
      result += 360f;
    }
    return result;
  }

}

public delegate void Notify();
