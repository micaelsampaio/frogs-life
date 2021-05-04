
using UnityEngine;

public class CrowAI : MonoBehaviour
{
  public CROW_TYPE CrowType;
  private ACTION Action;
  public Animator Animator;

  public Level CurrentLevel;

  public SoundController SoundController;

  public Transform[] Positions;
  private int CurrentPosition;
  private Vector3 TargetPosition;
  public bool Move;
  public float Speed;
  private Vector3 MoveDirection;
  private Vector3 StartPosition;

  public string Area;

  private Player Target;

  private float CurrentTime;

  private void OnEnable()
  {
    Target = null;
    Action = ACTION.IDLE;
    Move = false;
    MoveDirection = Vector3.zero;
    CurrentTime = 0f;

    if (CrowType == CROW_TYPE.PATROL)
    {
      CurrentPosition = 0;
      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, transform.position.y, Positions[CurrentPosition].position.z);
    }

    if (CrowType == CROW_TYPE.EATING)
    {
      StartEatingIdle();
    }

    SubscribeEvents();

    transform.rotation = Quaternion.Euler(0, 0, 0);
  }

  private void OnDisable()
  {
    UnsubscribeEvents();
    transform.position = StartPosition;
    transform.rotation = Quaternion.Euler(0, 0, 0);
  }

  private void Awake()
  {
    StartPosition = transform.position;
  }

  // Update is called once per frame
  void Update()
  {
    switch (CrowType)
    {
      case CROW_TYPE.EATING:
        UpdateEating();
        break;
      case CROW_TYPE.PATROL:
        UpdatePatrol();
        break;
    }
  }

  private void UpdatePatrol()
  {
    switch (Action)
    {
      case ACTION.IDLE:
        IdlePatrolState();
        break;
    }
  }

  private void IdlePatrolState()
  {

    if (Target != null && !Target.IsHidden)
    {
      Action = ACTION.PRE_ATTACK;
      Move = false;
      return;
    }

    transform.rotation = Utils.LookAt(transform, TargetPosition, 5);

    Vector3 dir = (TargetPosition - transform.position).normalized;
    float dot = Vector3.Dot(dir, transform.forward);
    Debug.Log(dot);
    if (dot > 0.9f)
    {
      transform.rotation = Utils.LookAt(transform, TargetPosition, 1000);
      Move = true;
      MoveDirection = transform.forward * Speed;
    }
    else if (Move)
    {
      Move = false;
    }

    if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
    {
      if (++CurrentPosition >= Positions.Length)
      {
        CurrentPosition = 0;
      }
      Move = false;
      TargetPosition = new Vector3(Positions[CurrentPosition].position.x, transform.position.y, Positions[CurrentPosition].position.z);
    }
  }

  public void UpdateEating()
  {
    switch (Action)
    {
      case ACTION.IDLE:
        IdleEatingState();
        break;
      case ACTION.LOOK_AROUND:
        LookAroundState();
        break;
      case ACTION.PRE_ATTACK:
        PreAttackState();
        break;
      case ACTION.ATTACK:
        AttackState();
        break;
      case ACTION.POS_ATTACK:
        PosAttackState();
        break;
      default:
        StartEatingIdle();
        break;
    }
  }

  public void IdleEatingState()
  {
    CurrentTime += Time.deltaTime;
    if (CurrentTime > 3.5f)
    {
      StartLookAround();
    }
  }

  private void StartEatingIdle()
  {
    Action = ACTION.IDLE;
    CurrentTime = 0f;
    transform.rotation = Quaternion.Euler(0, 0, 0);
    Animator.SetTrigger("Idle");
  }

  private void StartLookAround()
  {
    Action = ACTION.LOOK_AROUND;
    CurrentTime = 0f;
    Animator.SetTrigger("LookAt");
    // transform.rotation = Quaternion.Euler(0, 0, 0);
  }
  private void LookAroundState()
  {
    CurrentTime += Time.deltaTime;

    if (CurrentTime > 3f)
    {
      StartEatingIdle();
    }

    if (Target != null && !Target.IsHidden && CurrentTime > 0.2f)
    {
      Action = ACTION.PRE_ATTACK;
      Animator.SetTrigger("StartAttack");
      Animator.SetBool("Attack", false);
      SoundController.PlaySound("Wings");
    }
  }

  private void PreAttackState()
  {
    Move = true;
    transform.rotation = Utils.LookAt(transform, Target.gameObject.transform, 5f);
    MoveDirection = Vector3.up * (Speed / 2);

    if (Mathf.Abs(transform.position.y - StartPosition.y) > 8f)
    {
      Move = false;
      Action = ACTION.ATTACK;
      Animator.SetBool("Attack", true);
      SoundController.PlaySound("Crow");
    }
  }

  private void AttackState()
  {
    transform.rotation = Utils.LookAt(transform, Target.gameObject.transform, 5f);

    if (Vector3.Distance(transform.position, Target.gameObject.transform.position) < 0.2f)
    {
      GameController.Instance.CurrentLevel = CurrentLevel;
      Action = ACTION.POS_ATTACK;
      Target.IHit.Hit(IHitType.ENEMY);
      Target.gameObject.transform.SetParent(transform);
      Animator.SetBool("Attack", false);
      SoundController.PlaySound("Wings");
      Target = null;
    }
  }

  private void PosAttackState()
  {
    Move = true;
    MoveDirection = transform.forward * Speed + transform.up * Speed / 1.8f;
  }


  private void FixedUpdate()
  {
    if (Move)
    {
      transform.position = transform.position + MoveDirection * Time.fixedDeltaTime;
    }

    if (Action == ACTION.ATTACK && Target != null)
    {
      transform.position = Vector3.MoveTowards(transform.position, Target.gameObject.transform.position, Time.fixedDeltaTime * Speed * 2f);
    }
  }

  private void OnDrawGizmos()
  {
    if (Positions != null && CrowType == CROW_TYPE.PATROL)
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
  }

  /// EVENTS
  private void SubscribeEvents()
  {
    if (GameController.Instance != null && CrowType == CROW_TYPE.EATING && !string.IsNullOrEmpty(Area))
    {
      Debug.Log("SUSBSCRIBE EVENTS");
      GameController.Instance.AddEventListener("OnEnterArea_" + Area, OnEnterArea);
      GameController.Instance.AddEventListener("OnExitArea_" + Area, OnExitArea);
    }
  }

  private void UnsubscribeEvents()
  {
    if (GameController.Instance != null)
    {
      GameController.Instance.RemoveEventListener("OnEnterArea_" + Area, OnEnterArea);
      GameController.Instance.RemoveEventListener("OnExitArea_" + Area, OnExitArea);
    }
  }

  private void OnEnterArea()
  {
    Target = GameController.Instance.Player;
    Debug.Log("OnEnterArea " + Target);
  }

  private void OnExitArea()
  {

    if (Action == ACTION.IDLE)
    {
      Target = null;
      Debug.Log("OnExitArea " + Target);
    }
  }
}


public enum ACTION { IDLE, PRE_ATTACK, ATTACK, ATTACK_END, LOOK_AROUND, POS_ATTACK };

public enum CROW_TYPE { PATROL, EATING };
