using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _extinguisherRadius = 1;
    [SerializeField] private float _extinguisherRange = 3;
    [SerializeField] private float _extinguisherUseAnimationTime = 1;
    [SerializeField] private MeshRenderer _extinguisherPosition;
    [SerializeField] private GameObject _extinguisherTarget, _audioListener;
    public bool HasExtinguisher => _currentExtinguisher != null;
    private ExtinguisherInstance _currentExtinguisher;
    private Camera _camera;
    private Plane _mousePlane;
    private float _currentExtinguisherUseAnimationTime;
    private bool _usingExtinguisher;
    private List<FireObject> _extinguisherTargets = new List<FireObject>();
    private IInteractable _interactionTarget;
    public UnityEvent<ExtinguisherInstance> OnExtinguisherChanged, OnExtinguisherUsed;
    public UnityEvent OnBeginInteract, OnEndInteract;

    private void OnEnable()
    {
        _camera = Camera.main;
        _mousePlane = new Plane(transform.up, transform.position);
        UpdateModel();
    }

    private void Update()
    {
        if (_usingExtinguisher)
        {
            _currentExtinguisherUseAnimationTime -= Time.deltaTime;
            if (_currentExtinguisherUseAnimationTime > 0) return;
            OnExtinguisherAnimationEnded();
        }
        ProcessMovement();
        ProcessMousePosition();
        ProcessInput();
    }

    private void ProcessMovement()
    {
        var movementVector = new Vector3
        {
            x = Input.GetAxis("Horizontal"),
            z = Input.GetAxis("Vertical")
        };
        movementVector.Normalize();
        _controller.Move(movementVector * (_speed * Time.deltaTime));
    }
    
    private void ProcessMousePosition()
    {
        var mousePosition = Input.mousePosition;
        var ray = _camera.ScreenPointToRay(mousePosition);
        if (_mousePlane.Raycast(ray, out var distance))
        {
            var point = ray.GetPoint(distance);
            var dir = (point - transform.position).normalized;
            transform.forward = dir;
            _audioListener.transform.rotation = Quaternion.identity;
            var mouseDistance = Mathf.Min((point - transform.position).magnitude, _extinguisherRange);
            if (Physics.Raycast(transform.position, dir, out var hit, mouseDistance, LayerMask.GetMask("Wall")))
            {
                _extinguisherTarget.transform.position = hit.point;
            }
            else
            {
                _extinguisherTarget.transform.position = transform.position + (dir * mouseDistance);
            }
            
        }
    }

    private void ProcessInput()
    {
        if (Input.GetButtonDown("Interact") && _interactionTarget != null)
        {
            _interactionTarget.Interact(this);
        };
        
        if (Input.GetButtonDown("Fire1") && HasExtinguisher)
        {
            _usingExtinguisher = true;
            _currentExtinguisherUseAnimationTime = _extinguisherUseAnimationTime;
            _currentExtinguisher.RemainingUses--;
            OnExtinguisherUsed.Invoke(_currentExtinguisher);
        }
    }

    private void OnExtinguisherAnimationEnded()
    {
        _usingExtinguisher = false;
        //var hits = Physics.OverlapSphere(_extinguisherTarget.transform.position, _extinguisherRadius, LayerMask.GetMask("FireObject"));
        foreach (var target in _extinguisherTargets)
        {
            target.Extinguish(_currentExtinguisher.Definition);
        }
        if (_currentExtinguisher.RemainingUses <= 0)
        {
            SwitchExtinguisher(null);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (_interactionTarget == null && other.TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract(this))
        {
            interactable.OnStartBeingTarget(this);
            _interactionTarget = interactable;
            OnBeginInteract.Invoke();
            return;
        }

        if (other.TryGetComponent<FireObject>(out var fireObject))
        {
            _extinguisherTargets.Add(fireObject);
            fireObject.OnBeginTargeted();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactionTarget != null && other.TryGetComponent<IInteractable>(out var interactable) && _interactionTarget == interactable)
        {
            interactable.OnStopBeingTarget(this);
            _interactionTarget = null;
            OnEndInteract.Invoke();
            return;
        }
        
        if (other.TryGetComponent<FireObject>(out var fireObject))
        {
            _extinguisherTargets.Remove(fireObject);
            fireObject.OnEndTargeted();
        }
    }

    public ExtinguisherInstance SwitchExtinguisher(ExtinguisherInstance newExtinguisher)
    {
        var previousExtinguisher = _currentExtinguisher;
        _currentExtinguisher = newExtinguisher;
        UpdateModel();
        OnExtinguisherChanged.Invoke(_currentExtinguisher);
        return previousExtinguisher;
    }
    
    private void UpdateModel()
    {
        _extinguisherPosition.gameObject.SetActive(_currentExtinguisher != null);
        _extinguisherTarget.SetActive(_currentExtinguisher != null);
        if (_currentExtinguisher != null)
        {
            _extinguisherPosition.material = _currentExtinguisher.Definition.Material;
        }
    }
    
    
}
