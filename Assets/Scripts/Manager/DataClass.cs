using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DataClass
{
    public MyInfo info;
    public Inventory inven;
    public AvatarInfo avatar_info;

    public DataClass(MyInfo info, Inventory inven, AvatarInfo avatar_info)
    {
        this.info = info;
        this.inven = inven;
        this.avatar_info = avatar_info;
    }

    public DataClass()
    {
        info = new MyInfo();
        inven = new Inventory();
        avatar_info = new AvatarInfo();
    }
}

public class MyInfo
{
    public int coin;
    public int heart;

    public MyInfo(int coin, int heart)
    {
        this.coin = coin;
        this.heart = heart;
    }

    public MyInfo() 
    {
        coin = 0;
        heart = 0;
    }
}

public class Inventory
{
    public List<string> skin;
    public List<string> face;
    //public List<string> head_item;
    //public List<string> body_item;
    
    public Inventory(List<string> skin, List<string> face/*, List<string> head_item, List<string> body_item*/)
    {
        this.skin = skin;
        this.face = face;
        //this.head_item = head_item;
        //this.body_item = body_item;
    }

    public Inventory()
    {
        skin = new List<string>();
        face = new List<string>();
        //head_item = new List<string>();
        //body_item = new List<string>();

        //기본 아이템
        skin.Add("Cat00");
        face.Add("Face00");
    }
}
public class AvatarInfo
{
    public string skin;
    public string face;
    //public string head_item;
    //public string body_item;

    public AvatarInfo(string skin, string face/*, string head_item, string body_item*/)
    {
        this.skin = skin;
        this.face = face;
        //this.head_item = head_item;
        //this.body_item = body_item;
    }

    public AvatarInfo()
    {
        skin = "Cat00";
        face = "Face00";
        //head_item = "";
        //body_item = "";
    }
}
