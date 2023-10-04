using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerCharacter interactor);

    public void OnStartBeingTarget(PlayerCharacter interactor);
    
    public void OnStopBeingTarget(PlayerCharacter interactor);

    public bool CanInteract(PlayerCharacter interactor);
}
