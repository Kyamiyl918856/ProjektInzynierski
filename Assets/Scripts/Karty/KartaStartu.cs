using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Startu", menuName = "Karta Startu")]
public class KartaStartu : KartaSciezki
{
    [SerializeField]protected TypKlanu typKlanu;
    public TypKlanu _TypKlanu
    {
        get { return typKlanu; }
    }
    

    public enum TypKlanu
    {
        Niebieski,
        Żółty
    }

    public override string ToString()
    {
        return base.ToString() + " " + typKlanu.ToString();
    }
}
