using System;
using UniRx;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    protected CompositeDisposable _disposables = new CompositeDisposable();

    protected virtual void OnDestroy()
    {
        _disposables?.Dispose();
    }
}