using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Transform target;
  public Vector3 cameraOffset;
  public float cameraSpeed = 0.1f;

  void Start()
  {
    transform.position = target.position + cameraOffset;
  }

  //// TRAILER
  //bool newTarget = false;
  //private void Update()
  //{
  //    if (player == null || !player.gameObject.activeSelf)
  //    {
  //        player = GameManager.Instance.Player.transform;
  //        cameraOffset = new Vector3(0, 10, -10);
  //        newTarget = false;
  //    }
  //    if (newTarget) return;

  //    var objs = FindObjectsOfType<ElfPresentCatcher>();
  //    var elf = Array.Find(objs, o => o.enabled);
  //    if (elf != null)
  //    {
  //        newTarget = true;
  //        player = elf.transform;
  //        cameraOffset = new Vector3(0, 4, -4);
  //    }
  //}

  private void LateUpdate()
  {
    if (target != null)
    {
      Vector3 finalPosition = target.position + cameraOffset;
      //Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, cameraSpeed * Time.);
      transform.position = finalPosition;
    }
  }
}