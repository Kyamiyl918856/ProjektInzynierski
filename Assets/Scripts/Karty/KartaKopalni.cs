using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Kopalni", menuName = "Karta Kopalni")]
public class KartaKopalni : KartaSciezki
{
    [SerializeField] protected TypKopalni typKopalni;
    [SerializeField] protected bool czyKopalniaOdkryta = false;

    public bool CzyKopalniaOdkryta
    {
        get { return czyKopalniaOdkryta; }
        set { czyKopalniaOdkryta = value; }
    }

    public enum TypKopalni
    {
        Klejnoty,
        Złoto,
        Srebro,
        Smok
    }

    public TypKopalni _TypKopalni
    {
        get { return typKopalni; }
    }

    public override string ToString()
    {
        return base.ToString() + " " + typKopalni.ToString();
    }
}
