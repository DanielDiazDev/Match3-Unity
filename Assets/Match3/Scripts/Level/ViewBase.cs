using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Match3.Scripts.Level
{
    public abstract class ViewBase : MonoBehaviour
    {
        protected CompositeDisposable _disposables = new CompositeDisposable();
      
        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}