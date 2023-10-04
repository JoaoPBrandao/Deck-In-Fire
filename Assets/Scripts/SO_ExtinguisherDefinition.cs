using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DeckInFire/SO/ExtinguisherDefinition")]
public class SO_ExtinguisherDefinition : ScriptableObject
{
    public int Uses;
    public Mesh Model;
    public Sprite Icon;
    public string Name;
    public string Description;

    public ExtinguisherInstance CreateInstance()
    {
        var result = new ExtinguisherInstance();
        result.RemainingUses = Uses;
        result.Definition = this;
        return result;
    }
}

public class ExtinguisherInstance
{
    public int RemainingUses;
    public SO_ExtinguisherDefinition Definition;
}