using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<LevelFireTargetDefinition> _levelFireTargets;
    [SerializeField] private Vector2 _fireInterval;
    [SerializeField] private int _levelTarget, _levelMisses;

    private int _remainingMisses, _remainingTargets;
    private float _nextFireTime;
    
    private Dictionary<SO_FireDefinition, List<FireObject>> _objectsByDefinition = new Dictionary<SO_FireDefinition, List<FireObject>>();

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _remainingMisses = _levelMisses;
        _remainingTargets = _levelTarget;
        _nextFireTime = Random.Range(_fireInterval.x, _fireInterval.y);
    }

    private void Update()
    {
        _nextFireTime -= Time.deltaTime;
        if (_nextFireTime <= 0)
        {
            _nextFireTime = Random.Range(_fireInterval.x, _fireInterval.y);
            if (_levelFireTargets.Count <= 0)
            {
                Debug.LogError("GameManager does not have any fire object");
                return;
            }
            var target = _levelFireTargets.GetRandom();
            target.Quantity--;
            if (target.Quantity <= 0)
            {
                _levelFireTargets.Remove(target);
            }
            if (!_objectsByDefinition.TryGetValue(target.FireType, out var objectsList))
            {
                Debug.LogError("GameManager does not have selected fire type object list");
                return;
            }
            if (objectsList.Count <= 0)
            {
                Debug.LogError("GameManager does not have selected fire type object");
                return;
            }
            objectsList.GetRandom().SetOnFire();
        }
    }

    public void AddFireObject(FireObject newObject)
    {
        List<FireObject> fireObjectList;
        if (!_objectsByDefinition.TryGetValue(newObject.FireDefinition, out fireObjectList))
        {
            fireObjectList = new List<FireObject>();
            _objectsByDefinition.Add(newObject.FireDefinition, fireObjectList);
        }
        newObject._OnDestroyed.AddListener(OnObjectDestroyed);
        newObject._OnExtinguish.AddListener(OnObjectExtinguished);
        fireObjectList.Add(newObject);
    }
    
    public void RemoveFireObject(FireObject fireObject)
    {
        if (_objectsByDefinition.TryGetValue(fireObject.FireDefinition, out var fireObjectList))
        {
            fireObjectList.Remove(fireObject);
        }
    }

    private void OnObjectExtinguished()
    {
        _remainingTargets--;
        if (_remainingTargets <= 0)
        {
            Debug.Log("Won");
        }
    }

    private void OnObjectDestroyed()
    {
        _remainingMisses--;
        if (_remainingMisses <= 0)
        {
            Debug.Log("Lost");
        }
    }

    [Serializable]
    private class LevelFireTargetDefinition
    {
        public int Quantity;
        public SO_FireDefinition FireType;
    }
}
