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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HexTools.UI
{
  public class ExternalUIManager : MonoBehaviour
  {

    private class StackEvent
    {
      private readonly Action onFire;
      private bool isFired;

      public StackEvent(Action onFire)
      {
        this.onFire = onFire;
        isFired = false;
      }

      public void Fire()
      {
        if (!isFired)
        {
          isFired = true;
          onFire?.Invoke();
        }
      }
    }
    private readonly static Stack<StackEvent> escEventStack = new();
    private bool isEscFired = false;
    private static ExternalUIManager instance;
    private static ExternalUIManager Instance
    {
      get
      {
        if (instance == null)
        {
          GameObject obj = new("[External UI]");
          instance = obj.AddComponent<ExternalUIManager>();
          DontDestroyOnLoad(obj);
        }
        return instance;
      }
    }

    public static void Load(string key, ExternalUI.IContext context, Action onDispose = null)
    {
      var manager = Instance;
      manager.StartCoroutine(manager.LoadProcess(key, context, onDispose));
    }
    public static void Unload(AsyncOperationHandle<SceneInstance> handle)
    {
      RemoveActiveEscEvent();
      Addressables.UnloadSceneAsync(handle);
    }
    public static void AddActiveEscEvent(Action onEscape)
    {
      escEventStack.Push(new StackEvent(onEscape));
    }
    public static void RemoveActiveEscEvent()
    {
      if (escEventStack.Count > 0)
        escEventStack.Pop();
    }

    private void Update()
    {
      if (Input.GetKey(KeyCode.Escape))
      {
        if (!isEscFired)
        {
          if (escEventStack.Count > 0)
            escEventStack.Peek().Fire();
          isEscFired = true;
        }
      }
      else if (isEscFired)
        isEscFired = false;
    }
    private IEnumerator LoadProcess(string key, ExternalUI.IContext context, Action onDispose = null)
    {
      var handle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Additive);
      yield return handle;
      if (handle.Status == AsyncOperationStatus.Succeeded)
      {
        var roots = handle.Result.Scene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
          var externalUI = root.GetComponentInChildren<ExternalUI>();
          if (externalUI != null)
          {
            externalUI.Init(context, handle, escEventStack.Count, onDispose);
            AddActiveEscEvent(externalUI.Escape);
            break;
          }
        }
      }
    }
  }
}
