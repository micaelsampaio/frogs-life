using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishUnderWater : MonoBehaviour
{
  public Animator Animator;
  public bool Kill = false;
  public bool Down = false;

  private float Grav;
  private Vector3 StartPosition = Vector3.zero;

  public SoundController SoundController;

  private void OnEnable()
  {
    Kill = false;
    Down = false;

    if (StartPosition != Vector3.zero)
    {
      transform.position = StartPosition;
    }
    transform.rotation = Quaternion.Euler(0, 0, 0);

    Animator.SetBool("Attack", true);
  }

  private void Start()
  {
    StartPosition = transform.position;
  }

  private void FixedUpdate()
  {
    if (Kill)
    {
      var pos = GameController.Instance.Player.gameObject.transform.position;
      transform.position = new Vector3(pos.x, transform.position.y + Time.fixedDeltaTime * 30f, pos.z);

      if (transform.position.y > GameController.Instance.Player.gameObject.transform.position.y)
      {
        Grav = 15;
        Kill = false;
        Down = true;
        GameController.Instance.Player.IHit.Hit(IHitType.ENEMY);
        GameController.Instance.Player.gameObject.transform.SetParent(transform);

        var clone = GameController.Instance.GetPool("WaterSplash").Get();
        clone.transform.position = transform.position;
        clone.SetActive(true);

        SoundController.PlaySound("BigSplash");
        SoundController.PlaySound("Bite");
      }
    }

    if (Down)
    {
      var desiredRotQ = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
      transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * 5f);

      transform.position += Vector3.up * Grav * Time.fixedDeltaTime;
      if (Grav > -25f)
      {
        Grav -= Time.fixedDeltaTime * 20f;
      }
    }
  }


  private void OnTriggerEnter(Collider other)
  {
    if (!Kill && Utils.IsPlayer(other))
    {
      transform.rotation = Quaternion.Euler(270, -90, 0);
      Kill = true;
    }
  }
}
