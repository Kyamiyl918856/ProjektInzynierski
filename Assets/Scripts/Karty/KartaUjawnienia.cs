using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Ujawnienia", menuName = "Karta Ujawnienia")]
public class KartaUjawnienia : KartaAkcji
{
    [SerializeField]protected TypUjawnienia[] typUjawnienia;

   

    public enum TypUjawnienia
    {
        Kopalnia,
        Skarb,
        Krasnolud
    }

    public override string ToString()
    {
        return base.ToString() + " " + typUjawnienia[0].ToString() + " lub " + typUjawnienia[1].ToString();
    }
}
