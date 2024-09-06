using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PionekVisualization : MonoBehaviour
{
    [SerializeField]private Pionek scriptablePionek;
    
    void Start()
    {
        DisplayVisualizationPionek();
    }

    private void DisplayVisualizationPionek()
    {
        Material grafikaPionka = scriptablePionek.GetMaterialGrafika();

        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = grafikaPionka;
        this.gameObject.transform.GetChild(1).GetComponent<Renderer>().material = grafikaPionka;
    }
}
