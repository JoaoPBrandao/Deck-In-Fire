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
    [SerializeField] private Animator _animator;
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
    private static readonly int UsingExtinguisher = Animator.StringToHash("UsingExtinguisher");
    private static readonly int MovementX = Animator.StringToHash("MovementX");
    private static readonly int MovementZ = Animator.StringToHash("MovementZ");

    private void OnEnable()
    {
        _camera = Camera.main;
        _mousePlane = new Plane(transform.up, transform.position);
        UpdateModel();
    }

    private void Update()
    {
        if (!GameManager.Instance.enabled) return;
        if (_usingExtinguisher)
        {
            _currentExtinguisherUseAnimationTime -= Time.deltaTime;
            if (_currentExtinguisherUseAnimationTime > 0) return;
            OnExtinguisherAnimationEnded();
        }
        
        ProcessMousePosition();
        ProcessMovement();
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
        var animationDir = transform.InverseTransformDirection(movementVector);
        _animator.SetFloat(MovementX, animationDir.x);
        _animator.SetFloat(MovementZ, animationDir.z);

    }
    
    private void ProcessMousePosition()
    {
        var mousePosition = Input.mousePosition;
        var ray = _camera.ScreenPointToRay(mousePosition);
        if (_mousePlane.Raycast(ray, out var distance))
        {
            var point = ray.GetPoint(distance);
            var dir = (point - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
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
            _animator.SetBool(UsingExtinguisher, true);
            _currentExtinguisherUseAnimationTime = _extinguisherUseAnimationTime;
            _currentExtinguisher.RemainingUses--;
            OnExtinguisherUsed.Invoke(_currentExtinguisher);
        }
    }

    private void OnExtinguisherAnimationEnded()
    {
        _usingExtinguisher = false;
        _animator.SetBool(UsingExtinguisher, false);
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
