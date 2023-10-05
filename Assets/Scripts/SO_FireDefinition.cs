using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DeckInFire/SO/FireDefinition")]
public class SO_FireDefinition : ScriptableObject
{
    [SerializeField] private List<SO_ExtinguisherDefinition> _compatibleExtinguishers;

    public bool Extinguish(SO_ExtinguisherDefinition extinguisher)
    {
        return _compatibleExtinguishers.Contains(extinguisher);
    }
}
