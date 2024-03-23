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
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HexTools.UI
{
    public abstract class ExternalUI : MonoBehaviour
    {
        public interface IContext { }
        protected Canvas canvas;
        private AsyncOperationHandle<SceneInstance> handle;
        private Action OnDispose;

        public void Init(IContext context, AsyncOperationHandle<SceneInstance> handle, int layer, Action onDispose = null)
        {
            this.handle = handle;
            OnDispose = onDispose;
            canvas.sortingOrder = layer;
            OnInit(context);
        }
        public void Escape()
        {
            OnEscape();
        }
        public void Dispose()
        {
            OnDispose?.Invoke();
            ExternalUIManager.Unload(handle);
        }

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
        }
        protected virtual void OnEscape() { }
        protected abstract void OnInit(IContext context);
    }
}
