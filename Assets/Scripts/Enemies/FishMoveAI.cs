using UnityEngine;

public class FishMoveAI : MonoBehaviour
{
  public Animator Animator;
  public float Speed = 5f;
  public Transform[] Positions;
  public Vector3 TargetPosition;
  public int CurrentPosition;
  private ACTION Action;

  private Vector3 StartPosition = Vector3.zero;
  private Vector3 MoveDirection;
  private Quaternion StartRotation;

  public SoundController SoundController;

  private IHit Target;

  private bool SplashSound;

  private void OnEnable()
  {
    if (StartPosition != Vector3.zero)
    {
      transform.position = StartPosition;
      transform.rotation = StartRotation;
    }

    Target = null;
    Action = ACTION.IDLE;
    CurrentPosition = 0;
    TargetPosition = new Vector3(Positions[CurrentPosition].position.x, transform.position.y, Positions[CurrentPosition].position.z);
    MoveDirection = Vector3.zero;

    SplashSound = false;
    Animator.Play("Swim");
  }

  private void OnDisable()
  {
    transform.position = StartPosition;
    transform.rotation = StartRotation;
  }

  private void Start()
  {
    StartPosition = transform.position;
    StartRotation = transform.rotation;
  }

  // Update is called once per frame
  void Update()
  {
    switch (Action)
    {
      case ACTION.IDLE:
        IdleState();
        break;
      case ACTION.GO_BACK:
        GoBackState();
        break;
      case ACTION.ATTACK:
        AttackState();
        break;
      case ACTION.POS_ATTACK:
        PosAttackState();
        break;
    }
  }

  private void IdleState()
  {
    transform.rotation = Utils.LookAt(transform, TargetPosition, 10f);

    if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
    {
      if (++CurrentPosition >= Positions.Length)
      {
        CurrentPosition = 0;
      }

      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, transform.position.y, Positions[CurrentPosition].position.z);
    }
  }
  private void GoBackState()
  {
    transform.rotation = Utils.LookAtSmooth(transform, TargetPosition, 20f);

    if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
    {
      Action = ACTION.IDLE;
      transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, transform.position.y, Positions[CurrentPosition].position.z);
    }
  }

  private void AttackState()
  {
    var pos = GameController.Instance.Player.gameObject.transform.position;
    MoveDirection = transform.forward * Speed * 2;
    transform.rotation = Utils.LookAtSmooth(transform, pos, 30f);

    if (Target == null || !Target.Alive)
    {
      Target = null;
      Action = ACTION.GO_BACK;
      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, StartPosition.y, Positions[CurrentPosition].position.z);
      return;
    }

    if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < 0.15f)
    {
      Animator.SetTrigger("Attack");
      Target.Hit(IHitType.ENEMY);
      Target.gameObject.transform.SetParent(transform);
      Action = ACTION.POS_ATTACK;

      var clone = GameController.Instance.GetPool("WaterSplash").Get();
      clone.transform.position = transform.position;
      clone.SetActive(true);

      SoundController.PlaySound("BigSplash");
      SoundController.PlaySound("Bite");
      SplashSound = false;
    }
  }

  private void PosAttackState()
  {
    MoveDirection = transform.forward * Speed * 2f;
    var desiredRotQ = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * 5f);

    if (transform.position.y < -6f)
    {
      Target = null;
      Action = ACTION.GO_BACK;
      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, StartPosition.y, Positions[CurrentPosition].position.z);
    }
  }

  private void FixedUpdate()
  {
    if (Target != null)
    {
      transform.position += MoveDirection * Time.fixedDeltaTime;
    }
    else
    {
      transform.position = transform.position + transform.forward * Speed * Time.fixedDeltaTime;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    IHit obj = other.GetComponent<IHit>();

    if (Target == null && obj != null && obj.Alive && (obj.Type == IHitType.PLAYER || obj.Type == IHitType.PLAYER_AI))
    {
      Target = obj;
      Action = ACTION.ATTACK;
    }
  }

  private void OnDrawGizmos()
  {
    if (Positions != null)
    {
      for (int i = 0; i < Positions.Length; ++i)
      {
        if (Positions[i] != null)
        {
          Gizmos.color = Color.green;
          Gizmos.DrawWireSphere(Positions[i].position, 0.5f);
          if (i < Positions.Length - 1 && Positions[i + 1] != null)
          {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Positions[i].position, Positions[i + 1].position);
          }
        }
      }
    }

    if (TargetPosition != Vector3.zero)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawLine(transform.position, TargetPosition);
    }
  }

  private enum ACTION { IDLE, ATTACK, POS_ATTACK, GO_BACK };
}
