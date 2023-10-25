using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    Dictionary<string, Sprite> Sprites;

    public void Init()
    {
        SpriteLoad();
    }

    private void SpriteLoad()
    {
        Sprites = new Dictionary<string, Sprite>();

        for(int i=0; i < (int)Define.sprites.Count; i++)
        {
            Sprites.Add(((Define.sprites)i).ToString(), Resources.Load<Sprite>("Sprite/" + ((Define.sprites)i).ToString()));
        }
    }
}
