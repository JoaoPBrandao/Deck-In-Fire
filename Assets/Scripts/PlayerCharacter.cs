using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 1;
    [SerializeField] private MeshFilter _extinguisherPosition;

    public bool HasExtinguisher => _currentExtinguisher != null;
    private ExtinguisherInstance _currentExtinguisher;
    private Camera _camera;
    private Plane _mousePlane;
    private IInteractable _interactionTarget;
    public UnityEvent<ExtinguisherInstance> OnExtinguisherChanged;
    public UnityEvent OnBeginInteract, OnEndInteract;

    private void OnEnable()
    {
        _camera = Camera.main;
        _mousePlane = new Plane(transform.up, transform.position);
        UpdateModel();
    }

    private void Update()
    {
        var movementVector = new Vector3();
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.z = Input.GetAxis("Vertical");
        movementVector.Normalize();
        _controller.Move(movementVector * _speed);

        var mousePosition = Input.mousePosition;
        var ray = _camera.ScreenPointToRay(mousePosition);
        if (_mousePlane.Raycast(ray, out var distance))
        {
            var point = ray.GetPoint(distance);
            transform.forward = (point - transform.position).normalized;
        }

        if (Input.GetButtonDown("Interact") && _interactionTarget != null)
        {
            _interactionTarget.Interact(this);
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactionTarget == null && other.TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract(this))
        {
            interactable.OnStartBeingTarget(this);
            _interactionTarget = interactable;
            OnBeginInteract.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactionTarget != null && other.TryGetComponent<IInteractable>(out var interactable) && _interactionTarget == interactable)
        {
            interactable.OnStopBeingTarget(this);
            _interactionTarget = null;
            OnEndInteract.Invoke();
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
        if (_currentExtinguisher != null)
        {
            _extinguisherPosition.mesh = _currentExtinguisher.Definition.Model;
        }
    }
}
