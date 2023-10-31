using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UnityEvent OnGameUpdated;
    public UnityEvent<bool> OnMatchFinished;
    [SerializeField] private List<LevelFireTargetDefinition> _levelFireTargets;
    [SerializeField] private Vector2 _fireInterval;
    [SerializeField] private int _levelTarget, _levelMisses, _fireAmount;
    [SerializeField] private bool _startEnabled;

    public int Score => _score;
    public int TotalScore => _totalScore;
    public int RemainingTargets => _remainingTargets;
    public int RemainingMisses => _remainingMisses;
    public int LevelTarget => _levelTarget;
    public int LevelMisses => _levelMisses;
    public int FireAmount => _fireAmount;

    private int _remainingMisses, _remainingTargets, _score;
    private float _nextFireTime;
    private static int _totalScore;
    
    private Dictionary<SO_FireDefinition, List<FireObject>> _objectsByDefinition = new Dictionary<SO_FireDefinition, List<FireObject>>();

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _remainingMisses = _levelMisses;
        _remainingTargets = 0;
        Instance = this;
    }

    private void Start()
    {
        _remainingMisses = _levelMisses;
        _remainingTargets = 0;
        _nextFireTime = Random.Range(_fireInterval.x, _fireInterval.y);
        enabled = _startEnabled;
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
            _fireAmount++;
            OnGameUpdated.Invoke();
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

    private void OnObjectExtinguished(FireObject targetObject)
    {
        if (!enabled) return;
        _remainingTargets++;
        _score += (int)targetObject.RemainingTime;
        _fireAmount--;
        OnGameUpdated.Invoke();
        if (_remainingTargets >= _levelTarget)
        {
            enabled = false;
            OnMatchFinished.Invoke(true);
        }
    }

    private void OnObjectDestroyed(FireObject targetObject)
    {
        if (!enabled) return;
        _remainingMisses--;
        _fireAmount--;
        OnGameUpdated.Invoke();
        if (_remainingMisses <= 0)
        {
            enabled = false;
            OnMatchFinished.Invoke(false);
        }
    }

    public void StartGame()
    {
        enabled = true;
    }
    
    [Serializable]
    private class LevelFireTargetDefinition
    {
        public int Quantity;
        public SO_FireDefinition FireType;
    }
}
