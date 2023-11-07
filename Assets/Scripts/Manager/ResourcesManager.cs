using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public Dictionary<string, Sprite> Sprites;
    [SerializeField] public Dictionary<string, Sprite> Item;//Cloth

    public void Init()
    {
        SpriteLoad();
        ItemLoad();
    }

    private void SpriteLoad()
    {
        Sprites = new Dictionary<string, Sprite>();

        for(int i=0; i < (int)Define.sprites.Count; i++)
        {
            Sprites.Add(((Define.sprites)i).ToString(), Resources.Load<Sprite>("Sprites/" + ((Define.sprites)i).ToString()));
        }
    }

    private void ItemLoad()
    {
        Item = new Dictionary<string, Sprite>();

        for (int i = 0; i < (int)Define.hair_item.Count; i++)
        {
            Item.Add(((Define.hair_item)i).ToString(), Resources.Load<Sprite>("Sprites/Item/" + ((Define.hair_item)i).ToString()));
        }
        for (int i = 0; i < (int)Define.top_item.Count; i++)
        {
            Item.Add(((Define.top_item)i).ToString(), Resources.Load<Sprite>("Sprites/Item/" + ((Define.top_item)i).ToString()));
        }
        for (int i = 0; i < (int)Define.bottom_item.Count; i++)
        {
            Item.Add(((Define.bottom_item)i).ToString(), Resources.Load<Sprite>("Sprites/Item/" + ((Define.bottom_item)i).ToString()));
        }
        for (int i = 0; i < (int)Define.shoes_item.Count; i++)
        {
            Item.Add(((Define.shoes_item)i).ToString(), Resources.Load<Sprite>("Sprites/Item/" + ((Define.shoes_item)i).ToString()));
        }
    }
}
