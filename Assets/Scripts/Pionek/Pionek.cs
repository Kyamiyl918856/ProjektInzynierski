using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowy Pionek", menuName = "Pionek")]
public class Pionek : ScriptableObject
{
    [SerializeField] protected Material materialGrafika;

    public Material GetMaterialGrafika()
    {
        return materialGrafika;
    }
}
