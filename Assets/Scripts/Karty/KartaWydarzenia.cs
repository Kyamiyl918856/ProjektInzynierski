using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Wydarzenia", menuName = "Karta Wydarzenia")]
public class KartaWydarzenia : KartaAkcji
{
    [SerializeField]protected TypWydarzenia typWydarzenia;

    

    public enum TypWydarzenia
    {
        Pułapka,
        Troll,
        Burza
    }

    public override string ToString()
    {
        return base.ToString() + " " + typWydarzenia.ToString();
    }
}
