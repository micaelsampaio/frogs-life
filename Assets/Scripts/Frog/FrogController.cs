using UnityEngine;

public class FrogController : MonoBehaviour, Player, IHit
{
  [Header("Components")]
  public Camera Camera;
  public Rigidbody rb;
  public Animator Animator;

  public SoundController SoundController;

  [Header("Tongue")]
  public Transform StartTongue;
  public Transform EndTongue;

  public TongueInteractable TongueInteractable;
  private bool ReturnTongue;

  [Header("Game")]
  public float Speed;
  public float TurnSpeed;

  public float JumpForce;
  public bool IsGrounded;
  public float lastJump;

  // Movement
  private float CurrentSpeed;
  private float TurnSmoothVelocity;
  private float SpeedSmoothTime = 0.1f;
  private float SpeedSmoothVelocity;

  private ACTION Action;
  private Vector2 input;
  private Vector2 inputDirection;
  private Vector3 MovementDirection;

  public bool IsHidden { get; set; }

  public IHit IHit => this;

  public IHitType Type => IHitType.PLAYER;

  public bool Alive { get; set; }
  public bool Controls { get; set; }

  private void OnEnable()
  {
    Controls = true;
    Alive = true;
    rb.isKinematic = false;
    GameController.Instance.Player = this;
    Action = ACTION.IDLE;
    lastJump = -1.0f;
    ResetTongue();
    IsGrounded = true;
    Animator.enabled = false;
    Animator.enabled = true;
  }

  private void Start()
  {
    GameController.Instance.AddEventListener("HidePlayer", () => { IsHidden = true; });
    GameController.Instance.AddEventListener("NoHidePlayer", () => { IsHidden = false; });
  }

  void Update()
  {
    if (!Controls || !Alive) return;

    switch (Action)
    {
      case ACTION.IDLE:
        IdleState();
        break;
      case ACTION.JUMP_MOVE:
        JumpMoveState();
        break;
      case ACTION.TONGUE_EAT:
        TongueEatState();
        break;
      case ACTION.TONGUE_EAT_RETURN:
        TongueEatReturnState();
        break;
    }

    CheckIsGrounded();
  }

  private void FixedUpdate()
  {
    if (!Controls || !Alive) return;
    rb.MovePosition(transform.position + MovementDirection * Time.fixedDeltaTime);
  }

  private void IdleState()
  {
    input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    inputDirection = input.normalized;

    if (inputDirection != Vector2.zero)
    {
      float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
      transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetRotation, ref TurnSmoothVelocity, TurnSpeed);
    }
    else
    {
      MovementDirection = Vector3.zero;
    }

    MoveInput();

    if (Input.GetKey(KeyCode.Space) && Action == ACTION.IDLE && lastJump < Time.timeSinceLevelLoad && IsGrounded && inputDirection != Vector2.zero)
    {
      Jump();
    }

