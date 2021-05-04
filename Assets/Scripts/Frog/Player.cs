using UnityEngine;

public interface Player
{
  GameObject gameObject { get; }
  bool IsHidden { get; }

  IHit IHit { get; }

  void Stop();

}