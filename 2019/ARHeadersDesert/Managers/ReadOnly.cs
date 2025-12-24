using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.ReadOnly
{
    public static class Defines
    {
        public const string CSV_LEVEL_DATA = "LevelData";
        public const string CSV_UI_SHOP_ENG = "ShopUI_eng";
        public const string CSV_UI_SHOP_KOR = "ShopUI_kor";
        public const string CSV_MISSILE_DATA = "MissileData";
        public const string CSV_UITEXT_DATA_KOR = "UITexts_kor";
        public const string CSV_UITEXT_DATA_ENG = "UITexts_eng";
        public const string CSV_DIALOG_DATA_KOR = "Dialog_kor";
        public const string CSV_DIALOG_DATA_ENG = "Dialog_eng";
        public const string CSV_DIALOG_KANTO_KOR = "KantoDialog_kor";
        public const string CSV_DIALOG_KANTO_ENG = "KantoDialog_eng";

        public const string PREFAB_UI_CANVAS = "UI_Canvas";
        public const string PREFAB_UI_RESULT = "UI_Result";
        public const string PREFAB_UI_GAME = "UI_Game";

        //스테이지, 캐릭터 프리팹
        public const string PREFAB_STAGE = "Stage";
        public const string PREFAB_STAGE_ISLAND = "StageIsland";
        public const string PREFAB_STAGE_WATER = "WaterStage";

        public const string PREFAB_MISSILEMANAGER = "MissileManager";
        public const string PREFAB_MISSILE = "BubbleMissile";
        public const string PREFAB_KANTO = "Kanto";
        public const string PREFAB_ZENORA = "Zenora";

        public const string PREFAB_ENEMY_CLOUD = "Enemy_Cloud";

        public const string PREFAB_BUTTON_LEVEL = "LevelButton";

        public const string PREFAB_TOON_RAT = "toon_Tena";
        public const string PREFAB_TOON_GIRRAFE = "toon_Canto";
        public const string PREFAB_TOON_ZEBRA = "toon_Ginora";
        public const string PREFAB_TOON_RHINO = "toon_Oodada";
        public const string PREFAB_TOON_ELEPHANT = "toon_Copan";
        public const string PREFAB_TOON_PIG = "toon_Doying";
        public const string PREFAB_TOON_BLACKCLOUD = "toon_GGamung";

        //스프라이트 이미지
        public const string SPRITE_TEXT_VICTORY = "txt_victory_red";
        public const string SPRITE_TEXT_DEFEAT = "txt_defeat";
        public const string SPRITE_TEXT_LEVEL = "txt_level";
        public const string SPRITE_TEXT_READY = "txt_ready";
        public const string SPRITE_TEXT_START = "txt_start";

        public const string SPRITE_TEXTBOX_NORMAL = "shop_itme_bg";
        public const string SPRITE_TEXTBOX_ANGRY = "shop_itme_bg";
        public const string SPRITE_TEXTBOX_HAPPY = "shop_itme_bg";
        public const string SPRITE_TEXTBOX_SAD = "shop_itme_bg";

        public const string SPRITE_EMOTE_ANGRY = "shop_itme_bg";
        public const string SPRITE_EMOTE_HAPPY = "shop_itme_bg";
        public const string SPRITE_EMOTE_SAD = "shop_itme_bg";

        public const string SPRITE_CHARACTER_RAT = "Tena_toon";
        public const string SPRITE_CHARACTER_GIRRAFE = "Canto_toon";
        public const string SPRITE_CHARACTER_ZEBRA = "Ginora_toon";
        public const string SPRITE_CHARACTER_RHINO = "Oodada_toon";
        public const string SPRITE_CHARACTER_ELEPHANT = "Copan_toon";
        public const string SPRITE_CHARACTER_PIG = "Pig_toon";
        public const string SPRITE_CHARACTER_HEADERS = "Headers";
        public const string SPRITE_CHARACTER_MINICLOUD = "MiniCloud";
        public const string SPRITE_CHARACTER_BLACKCLOUD = "GGamung_toon";

        public const string SPRITE_BUTTON_RED = "btn_red";
        public const string SPRITE_BUTTON_GREEN = "btn_green";
        public const string SPRITE_BUTTON_KOREAN = "btn_language2_kor2";
        public const string SPRITE_BUTTON_ENGLISH = "btn_language2_eng2";

        public const string SPRITE_ICON_BUBBLE_NORMAL = "normalBubble";
        public const string SPRITE_ICON_BUBBLE_SPREAD = "normalBubble";
        public const string SPRITE_ICON_BUBBLE_POWER = "normalBubble";
        public const string SPRITE_ICON_BUBBLE_REPEAT = "normalBubble";

        public const string ANIMATOR_SLIDELEFT = "SlideLeft";
        public const string ANIMATOR_SLIDERIGHT = "SlideRight";
        public const string ANIMATOR_CHARACTER_GIRRAFE = "Girrafe";


        //오디오 클립
        public const string SOUND_BGM_0 = "Casual Theme Loop #1";
        public const string SOUND_BGM_1 = "Casual Theme Loop #2";
        public const string SOUND_BGM_2 = "Casual Theme Loop #3";

        public const string SOUND_SFX_BUTTON = "sfx_pop3";

        public const string SOUND_SFX_COUNT = "sfx_racebeep";
        public const string SOUND_SFX_START = "sfx_racestart";
        public const string SOUND_SFX_WIN = "sfx_win0";
        public const string SOUND_SFX_LOSE = "sfx_fail0";

        public const string SOUND_SFX_BUBBLE = "sfx_pop2";
        public const string SOUND_SFX_POP = "sfx_pop1";
        public const string SOUND_SFX_WATERSPLASH = "sfx_pop0";

        public const string SOUND_SFX_MOVE = "sfx_bt";
        public const string SOUND_SFX_FAST = "sfx_fast";
        public const string SOUND_SFX_HIT = "sfx_clean0";
        public const string SOUND_SFX_CLEAN = "sfx_clean1";

        //Define
        public const string ANIM_TRIGGER_HIT = "isHit";
        public const string ANIM_TRIGGER_ENTER = "isEnter";
        public const string ANIM_TRIGGER_CLEAN = "isDie";
        public const string ANIM_TRIGGER_WALK = "isWalk";
        public const string ANIM_BOOL_FLEE = "isFlee";
        public const string ANIM_BOOL_FEAR = "isFear";
        public const string ANIM_BOOL_PATROL = "isPatrol";
        public const string ANIM_BOOL_FLIP = "isFlip";
        public const string ANIM_INT_IDLE = "Idle";
        public const string ANIM_INT_CHARTYPE = "CharType";


        public const string SAVE_INT_CLEAR_LEVEL = "ClearLevel";
        public const string SAVE_INT_TUTORIAL_NUMBER = "TutorialNum";
        public const string SAVE_INT_COIN = "Coin";
        public const string SAVE_INT_LANGUAGE = "Language";

        public const string SAVE_INT_LEVEL_DAMAGE = "LvDamage";
        public const string SAVE_INT_LEVEL_SPEED = "LvSpeed";
        public const string SAVE_INT_LEVEL_DELAY = "LvDelay";
    }
}