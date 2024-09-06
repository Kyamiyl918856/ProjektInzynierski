using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nowa Karta Sciezki", menuName = "Karta Sciezki")]
public class KartaSciezki : Karta
{
    [SerializeField]protected bool północ;
    [SerializeField]protected bool południe;
    [SerializeField]protected bool zachód;
    [SerializeField]protected bool wschód;
    [SerializeField]protected TypSciezki typSciezki;
    [SerializeField]protected bool czyKartaObrócona = false;
    [SerializeField]protected List<GameObject> listaPionkowStojacychNaTejKarcie = new List<GameObject>();

    public bool Północ { get { return północ; } }
    public bool Południe { get { return południe; } }
    public bool Zachód { get { return zachód; } }
    public bool Wschód { get { return wschód; } }

    public TypSciezki _TypSciezki
    {
        get { return typSciezki; }
    }

    public bool CzyKartaObrócona
    {
        get { return czyKartaObrócona; }
        set { czyKartaObrócona = value; }
    }

    public List<GameObject> ListaPionkowStojacychNaTejKarcie
    {
        get { return listaPionkowStojacychNaTejKarcie; }
    }
   

    public enum TypSciezki
    {
        Zwykła,
        Przeszkoda,
        Kamienie,
        Ognisko
    }

    public override string ToString()
    {
        string czyObrócona = "";
        if (CzyKartaObrócona) { czyObrócona = "Obrócona"; }
        else { czyObrócona = "nieobrócona"; }

        //return base.ToString() + " " + typSciezki.ToString() + " " + północ.ToString() + " " + południe.ToString() + " " + zachód.ToString() + " " + wschód.ToString() + " " + obrócona;
        return base.ToString() + " " + typSciezki.ToString();
    }

    public virtual void ObróćKarte()
    {
        CzyKartaObrócona = !CzyKartaObrócona;

        bool tmp = północ;
        północ = południe;
        południe = tmp;

        tmp = zachód;
        zachód = wschód;
        wschód = tmp;

        Debug.Log(ToString());

        // dekonstrukcja (zamiast używać zmiennej tymczasowej tmp)
        //(północ, południe, zachód, wschód) = (południe, północ, wschód, zachód);
    }
}
