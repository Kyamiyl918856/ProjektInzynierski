using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleMapy : MonoBehaviour
{
    [SerializeField]private int x;
    [SerializeField]private int y;
    [SerializeField]private TypPola typPola;
    [SerializeField]private GameObject kartaPołozonaNaPolu;


    public PoleMapy(TypPola typPola)
    {
        this.typPola = typPola;
        this.kartaPołozonaNaPolu = null;
    }

    public enum TypPola
    {
        Zwykłe,
        Startowe,
        Kopalnia
    }

    public TypPola _TypPola { get { return this.typPola; } }

    public void SetTypPola(TypPola typPola)
    {
        this.typPola = typPola;
    }
    public void SetKartaPołozonaNaPolu(GameObject kartaDoPolozenia)
    {
        this.kartaPołozonaNaPolu = kartaDoPolozenia;
    }
    public GameObject GetKartaPolozonaNaPolu()
    {
        return this.kartaPołozonaNaPolu;
    }

    public GameObject KartaPolozonaNaPolu
    {
        get { return kartaPołozonaNaPolu; }
        set { kartaPołozonaNaPolu = value; }
    }

    public int X
    {
        get { return x; }
        set { }
    }

    public int Y
    {
        get { return y; }
        set { }
    }
}   
