using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Krasnoluda", menuName = "Karta Krasnoluda")]
public class KartaKrasnoluda : Karta
{
    [SerializeField]protected TypKlanu typKlanu;
    [SerializeField]protected TypKrasnoluda typKrasnoluda;

    public TypKlanu _TypKlanu
    {
        get { return typKlanu; }
    }

    public TypKrasnoluda _TypKrasnoluda
    {
        get { return typKrasnoluda; }
    }

   

    public enum TypKlanu
    {
        Niebieski,
        Żółty
    }

    public enum TypKrasnoluda
    {
        Lojalny,
        Sabotażysta,
        Samolub
    }

    public override string ToString()
    {
       return base.ToString() + " " + typKlanu.ToString() + " " + typKrasnoluda.ToString();
    }
}
