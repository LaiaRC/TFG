using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInfo : Resource
{
    public string villagerName;
    public Sprite villagerIcon;

    public DropInfo(string id, string resourceName, string villagerName, Sprite icon, Sprite villagerIcon)
    {
        this.id = id;
        this.resourceName = resourceName;
        this.villagerName = villagerName;
        this.icon = icon;
        this.villagerIcon = villagerIcon;
    }
}
