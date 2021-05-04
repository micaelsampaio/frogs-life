using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
  public FrogController Frog;
  // Start is called before the first frame update
  void Start()
  {
    GameController.Instance.AddEventListener("EndGame", () =>
    {
      Frog.Controls = false;
      Frog.Animator.Play("Idle");
      Frog.Animator.SetFloat("Speed", 0f);
      Frog.Animator.SetBool("Grounded", true);

      StartCoroutine(EndGame());
    });
  }


  private IEnumerator EndGame()
  {
    yield return GameController.Instance.GameUi.ShowUIDead("");
    GameController.Instance.GameUi.ShowCredits();
    yield return new WaitForSeconds(5f);
    SceneManager.LoadScene("Menu");
  }
  // Update is called once per frame
  void Update()
  {

  }
}