    if (Action == ACTION.IDLE && IsGrounded && Input.GetMouseButtonDown(0))
    {
      CheckTongueAction();
    }
  }

  private void MoveInput()
  {
    float targetSpeed = Speed * inputDirection.magnitude;
    CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, targetSpeed, ref SpeedSmoothVelocity, SpeedSmoothTime);
    MovementDirection = transform.forward * CurrentSpeed;

    float magnitude = inputDirection.magnitude;
    if (magnitude == 0 && Animator.GetFloat("Speed") < 0.05f)
    {
      SpeedSmoothVelocity = 0f;
      CurrentSpeed = 0f;
      Animator.SetFloat("Speed", 0f);
    }
    else
    {
      Animator.SetFloat("Speed", 1f * magnitude, SpeedSmoothTime, Time.deltaTime);
    }
  }

  private void Jump()
  {
    float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
    transform.transform.rotation = Quaternion.Euler(0, targetRotation, 0);

    MovementDirection = transform.forward * Speed;
    rb.velocity += Vector3.up;
    rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    Action = ACTION.JUMP_MOVE;
    lastJump = Time.timeSinceLevelLoad + 0.2f;
    SoundController.PlayRandomSound("frog1,frog2,frog3");
  }

  private void JumpMoveState()
  {
    input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    inputDirection = input.normalized;

    if (inputDirection != Vector2.zero)
    {
      float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
      transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetRotation, ref TurnSmoothVelocity, TurnSpeed * 4f);
      MovementDirection = transform.forward * Speed * 2f;
    }

    if (IsGrounded && rb.velocity.y <= 0f)
    {
      Action = ACTION.IDLE;
      MovementDirection = Vector3.zero;
      TurnSmoothVelocity = 0f;
    }
  }

  private void TongueEatState()
  {
    Vector3 targetPosition = TongueInteractable.transform.position;
    EndTongue.transform.position = Vector3.MoveTowards(EndTongue.transform.position, targetPosition, Time.deltaTime * 30f);

    if (Vector3.Distance(EndTongue.transform.position, targetPosition) < 0.1f)
    {
      Action = ACTION.TONGUE_EAT_RETURN;
      var hitEffect = GameController.Instance.Pools["Hit"].Get();
      hitEffect.transform.position = targetPosition;
      hitEffect.SetActive(true);
    }
  }

  private void TongueEatReturnState()
  {
    Vector3 targetPosition = StartTongue.position;
    EndTongue.transform.position = Vector3.MoveTowards(EndTongue.transform.position, targetPosition, Time.deltaTime * 30f);
    TongueInteractable.transform.position = EndTongue.transform.position;

    if (Vector3.Distance(EndTongue.transform.position, targetPosition) < 0.1f)
    {
      Action = ACTION.IDLE;
      ResetTongue();
      //TongueInteractable.Hide()
    }
  }

  public void CheckIsGrounded()
  {
    IsGrounded = false;
    Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.up * 0.3f, 0.5f);
    foreach (var other in hitColliders)
    {
      if (!other.isTrigger && other.gameObject != gameObject)
      {
        IsGrounded = true;
      }
    }

    Animator.SetBool("Grounded", Action == ACTION.JUMP_MOVE ? false : IsGrounded);
  }

  private void CheckTongueAction()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.up * 0.3f, 5f);
    float distance = float.MaxValue;
    TongueInteractable interactableCandidate = null;
    foreach (var other in hitColliders)
    {
      var interactable = other.GetComponent<TongueInteractable>();
      if (interactable != null)
      {
        var dist = Vector3.Distance(transform.position, interactable.gameObject.transform.position);
        if (dist < distance)
        {
          distance = dist;
          interactableCandidate = interactable;
        }
      }
    }

    if (interactableCandidate != null)
    {
      TongueInteractable = interactableCandidate;
      switch (interactableCandidate.Type)
      {
        case TONGUE_TYPE.EAT:
          StartTongueEat();
          break;
      }
    }
  }

  private void KillTongueInteractable()
  {
    if (TongueInteractable != null)
    {
      var hit = TongueInteractable.GetComponent<IHit>();
      if (hit != null) hit.Hit(IHitType.PLAYER);
    }
  }
  private void StartTongueEat()
  {
    StopMovement();
    Action = ACTION.TONGUE_EAT;
    transform.rotation = Utils.LookAt(transform, TongueInteractable.transform);
    KillTongueInteractable();
    Animator.SetTrigger("Tongue");
    SoundController.PlaySound("Tongue");
  }

  private void StopMovement()
  {
    MovementDirection = Vector3.zero;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position + transform.up * 0.3f, 0.5f);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, 5f);
  }

  private void ResetTongue()
  {
    EndTongue.position = StartTongue.position;
    EndTongue.rotation = StartTongue.rotation;

    if (TongueInteractable != null)
    {
      TongueInteractable.gameObject.SetActive(false);
      TongueInteractable = null;
    }
  }

  public void Stop()
  {
    Controls = false;
    rb.isKinematic = true;
  }

  public bool Hit(IHitType Enemy)
  {
    Alive = false;
    SoundController.PlayRandomSound("frog1,frog2,frog3");
    GameController.Instance.OnPlayerDead();
    ResetTongue();
    Stop();
    return true;
  }

  private enum ACTION { IDLE, PRE_JUMP_MOVE, JUMP_MOVE, TONGUE_EAT, TONGUE_EAT_RETURN };
}
