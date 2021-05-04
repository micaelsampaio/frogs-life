using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggletController : MonoBehaviour, IHit, Player
{
  [Header("Game")]
  public Camera Camera;
  public Animator Animator;
  public Rigidbody rb;
  public SoundController SoundController;
  public IHitType type;
  public float Speed = 3f;
  private float turnSmoothVelocity;
  public float turnSpeed;
  public bool Grow = true;
  public int GrowSize;

  private Vector2 input;
  private Vector2 inputDirection;
  private Vector3 MovementDirection;

  [Header("Body")]
  public GameObject Eyes;
  public GameObject Arms;
  public GameObject Legs;

  [Header("Other")]
  private FROGGLET_ACTIONS Action = FROGGLET_ACTIONS.IDLE;
  public bool Alive { get; private set; } = true;

  private void OnEnable()
  {
    Alive = true;
    GameController.Instance.Player = this;
    Action = FROGGLET_ACTIONS.IDLE;
    MovementDirection = Vector3.zero;

    if (Grow)
    {
      transform.localScale = Vector3.one * 0.2f;
      Eyes.SetActive(false);
      Arms.SetActive(false);
      Legs.SetActive(false);
      GrowSize = 6;
    }
    else
    {
      transform.localScale = Vector3.one;
      Eyes.SetActive(true);
      Arms.SetActive(true);
      Legs.SetActive(true);
    }

    GameController.Instance.Player = this;
  }

  private void Start()
  {
    GameController.Instance.Player = this;
  }

  public bool Hit(IHitType other)
  {
    Alive = false;
    GameController.Instance.OnPlayerDead();
    return true;
  }

  // Update is called once per frame
  void Update()
  {
    if (!Alive) return;

    switch (Action)
    {
      case FROGGLET_ACTIONS.IDLE:
        IdleState();
        break;
    }

    GrowState();
  }
  private void FixedUpdate()
  {
    rb.MovePosition(transform.position + MovementDirection * Time.fixedDeltaTime);
  }

  public void IdleState()
  {

    input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    inputDirection = input.normalized;

    if (inputDirection != Vector2.zero)
    {
      float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
      transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSpeed);
    }

    MovementDirection = transform.forward * Speed * transform.localScale.x;
  }

  private void GrowState()
  {
    float growScale = (float)GrowSize / 10;

    if (Grow && growScale > transform.localScale.x)
    {
      transform.localScale += Vector3.one * 0.3f * Time.deltaTime;
      if (transform.localScale.x >= growScale)
      {
        transform.localScale = (Vector3.one * GrowSize) / 10;

        if (GrowSize >= 7)
        {
          Eyes.SetActive(true);
        }
        if (GrowSize >= 8)
        {
          Arms.SetActive(true);
        }
        if (GrowSize >= 9)
        {
          Legs.SetActive(true);
        }
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.name == "Frogglet_Food")
    {
      SoundController.PlayRandomSound("Food1,Food2");
      other.gameObject.SetActive(false);

      if (GrowSize < 10)
      {
        GrowSize += 1;
      }
    }
  }

  public void Stop() { }

  public IHitType Type { get => type; }

  public bool IsHidden => false;

  public IHit IHit => this;

  private enum FROGGLET_ACTIONS { IDLE, DYING };
}

