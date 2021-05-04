using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJumpSplash : MonoBehaviour
{
  private void OnCollisionEnter(Collision other)
  {
    if (Utils.IsPlayer(other))
    {
      var clone = GameController.Instance.Pools["WaterSplash"].Get();
      clone.transform.position = other.transform.position;
      clone.SetActive(true);
    }
  }
}
