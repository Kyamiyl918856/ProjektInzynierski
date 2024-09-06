using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Znacznik : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] private TypZnacznika typZnacznika;

    public enum TypZnacznika
    {
        Topor,
        Lodka,
        Lina
    }

    void Start()
    {
        //UstawMaterial(material);
    }

    public void UstawMaterial(Material material)
    {
        gameObject.GetComponent<Renderer>().material = material;
    }

    
}
