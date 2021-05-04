using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  public CanvasRenderer Fade;

  private void Start()
  {
    StartCoroutine(GameUi.FadeOutRoutine(Fade, 0.5f));
  }

  public void StartGame()
  {
    SceneManager.LoadScene("Game");
  }


}
