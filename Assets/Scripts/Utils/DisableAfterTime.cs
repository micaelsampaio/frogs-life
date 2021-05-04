using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
  public float CurrentTime = 0f;
  public float MaxTime;

  public string PoolObjectName;

  private void OnEnable()
  {
    CurrentTime = 0f;
  }

  void Update()
  {
    CurrentTime += Time.deltaTime;
    if (CurrentTime > MaxTime)
    {
      if (string.IsNullOrEmpty(PoolObjectName))
      {
        gameObject.SetActive(false);
      }
      else
      {
        GameController.Instance.Pools[PoolObjectName].Enqueue(gameObject);
      }
    }
  }
}
