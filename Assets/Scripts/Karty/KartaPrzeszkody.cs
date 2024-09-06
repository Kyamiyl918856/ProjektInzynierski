using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Przeszkody", menuName = "Karta Przeszkody")]
public class KartaPrzeszkody : KartaSciezki
{
    [SerializeField]protected TypPrzeszkody typPrzeszkody;
    [SerializeField]protected bool czyZnacznikUmieszczony;

   

    public enum TypPrzeszkody
    {
        Drzewo,
        Jezioro,
        Rów
    }

    public TypPrzeszkody GetTypPrzeszkody()
    {
        return typPrzeszkody;
    }

    public bool GetCzyZnacznikUmieszczony()
    {
        return czyZnacznikUmieszczony;
    }

    public void SetCzyZnacznikUmieszczony(bool value)
    {
        czyZnacznikUmieszczony = value;
    }

    public override string ToString()
    {
        return base.ToString() + " " + typPrzeszkody.ToString();
    }
}
