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

    public enum scene
    {
        Kamp,
        WordHunting,
        HangulRun
    }

    public enum ui
    {
        //Login,
        Main,
        Home,
        Game,
        Kamp,
        Inventory,
        CountPanel,
        WordHuntingGame,
        Count
    }

    public enum sprites
    {
        home,
        home_select,
        book,
        book_select,
        chat,
        chat_select,
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
        hair1,
        hair2,
        hair3,
        hair4,
        hair5,
        hair6,
        hair7,
        hair8,
        hair9,
        hair10,
        hair11,
        Count
    }
    public enum top_item
    {
        top1,
        top2,
        top3,
        top4,
        top5,
        top6,
        top7,
        top8,
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
