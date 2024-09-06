using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KartaAkcji : Karta
{
    [SerializeField]protected TypAkcji typAkcji;  

    public enum TypAkcji
    {
        Narzędzie,
        Ujawnienie,
        Wydarzenie
    }

    public override string ToString()
    {
        return base.ToString() + " " + typAkcji.ToString();
    }
}
