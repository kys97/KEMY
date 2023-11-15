using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ResourcesManager : MonoBehaviour
{
    public Dictionary<string, Sprite> Sprites;
    public Dictionary<string, Sprite> ItemImage;//Character, Face, Acessary
    public Dictionary<string, Material> ItemMaterials;//Material

    public void Init()
    {
        SpriteLoad();
        ItemLoad();
    }

    private void SpriteLoad()
    {
        Sprites = new Dictionary<string, Sprite>();

        for(int i=0; i < (int)sprites.Count; i++)
        {
            Sprites.Add(((sprites)i).ToString(), Resources.Load<Sprite>("Sprites/" + ((sprites)i).ToString()));
        }
    }

    private void ItemLoad()
    {
        ItemImage = new Dictionary<string, Sprite>();
        ItemMaterials = new Dictionary<string, Material>();

        for (int i = 0; i < (int)skin.Count; i++)
        {
            ItemImage.Add(((skin)i).ToString(), Resources.Load<Sprite>("Sprites/Skin/" + ((skin)i).ToString()));
            ItemMaterials.Add(((skin)i).ToString(), Resources.Load<Material>("Materials/Skin/" + ((skin)i).ToString()));
        }
        for (int i = 0; i < (int)face.Count; i++)
        {
            ItemImage.Add(((face)i).ToString(), Resources.Load<Sprite>("Sprites/Face/" + ((face)i).ToString()));
            ItemMaterials.Add(((face)i).ToString(), Resources.Load<Material>("Materials/Face/" + ((face)i).ToString()));
        }
        /*
        for (int i = 0; i < (int)head.Count; i++)
        {
            ItemImage.Add(((head)i).ToString(), Resources.Load<Sprite>("Sprites/Head/" + ((head)i).ToString()));
            ItemMaterials.Add(((head)i).ToString(), Resources.Load<Material>("Materials/Head/" + ((head)i).ToString()));
        }
        for (int i = 0; i < (int)body.Count; i++)
        {
            ItemImage.Add(((body)i).ToString(), Resources.Load<Sprite>("Sprites/Body/" + ((body)i).ToString()));
            ItemMaterials.Add(((body)i).ToString(), Resources.Load<Material>("Materials/Body/" + ((body)i).ToString()));
        }
        */
    }

}
