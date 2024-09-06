using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Karta : ScriptableObject
{
    // Awers
    [SerializeField]protected Material materialAwers;
    // Rewers
    [SerializeField]protected Material materialRewers;
    
    [SerializeField]protected TypKarty typKarty;

    private bool czyKartaOdkryta = false;

    public TypKarty _TypKarty
    {
        get { return typKarty; }
    }

    
    public enum TypKarty
    {
        Ścieżka,
        Akcja,
        Start,
        Skarb,
        Kopalnia,
        Krasnolud
    }

    public bool CzyKartaOdkryta
    {
        get { return czyKartaOdkryta; }
        set {czyKartaOdkryta=value; }
    }

    public Material GetMaterialAwers()
    {
        return materialAwers;
    }

    public Material GetMaterialRewers()
    {
        return materialRewers;
    }

    public override string ToString()
    {
        return typKarty.ToString();
    }

}
