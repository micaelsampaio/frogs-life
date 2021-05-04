using UnityEngine;

public class MosquitoMovement : MonoBehaviour, IHit
{
  public Transform RandomBounds;
  public Vector3 Target;

  public IHitType Type => IHitType.ENEMY;
  public bool Alive { get; set; } = false;

  public AudioSource AudioSource;

  private void OnEnable()
  {
    Alive = true;
    Target = GetRandomPosition();
    AudioSource.Stop();
    Invoke("StartSound", Random.Range(0, 3f));
  }

  private void OnDisable()
  {
    CancelInvoke();
  }

  private void StartSound()
  {
    AudioSource.Play();
  }

  void Update()
  {
    if (!Alive) return;

    if (Vector3.Distance(transform.position, Target) < 0.1f)
    {
      Target = GetRandomPosition();
    }

    transform.rotation = Utils.LookAtSmooth(transform, Target, 20f);
  }

  private void FixedUpdate()
  {
    transform.position = transform.position + transform.forward * 5f * Time.fixedDeltaTime;
  }

  Vector3 GetRandomPosition()
  {
    if (!RandomBounds) return Vector3.zero;
    var randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    return RandomBounds.TransformPoint(randomPos * 0.5f);
  }

  public bool Hit(IHitType Enemy)
  {
    Alive = false;
    return true;
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(Target, 0.5f);
  }
}
