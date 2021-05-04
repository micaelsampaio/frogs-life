using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

  [Header("Level")]
  public string LevelName;
  public string HintText;

  public GameObject[] LevelObjects;
  public GameObject[] HideObjects;
  public string[] OnEnterLevelEvents;
  public string[] OnExitLevelEvents;

  public Transform CheckPoint;

  public bool DisableOnStart = true;

  public bool Visible = true;

  private void Awake()
  {

  }
  private void Start()
  {
    if (!string.IsNullOrEmpty(LevelName))
    {
      GameController.Instance.AddEventListener("OnEnterLevel_" + LevelName, OnEnterLevel);
      GameController.Instance.AddEventListener("OnExitLevel_" + LevelName, OnExitLevel);
    }

    if (DisableOnStart)
    {
      OnExitLevel();
    }

    if (HideObjects != null)
    {
      foreach (var obj in HideObjects)
      {
        obj.SetActive(false);
      }
    }
  }

  public virtual void OnEnterLevel()
  {
    SetObjectsEnabled(true);

    if (OnEnterLevelEvents.Length > 0)
    {
      foreach (var eventname in OnEnterLevelEvents)
      {
        GameController.Instance.DispatchEvent(eventname);
      }
    }
  }

  public virtual void OnLevelCheckPoint()
  {
    if (GameController.Instance.Player != null)
    {
      GameController.Instance.Player.gameObject.transform.SetParent(null);

      if (CheckPoint != null)
      {
        GameController.Instance.Player.gameObject.transform.position = CheckPoint.position;
        GameController.Instance.Player.gameObject.transform.rotation = CheckPoint.rotation;
      }
    }
    SetObjectsEnabled(false);
    SetObjectsEnabled(true);
  }

  public virtual void OnRestartLevel()
  {
    SetObjectsEnabled(false);
    SetObjectsEnabled(true);
  }

  public virtual void OnExitLevel()
  {
    Visible = false;
    SetObjectsEnabled(false);

    if (OnExitLevelEvents.Length > 0)
    {
      foreach (var eventname in OnExitLevelEvents)
      {
        GameController.Instance.DispatchEvent(eventname);
      }
    }
  }

  public void SetObjectsEnabled(bool state)
  {
    if (LevelObjects.Length > 0)
    {
      foreach (var obj in LevelObjects)
      {
        if (obj != null)
        {
          obj.SetActive(state);
        }
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (Utils.IsPlayer(other))
    {
      GameController.Instance.CurrentLevel = this;

      if (!Visible)
      {
        OnEnterLevel();
      }
    }
  }

  private void OnDrawGizmos()
  {
    if (CheckPoint != null)
    {
      Gizmos.color = Color.white;
      Gizmos.DrawWireSphere(CheckPoint.position, 1f);
      Gizmos.DrawWireSphere(CheckPoint.position, 0.5f);
    }
  }
}
