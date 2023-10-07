using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherBase : MonoBehaviour, IInteractable
{
    [SerializeField] private SO_ExtinguisherDefinition _initialExtinguisher;
    [SerializeField] private MeshRenderer _extinguisherPosition;
    public bool HasExtinguisher => _currentExtinguisher != null;
    private ExtinguisherInstance _currentExtinguisher;
    
    private void Start()
    {
        if (_initialExtinguisher)
        {
            _currentExtinguisher = _initialExtinguisher.CreateInstance();
        }
        UpdateModel();
    }

    public void Interact(PlayerCharacter interactor)
    {
        _currentExtinguisher = interactor.SwitchExtinguisher(_currentExtinguisher);
        UpdateModel();
    }

    public void OnStartBeingTarget(PlayerCharacter interactor)
    {
    }

    public void OnStopBeingTarget(PlayerCharacter interactor)
    {
    }

    public bool CanInteract(PlayerCharacter interactor)
    {
        return HasExtinguisher || interactor.HasExtinguisher;
    }

    private void UpdateModel()
    {
        _extinguisherPosition.gameObject.SetActive(_currentExtinguisher != null);
        if (_currentExtinguisher != null)
        {
            _extinguisherPosition.material = _currentExtinguisher.Definition.Material;
        }
    }
}
