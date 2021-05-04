using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUi : MonoBehaviour
{
  public CanvasRenderer BlackScreen;
  public GameObject Credits;
  public TextMeshProUGUI Hint;

  private void Start()
  {
    BlackScreen.SetAlpha(0f);
    BlackScreen.gameObject.SetActive(false);
    Credits.gameObject.SetActive(false);
  }

  public IEnumerator ShowUIDead(string hint)
  {
    Hint.text = "";

    yield return FadeInRoutine(BlackScreen, 0.5f);

    if (string.IsNullOrEmpty(hint))
    {
      Hint.gameObject.SetActive(false);
    }
    else
    {
      Hint.text = hint;
      Hint.gameObject.SetActive(true);
    }
    //return new WaitForSeconds(0.1f);
  }

  public void ShowCredits()
  {
    Credits.SetActive(true);
  }

  public IEnumerator HideUIDead()
  {
    Hint.text = "";
    yield return FadeOutRoutine(BlackScreen, 0.5f);
  }

  public static IEnumerator FadeInRoutine(CanvasRenderer FadeObject, float time)
  {
    float alpha = 0;
    FadeObject.gameObject.SetActive(true);
    while (alpha < 1)
    {
      alpha += Time.deltaTime * time;
      FadeObject.SetAlpha(alpha);
      yield return new WaitForEndOfFrame();
    }
    FadeObject.SetAlpha(1f);
  }

  public static IEnumerator FadeOutRoutine(CanvasRenderer FadeObject, float time)
  {
    float alpha = 1;
    FadeObject.gameObject.SetActive(true);
    while (alpha > 0)
    {
      alpha -= Time.deltaTime * time;
      FadeObject.SetAlpha(alpha);
      yield return new WaitForEndOfFrame();
    }
    FadeObject.SetAlpha(0f);
    FadeObject.gameObject.SetActive(false);

  }
}
