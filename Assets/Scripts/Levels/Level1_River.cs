using System.Collections;
using UnityEngine;

public class Level1_River : Level
{
  [Header("River Level")]
  public FroggletController Frogglet;
  public FrogController Frog;

  public GameObject Eggs;
  public GameObject Egg;
  public GameObject WALL;
  public Vector3 EggsDirection;

  public FishUnderWater FishUnderWater;

  // Start is called before the first frame update
  void Start()
  {
    GameController.Instance.AddEventListener("Level1_Hit_EndRiver", () =>
    {
      StartCoroutine(SwitchFrog());
      return;
      if (Frogglet.transform.localScale.x < 1)
      {
        FishUnderWater.Kill = true;
      }
      else
      {
        StartCoroutine(SwitchFrog());
      }
    });

    if (Frogglet.gameObject.activeSelf)
    {
      GameController.Instance.CameraController.target = Frogglet.transform;
      Frog.gameObject.SetActive(false);
      Frogglet.gameObject.SetActive(false);
      StartCoroutine(ShowFroggLet());
    }
  }

  // Update is called once per frame
  void Update()
  {
  }

  private void FixedUpdate()
  {
    if (!Visible) return;
  }

  private IEnumerator SwitchFrog()
  {
    GameController.Instance.DispatchEvent("OnEnterLevel_Crow");
    GameController.Instance.DispatchEvent("OnEnterLevel_Mosquitos");

    Frogglet.gameObject.SetActive(false);
    Frog.gameObject.SetActive(true);
    Frog.transform.position = Frogglet.transform.position;
    Frog.transform.rotation = Quaternion.Euler(0, -90, 0);
    Frog.Animator.Play("Jump");
    Frog.Animator.SetBool("Grounded", false);
    Frog.rb.AddForce(Frog.transform.forward * 2.5f + Vector3.up * 5f, ForceMode.Impulse);
    Frog.Controls = false;
    GameController.Instance.CameraController.target = Frog.transform;

    var clone = GameController.Instance.GetPool("WaterSplash").Get();
    clone.transform.position = Frog.transform.position;
    clone.SetActive(true);
    Frog.SoundController.PlayRandomSound("frog1,frog2,frog3");

    yield return new WaitForEndOfFrame();
    Frog.Animator.Play("Jump");

    yield return new WaitForSeconds(1.5f);
    WALL.SetActive(true);
    Frog.Controls = true;
  }

  public IEnumerator ShowFroggLet()
  {

    yield return new WaitForSeconds(2f);

    Frogglet.gameObject.SetActive(true);

    var clone = GameController.Instance.Pools["WaterSplash"].Get();
    clone.transform.position = Frogglet.transform.position;
    clone.SetActive(true);
  }
}
