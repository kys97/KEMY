using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ui_level
    {
        Lev1 = 0,
        Lev2 = 1,
        Lev3 = 2,
        Count
    }
    public enum ui
    {
        //Login,
        Main,
        Home,
        Kamp,
        Inventory,
        CountPanel,
        WordHuntingGame,
        Count
    }

    public enum sprites
    {
        Count
    }

    public enum game_state
    {
        Ready,
        Start,
        End
    }

    public enum topic
    {
        Animal,
        Plant,
        Count
    }

    public enum item_type
    {
        Hair,
        Top,
        Bottom,
        Shoes,
        Count
    }

    public enum hair_item
    {
        Count
    }
    public enum top_item
    {
        Count
    }
    public enum bottom_item
    {
        Count
    }
    public enum shoes_item
    {
        Count
    }
}
