using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireObject : MonoBehaviour
{
    [SerializeField] private SO_FireDefinition _fireDefinition;
    [SerializeField] private UnityEvent _OnSetOnFire, _OnExtinguish, _OnDestroyed;
    private bool _onFire;
    private bool _destroyed;
    private float _remainingTime;

    private void Start()
    {
        SetOnFire();
    }

    public void Extinguish(SO_ExtinguisherDefinition extinguisher)
    {
        if(!_onFire) return;
        if (_fireDefinition.Extinguish(extinguisher))
        {
            Extinguish();
        }
        else
        {
            DestroyFireObject();
        }
    }

    private void Update()
    {
        if (!_onFire) enabled = false;
        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0)
        {
            DestroyFireObject();
        }
    }

    public void SetOnFire()
    {
        _remainingTime = 10;
        _onFire = true;
        _OnSetOnFire.Invoke();
    }

    private void DestroyFireObject()
    {
        _destroyed = true;
        _onFire = false;
        enabled = false;
        _OnDestroyed.Invoke();
    }
    
    private void Extinguish()
    {
        _destroyed = true;
        _onFire = false;
        enabled = false;
        _OnExtinguish.Invoke();
    }
    
}
