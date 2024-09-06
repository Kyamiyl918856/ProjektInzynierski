using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Narzedzia", menuName = "Karta Narzedzia")]
public class KartaNarzedzia : KartaAkcji
{
    [SerializeField] protected TypNarzedzia typNarzedzia;

    

    public enum TypNarzedzia
    {
        Topór,
        Łódź,
        Lina
    }

    public TypNarzedzia GetTypNarzedzia()
    {
        return typNarzedzia;
    }

    public override string ToString()
    {
        return base.ToString() + " " + typNarzedzia.ToString();
    }
}
