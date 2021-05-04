using UnityEngine;

public class CollisionEvent : MonoBehaviour
{
  public string EventName;

  public string[] EventNames;
  public string EventExitName;
  public string[] EventExitNames;

  public MeshRenderer meshRenderer = null;

  private void Start()
  {
    if (meshRenderer != null)
    {
      meshRenderer.enabled = false;
    }
  }

  private void OnTriggerEnter(Collider other)
  {

    if (Utils.IsPlayer(other))
    {
      if (!string.IsNullOrEmpty(EventName)) GameController.Instance.DispatchEvent(EventName);

      if (EventNames != null)
      {
        foreach (var evt in EventNames)
        {
          GameController.Instance.DispatchEvent(evt);
        }
      }
    }

  }

  private void OnTriggerExit(Collider other)
  {
    if (!string.IsNullOrEmpty(EventExitName)) GameController.Instance.DispatchEvent(EventExitName);

    if (EventExitNames != null)
    {
      foreach (var evt in EventExitNames)
      {
        GameController.Instance.DispatchEvent(evt);
      }
    }
  }
}
