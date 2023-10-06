using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T GetRandom<T>(this List<T> obj)
    {
        return obj[Random.Range(0, obj.Count)];
    }
}
