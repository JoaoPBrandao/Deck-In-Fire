using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinctor : MonoBehaviour, IInteractable
{
    public void Interact(PlayerCharacter interactor)
    {
        Debug.Log("Interaction");
    }
}
