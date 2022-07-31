using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceName;
    public string id;
    public bool isPrimaryProduct; //Productes que es produeixen "gratis"
    public float time; //temps que tarda a crear-se
    public bool isUnlocked; //Jugador ha pagat el seu cost i ja pot crear-lo o no 
    public Requirement[] requirements; //resources necessaries x crear la resource (si no es primaryProduct)
    public Sprite icon;

    //public Resource[] production_list; //lista de recursos que necessiten aquesta resource x fabricar-se

    public Resource(string resourceName, string id, bool isPrimaryProduct, float time, bool isUnlocked, Requirement[] requirements, Sprite icon)
    {
        this.resourceName = resourceName;
        this.id = id;
        this.isPrimaryProduct = isPrimaryProduct;
        this.time = time;
        this.isUnlocked = isUnlocked;
        this.requirements = requirements;
        this.icon = icon;
    }

    public Resource() { }
}
