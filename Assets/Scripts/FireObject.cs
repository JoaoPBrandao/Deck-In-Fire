using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireObject : MonoBehaviour
{
    [SerializeField] private SO_FireDefinition _fireDefinition;
    [SerializeField] public UnityEvent<FireObject> _OnSetOnFire, _OnExtinguish, _OnDestroyed;
    public SO_FireDefinition FireDefinition => _fireDefinition;
    public float RemainingTime => _remainingTime;
    private bool _onFire;
    private bool _destroyed;
    private float _remainingTime;


    private void Start()
    {
        GameManager.Instance.AddFireObject(this);
        enabled = false;
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
        _remainingTime = 20;
        _onFire = true;
        _OnSetOnFire.Invoke(this);
        enabled = true;
        GameManager.Instance.RemoveFireObject(this);
    }

    private void DestroyFireObject()
    {
        _destroyed = true;
        _onFire = false;
        enabled = false;
        _OnDestroyed.Invoke(this);
    }
    
    private void Extinguish()
    {
        _destroyed = true;
        _onFire = false;
        enabled = false;
        _OnExtinguish.Invoke(this);
        GameManager.Instance.AddFireObject(this);
    }
    
}
