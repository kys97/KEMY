using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum scene
    {
        Main,
        Kamp,
        Quiz,
        RunningGame,
        Count
    }
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
        Home,
        Chat,
        Game,
        Kamp,
        Inventory,
        Shop,
        ShopResult,
        QuizReady,
        QuizStart,
        QuizResult,
        Count
    }

    public enum sprites
    {
        item_egg,
        cracked1,
        cracked2,
        cracked3,
        home,
        home_select,
        chat,
        chat_select,
        book,
        book_select,
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
        none,
        Skin,
        Face,
        //Deco,
        //Wall,
        Count
    }

    public enum skin
    {
        Cat00,
        Cat01,
        Cat02,
        Cat03,
        Cat04,
        Cat05,
        Cat06,
        Cat07,
        Cat08,
        Cat09,
        Count
    }
    public enum face
    {
        Face00,
        Face01,
        Face02,
        Face03,
        Face04,
        Face05,
        Face06,
        Face07,
        Face08,
        Face09,
        Face10,
        Face11,
        Face12,
        Face13,
        Face14,
        Face15,
        Face16,
        Face17,
        Face18,
        Face19,
        Face20,
        Face21,
        Face22,
        Face23,
        Face24,
        Face25,
        Face26,
        Count
    }
    public enum head
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
    public enum body
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
}
