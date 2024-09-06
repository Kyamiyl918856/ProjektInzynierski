using Mirror;
using Mirror.Websocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    /*[SerializeField]*/ private int id = 0;
    public int Id
    {
        get { return id; }
    }
    [SerializeField] private Karta scriptableKarta;
    /*[SerializeField]*/ private int nrKolejnosciNaStosieDoDobierania = 0;

    

    private void Start() 
    {
        DisplayCard();
    }

    private void DisplayCard()
    {
        Material awers = scriptableKarta.GetMaterialAwers();
        Material rewers = scriptableKarta.GetMaterialRewers();
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = awers;
        this.gameObject.transform.GetChild(1).GetComponent<Renderer>().material = rewers;
    }

    public Karta ScriptableKarta
    { 
        get { {  return scriptableKarta; } }
        set { scriptableKarta = value; }
    }

    public void ObrocKarte()
    {
        float x = gameObject.transform.localRotation.eulerAngles.x;
        float y = gameObject.transform.localRotation.eulerAngles.y;
        float z = gameObject.transform.localRotation.eulerAngles.z;
        //Debug.Log("Rotacja x = " + x + " Rotacja y = " + y + " Rotacja z = " + z);
        float obrotY = 0f;
        if ((scriptableKarta as KartaSciezki).CzyKartaObrócona == false) 
        {
            //Debug.Log("Karta nie jest obrócona.");
            obrotY = 180f; 
        }
        else 
        {
            //Debug.Log("Karta obrócona.");
            obrotY = 0f; 
        }
        //gameObject.transform.rotation = Quaternion.Euler(x, obrotY, z); // zobaczyæ to
        transform.localRotation = Quaternion.Euler(x, obrotY, z);
        (scriptableKarta as KartaSciezki).ObróæKarte();
             
    }

    public void OdkryjKarte()
    {
        float obrotZ = 0f;
        //Debug.Log($"{this.gameObject.name} scriptableKarta.CzyKartaOdkryta = {scriptableKarta.CzyKartaOdkryta}");
        if (scriptableKarta.CzyKartaOdkryta == false || scriptableKarta.CzyKartaOdkryta == true) 
        { 
            obrotZ = 180f;
            //Debug.Log("Ustawiamy obrót Z na 180 przy obrocie kart");
        }
        //else { obrotZ = 0f; }
        scriptableKarta.CzyKartaOdkryta = true;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, obrotZ);
    }

    public void ZakryjKarte()
    {
        float obrotZ = 0f;
        //Debug.Log($"{this.gameObject.name} scriptableKarta.CzyKartaOdkryta = {scriptableKarta.CzyKartaOdkryta}");
        if (scriptableKarta.CzyKartaOdkryta == false || scriptableKarta.CzyKartaOdkryta == true)
        {
            obrotZ = 0f;
            //Debug.Log("Ustawiamy obrót Z na 0 przy obrocie kart");
        }
        //else { obrotZ = 0f; }
        scriptableKarta.CzyKartaOdkryta = false;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, obrotZ);
    }
}
