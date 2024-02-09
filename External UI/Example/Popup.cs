/*
  _    _                     _                                  _                 _   _         
 | |  | |                   | |                                | |               | | (_)        
 | |__| |   ___  __  __   __| |   ___     __ _   ______   ___  | |_   _   _    __| |  _    ___  
 |  __  |  / _ \ \ \/ /  / _` |  / _ \   / _` | |______| / __| | __| | | | |  / _` | | |  / _ \ 
 | |  | | |  __/  >  <  | (_| | | (_) | | (_| |          \__ \ | |_  | |_| | | (_| | | | | (_) |
 |_|  |_|  \___| /_/\_\  \__,_|  \___/   \__, |          |___/  \__|  \__,_|  \__,_| |_|  \___/ 
                                          __/ |                                                 
                                         |___/             
*/
using UnityEngine;
using HexTools.UI;
using System;
using TMPro;
using System.Collections;

public sealed class Popup : ExternalUI
{
  public struct Context : IContext
  {
    public string title;
    public string description;
    public string acceptLabel;
    public string rejectLabel;
    public Action<bool> OnChoose;

    public Context(string title, string description, string acceptLabel, string rejectLabel, Action<bool> onChoose)
    {
      this.title = title;
      this.description = description;
      this.acceptLabel = acceptLabel;
      this.rejectLabel = rejectLabel;
      OnChoose = onChoose;
    }
  }

  [SerializeField] private float transitionTime = 0.5f;
  [SerializeField] private CanvasGroup panel;
  [SerializeField] private TextMeshProUGUI title;
  [SerializeField] private TextMeshProUGUI description;
  [SerializeField] private TextMeshProUGUI acceptLabel;
  [SerializeField] private TextMeshProUGUI rejectLabel;
  private Context context;

  public void Accept()
  {
    context.OnChoose?.Invoke(true);
    StartCoroutine(CloseProcess());
  }
  public void Reject()
  {
    context.OnChoose?.Invoke(false);
    StartCoroutine(CloseProcess());
  }
  protected override void OnEscape()
  {
    Reject();
  }

  protected override void OnInit(IContext value)
  {
    if (value is Context context)
    {
      this.context = context;
      title.text = context.title;
      description.text = context.description;
      acceptLabel.text = context.acceptLabel;
      rejectLabel.text = context.rejectLabel;
      canvas.worldCamera = Camera.main;
      StartCoroutine(OpenProcess());
    }
    else
      Dispose();
  }

  private float EaseInBack(float x)
  {
    float c1 = 1.70158f;
    float c3 = c1 + 1;

    return c3 * x * x * x - c1 * x * x;
  }
  private float EaseOutBack(float x)
  {
    float c1 = 1.70158f;
    float c3 = c1 + 1;

    return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
  }
  private IEnumerator OpenProcess()
  {
    float t = 0.0f;

    panel.blocksRaycasts = false;
    panel.transform.localScale = new Vector3(0, 0, 0);

    while (t < transitionTime)
    {
      t += Time.deltaTime;
      panel.transform.localScale = Vector3.LerpUnclamped(new Vector3(0, 0, 0), new Vector3(1, 1, 1), EaseOutBack(t / transitionTime));
      yield return null;
    }

    panel.transform.localScale = new Vector3(1, 1, 1);
    panel.blocksRaycasts = true;
  }
  private IEnumerator CloseProcess()
  {
    float t = 0.0f;

    panel.transform.localScale = new Vector3(1, 1, 1);
    panel.blocksRaycasts = false;

    while (t < transitionTime)
    {
      t += Time.deltaTime;
      panel.transform.localScale = Vector3.LerpUnclamped(new Vector3(1, 1, 1), new Vector3(0, 0, 0), EaseInBack(t / transitionTime));
      yield return null;
    }

    panel.transform.localScale = new Vector3(0, 0, 0);
    Dispose();
  }
}
