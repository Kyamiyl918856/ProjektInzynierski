using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    [SerializeField]private List<GameObject> listaKartDoGrania = new List<GameObject>();
    [SerializeField]private List<GameObject> stosKartDoDobierania = new List<GameObject>();
    [SerializeField]private List<GameObject> stosKartOdrzuconych = new List<GameObject>();
    [SerializeField]private List<GameObject> listaKartKrasnoludow = new List<GameObject>();
    [SerializeField]private List<GameObject> stosKartKrasnoludowDoRozdania = new List<GameObject>();
    [SerializeField]private List<GameObject> listaKartSkarbu = new List<GameObject>();
    [SerializeField] private List<GameObject> listaKartSkarbuDoBrania = new List<GameObject>();
    [SerializeField]private List<GameObject> listaKartStartu = new List<GameObject>();
    [SerializeField]private List<GameObject> listaKartKopalni = new List<GameObject>();
    [SerializeField] private List<GameObject> listaPionkow = new List<GameObject>();
    [SerializeField] private List<GameObject> znacznikiTopory = new List<GameObject>();
    [SerializeField] private List<GameObject> znacznikiLiny = new List<GameObject>();
    [SerializeField] private List<GameObject> znacznikiLodki = new List<GameObject>();

    public List<GameObject> getListaKartDoGrania()
    {
        return listaKartDoGrania;
    }
    public List<GameObject> getStosKartDoDobierania()
    {
        return stosKartDoDobierania;
    }
    public List<GameObject> getStosKartOdrzuconych()
    {
        return stosKartOdrzuconych;
    }
    public List<GameObject> getListaKartKrasnoludow()
    {
        return listaKartKrasnoludow;
    }
    public List<GameObject> getStosKartKrasnoludowDoRozdania()
    {
        return stosKartKrasnoludowDoRozdania;
    }
    public List<GameObject> getListaKartSkarbu()
    {
        return listaKartSkarbu;
    }
    public List<GameObject> getListaKartSkarbuDoBrania()
    {
        return listaKartSkarbuDoBrania;
    }
    public List<GameObject> getListaKartStartu()
    {
        return listaKartStartu;
    }
    public List<GameObject> getListaKartKopalni()
    {
        return listaKartKopalni;
    }
    public List<GameObject> getListaPionkow()
    {
        return listaPionkow;
    }
    public List<GameObject> GetListaZnacznikiTopory()
    {
        return znacznikiTopory;
    }
    public List<GameObject> GetListaZnacznikiLiny()
    {
        return znacznikiLiny;
    }
    public List<GameObject> GetListaZnacznikiLodki()
    {
        return znacznikiLodki;
    }

    public override void OnStartAuthority()
    {
        //Debug.Log("OnStartAuthority() - w CardManager.cs");
    }

    private void Update()
    {
        //Debug.Log("Update() - w CardManager.cs");

        if(hasAuthority)
        {
            //Debug.Log("Update() hasAuthority - w CardManager.cs");
        }
    }

    private void Start()
    {
        
    }
}
