using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PionekInfo : MonoBehaviour
{
    [SerializeField] TMP_Text nazwaGraczaTextMeshPro;
    //[SerializeField] string nazwaGracza;
    [SerializeField] Klan klan;
    [SerializeField] GameObject kartaSciezkiNaKtorejStoje;

    public string NazwaGraczaTextMeshPro_text
    {
        get { return nazwaGraczaTextMeshPro.text; }
        set { nazwaGraczaTextMeshPro.text = value; }
    }

    public Klan _Klan
    {
        get { return klan; }
    }

    public GameObject KartaSciezkiNaKtorejStoje
    {
        get { return kartaSciezkiNaKtorejStoje; }
        set { kartaSciezkiNaKtorejStoje = value; }
    }

    public enum Klan
    {
        Niebieski,
        Zolty
    }
}
