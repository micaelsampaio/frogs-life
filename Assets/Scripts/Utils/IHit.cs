using UnityEngine;

public interface IHit
{
  GameObject gameObject { get; }
  IHitType Type { get; }
  bool Alive { get; }
  bool Hit(IHitType Enemy);
}

public enum IHitType { PLAYER, PLAYER_AI, ENEMY }
