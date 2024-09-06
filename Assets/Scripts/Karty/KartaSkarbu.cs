using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Skarbu", menuName = "Karta Skarbu")]
public class KartaSkarbu : Karta
{
    [SerializeField]protected TypSkarbu typSkarbu;
    [SerializeField]protected int wartosc;

    

    public enum TypSkarbu
    {
        Klejnoty,
        Złoto,
        Srebro
    }

    public TypSkarbu _TypSkarbu
    {
        get { return typSkarbu; }
    }

    public int Wartosc
    {
        get { return wartosc; }
    }

    public override string ToString()
    {
        return base.ToString() + " " + typSkarbu.ToString() + " " + "Wartość=" + wartosc;
    }
}
