using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _speed = 1;
    private Camera _camera;
    private Plane _mousePlane;

    private void OnEnable()
    {
        _camera = Camera.main;
        _mousePlane = new Plane(transform.up, transform.position);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact(this);
        }
    }
}
