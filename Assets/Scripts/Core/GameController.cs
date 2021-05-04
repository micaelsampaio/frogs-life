using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
  private static GameController _instance;

  public static GameController Instance
  {
    get
    {
      if (_instance == null)
      {
        _instance = FindObjectOfType<GameController>();
        if (_instance != null)
        {
          _instance.Init();
        }
      }

      return _instance;
    }
    private set
    {
      _instance = value;
    }
  }
  public CameraController CameraController;

  public Player Player;

  // public LevelController LevelController;

  // public PlayerController Player;
  // public PlayerUI PlayerUI;
  // public EndGameUI EndGameUI;
  public SoundController SoundController;

  public GameUi GameUi;

  public Level CurrentLevel;
  public List<Level> CurrentActiveLevels;

  private Dictionary<string, List<Action>> Events;

  private bool Started = false;

  [Header("Pool Objects")]
  [Space(10)]
  public List<Pool> PoolObjects;
  public Dictionary<string, PoolObjects> Pools;

  private void OnEnable()
  {

    if (!Started)
    {
      Init();
    }
  }

  public void Awake()
  {
    if (!Started)
    {
      Init();
    }
  }

  public void Init()
  {
    if (Started) return;
    Started = true;
    Events = new Dictionary<string, List<Action>>();
    CurrentActiveLevels = new List<Level>();
    Pools = new Dictionary<string, PoolObjects>();

    if (PoolObjects != null)
    {
      foreach (var pool in PoolObjects)
      {
        RegisterPool(pool);
      }
    }

    GameUi = Instantiate<GameUi>(GameUi);
  }

  public PoolObjects RegisterPool(Pool p)
  {
    if (Pools.TryGetValue(p.key, out PoolObjects nPool))
    {
      nPool.Add(p);
      return nPool;
    }
    else
    {
      Pools[p.key] = new PoolObjects(p);
      return Pools[p.key];
    }
  }

  public PoolObjects GetPool(Pool p)
  {
    Pools.TryGetValue(p.key, out PoolObjects nPool);
    return nPool;
  }

  public PoolObjects GetPool(string key)
  {
    Pools.TryGetValue(key, out PoolObjects nPool);
    return nPool;
  }


  public void AddEventListener(string eventName, Action cb)
  {
    if (!Events.ContainsKey(eventName))
    {
      var list = new List<Action>();
      list.Add(cb);
      Events.Add(eventName, list);
    }
    else
    {
      Events[eventName].Add(cb);
    }
  }

  public void RemoveEventListener(string eventName, Action cb)
  {
    if (Events.ContainsKey(eventName))
    {
      Events[eventName].RemoveAll(evt => evt == cb);

      if (Events[eventName].Count == 0)
      {
        Events.Remove(eventName);
      }
    }
  }

  public void DispatchEvent(string eventName)
  {
    if (Events.TryGetValue(eventName, out var list))
    {
      list.ForEach(cb =>
      {
        cb.Invoke();
      });
    }
  }

  public void AddNewLevel(Level newLevel)
  {
    if (!CurrentActiveLevels.Contains(newLevel))
    {
      CurrentActiveLevels.Add(newLevel);
    }
  }

  public void OnPlayerDead()
  {
    DispatchEvent("OnPlayerDead");


    StartCoroutine(PlayerDeadRoutine());


    DispatchEvent("OnPlayerDeadStart");

  }

  public IEnumerator PlayerDeadRoutine()
  {

    CameraController.target = null;

    yield return GameUi.ShowUIDead(CurrentLevel?.HintText);

    yield return new WaitForSeconds(2.5f);
    // FADE IN SOME UI

    // YOU DIE

    // RESET
    Player.gameObject.transform.parent = null;
    CameraController.target = Player.gameObject.transform;

    CurrentLevel?.OnLevelCheckPoint();

    Player.gameObject.SetActive(false);
    Player.gameObject.SetActive(true);

    yield return GameUi.HideUIDead();

    // foreach (var level in CurrentActiveLevels)
    // {
    //   level.OnRestartLevel();
    // }
  }

  public void Update()
  {
    if (Input.GetKey(KeyCode.Escape))
    {
      SceneManager.LoadScene("Menu");
    }
  }
}
