using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DataClass
{
    public Inventory inven;
    public AvatarCloth avatar_cloth;
}

public class Inventory
{
    public List<string> hair_item;
    public List<string> top_item;
    public List<string> bottom_item;
    public List<string> shoes_item;
    
    public Inventory(List<string> hair_item, List<string> top_item, List<string> bottom_item, List<string> shoes_item)
    {
        this.hair_item = hair_item;
        this.top_item = top_item;
        this.bottom_item = bottom_item;
        this.shoes_item = shoes_item;
    }

    public Inventory()
    {
        hair_item = new List<string>();
        top_item = new List<string>();
        bottom_item = new List<string>();
        shoes_item = new List<string>();
    }
}
public class AvatarCloth
{
    public string hair;
    public string top;
    public string bottom;
    public string shoes;

    public AvatarCloth(string hair, string top, string bottom, string shoes)
    {
        this.hair = hair;
        this.top = top;
        this.bottom = bottom;
        this.shoes = shoes;
    }

    public AvatarCloth()
    {
        hair = "";
        top = "";
        bottom = "";
        shoes = "";
    }
}
