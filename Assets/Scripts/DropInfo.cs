using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInfo : MonoBehaviour
{
    public string id;
    public string dropName;
    public string villagerName;
    public Sprite icon;
    public Sprite villagerIcon;

    public DropInfo(string id, string dropName, string villagerName, Sprite icon, Sprite villagerIcon)
    {
        this.id = id;
        this.dropName = dropName;
        this.villagerName = villagerName;
        this.icon = icon;
        this.villagerIcon = villagerIcon;
    }
}
