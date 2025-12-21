

using System.Collections.Generic;
using UnityEngine;

namespace AroundEffect
{

    public static class Constants
    {
        public static class Animator
        {
            public const string TRIGGER_IDLE = "t_idle";
            public const string TRIGGER_JUMP = "t_jump";
            public const string TRIGGER_CLEAR = "t_clear";
            public const string TRIGGER_FAIL = "t_fail";
            public const string TRIGGER_CRASH = "t_crash";
            public const string TRIGGER_TOK = "t_tok";
            public const string TRIGGER_TOUCH = "t_touch";

            public const string FLOAT_IDLE = "idleNum";
            public const string FLOAT_FAIL = "failNum";
            public const string FLOAT_TOUCH_DIRECTION = "touchDirection";

            public const string BOOL_JUMP = "isJump";

            public const string BOOL_MOVE = "isMove";
            public const string BOOL_MOVE_START = "isMoveStart";

            public const string INT_MOVE = "MoveNum";
        }

        /// <summary>
        /// 10/27/2023-LYI
        /// ES3.Save, Load strings
        /// </summary>
        public static class ES3
        {
            public const string GAME_LANGUAGE = "GameLanguage";

            public const string LAST_STAGE = "LastStage";
            public const string LAST_STAGE_TYPE = "LastStageType";
            public const string LAST_STAGE_NUM = "LastStageNum";

            public const string TABLE_POSITION = "TablePosition";
            public const string TABLE_SCALE = "TableScale";

            public const string IS_TUTORIAL = "isTutorial";
            public const string IS_LEFT_HANDED = "isLeftHanded";

            public const string LAST_SELECT_HEADER = "LastSelectHeader";
        }

        public static class CSVPath
        {

        }

        public static class Sound
        {
            public const string BGM_VOLUME = "BGMVolume";
            public const string SFX_VOLUME = "SFXVolume";


            public const string BGM_STAGE = "BGM_Episode03_201117";

            public const string BGM_TUTORIAL = "BGM_Title";
            public const string BGM_MAZE = "BGM_Episode01_201117";
            public const string BGM_CROSSING = "BGM_Episode02_201117";
            public const string BGM_MEMORY = "BGM_Imagine_201117";
            public const string BGM_BLOCK = "BGM_Episode03_201117";

            public const string BGM_MENU = "BGM_EpisodeMain";
            public const string BGM_HEADER_SELECT = "BGM_EpisodeSelect";
            public const string BGM_CREDIT = "BGM_EpisodeEnding";


            public const string SFX_UI_BUTTON_CLICK = "SFX 01 Select";


            public const string SFX_HEADER_FOOT = "jump_15";
            public const string SFX_HEADER_JUMP = "Notification sound 14";
            public const string SFX_HEADER_LANDED = "Notification sound 14";

            public const string SFX_HEADER_CRUSH = "SFX 03 Crush";
            public const string SFX_HEADER_DIE_WATER = "sound_wash";
            public const string SFX_WATER_SPLASH = "Water Impact 4";
            public const string SFX_HEADER_DIE_HIT = "jump_29";


            public const string SFX_PLAY_TOK = "Fing_02_Mid";
            public const string SFX_PLAY_TOK_2 = "Fing_01_Mid";

            public const string SFX_INTERACTION_BUTTON_CLICK = "RPG_Menu_move_03";
            public const string SFX_INTERACTION_BUTTON_DEACTIVATE = "RPG_Menu_move_02";
            public const string SFX_INTERACTION_BUTTON_ACTIVE = "RPG_Menu_move_01";

            public const string SFX_INTERACTION_DOOR_OPEN = "Door 3 Open";
            public const string SFX_INTERACTION_KEY_GET = "Collect star 2";

            public const string SFX_INTERACTION_TELEPORT = "etfx_explosion_minimagic";

            public const string SFX_INTERACTION_JUMP_BOUNCE = "SFX 07 Jump";

            public const string SFX_INTERACTION_CANNON_READY = "Cannon recharge 1";
            public const string SFX_INTERACTION_CANNON_FIRE = "Cannon shots 1";

            public const string SFX_INTERACTION_HAND_MARKER = "etfx_explosion_minimagic";

            public const string SFX_TRAP_SPIKE_OUT = "Spikes 2_2";
            public const string SFX_TRAP_SPIKE_HIT = "jump_29";

            public const string SFX_TRAP_MISSILE_SHOOT = "etfx_shoot_energy03";
            public const string SFX_TRAP_MISSILE_HIT = "etfx_explosion_sharp";

            public const string SFX_TRAP_FLAMETHROWER = "Blow Torch Start 2";
            public const string SFX_TRAP_FLAME_HIT = "Fire Burst 7";

            public const string SFX_TRAP_PRESS = "etfx_explosion_dark02";


            public const string SFX_BLOCK_FALL_SHAKE = "Error Sound 5";
            public const string SFX_BLOCK_FALL_FALLING = "Error Sound 4";
            public const string SFX_BLOCK_FALL_RESPAWN = "Error Sound 16";
            public const string SFX_BLOCK_FALL_DISAPPEAR = "Boing sound 5";


            public const string SFX_MEMORY_PANNAL = "sfx_poop";
            public const string SFX_MEMORY_SUCCESS = "14doorNL";
            public const string SFX_MEMORY_FAILURE = "14short2NL";

            public const string SFX_STAGE_PORTAL = "Magic Element 3_1"; //Success 7
            public const string SFX_STAGE_CLEAR = "SFX 04 Success";


            public const string SFX_CHEER_FIREWORK = "Fire Burst 2";



            //TTS
            //public const string TTS_0_KOR = "AR_NumberCard_Kor_00";
            //public const string TTS_1_KOR = "AR_NumberCard_Kor_01";
            //public const string TTS_2_KOR = "AR_NumberCard_Kor_02";
            //public const string TTS_3_KOR = "AR_NumberCard_Kor_03";
            //public const string TTS_4_KOR = "AR_NumberCard_Kor_04";
            //public const string TTS_5_KOR = "AR_NumberCard_Kor_05";
            //public const string TTS_6_KOR = "AR_NumberCard_Kor_06";
            //public const string TTS_7_KOR = "AR_NumberCard_Kor_07";
            //public const string TTS_8_KOR = "AR_NumberCard_Kor_08";
            //public const string TTS_9_KOR = "AR_NumberCard_Kor_09";
            //public const string TTS_10_KOR = "AR_NumberCard_Kor_10";
            //public const string TTS_PLUS_KOR = "AR_NumberCard_Kor_Plus";
            //public const string TTS_MINUS_KOR = "AR_NumberCard_Kor_Minus";
            //public const string TTS_EQUALS_KOR = "AR_NumberCard_Kor_Equals";


            //public const string TTS_0_ENG = "AR_NumberCard_00_ENG";
            //public const string TTS_1_ENG = "AR_NumberCard_01_ENG";
            //public const string TTS_2_ENG = "AR_NumberCard_02_ENG";
            //public const string TTS_3_ENG = "AR_NumberCard_03_ENG";
            //public const string TTS_4_ENG = "AR_NumberCard_04_ENG";
            //public const string TTS_5_ENG = "AR_NumberCard_Eng-05";
            //public const string TTS_6_ENG = "AR_NumberCard_Eng-06";
            //public const string TTS_7_ENG = "AR_NumberCard_Eng-07";
            //public const string TTS_8_ENG = "AR_NumberCard_Eng-08";
            //public const string TTS_9_ENG = "AR_NumberCard_Eng-09";
            //public const string TTS_10_ENG = "AR_NumberCard_Eng-10";
            //public const string TTS_PLUS_ENG = "AR_NumberCard_Eng_Plus";
            //public const string TTS_MINUS_ENG = "AR_NumberCard_Eng_Minus";
            //public const string TTS_EQUALS_ENG = "AR_NumberCard_Eng_Equals";



        }


        public static class Particle
        {

        }
        public static class Layer
        {
            public const string LAYERMASK_DEFAULT = "Default";
            public const string LAYERMASK_TRANSPARENT_FX = "TransparentFX";
            public const string LAYERMASK_IGNORE = "Ignore Raycast";
            public const string LAYERMASK_WATER = "Water";
            public const string LAYERMASK_UI = "UI";

            //start with layer 6
            public const string LAYERMASK_GROUND = "Ground";
            public const string LAYERMASK_CHARACTER = "Character";
            public const string LAYERMASK_TOUCH = "Touch";
            public const string LAYERMASK_HAND = "Hand";

            public const string LAYERMASK_TRAP = "Trap";
            public const string LAYERMASK_POSTPROCESSING = "Postprocessing";
            public const string LAYERMASK_TOKMARKER = "TokMarker";
            public const string LAYERMASK_INTERACT = "Interact";
            public const string LAYERMASK_OBSTACLE_ONLY = "ObstacleOnly";

        }

        public static class InteractName
        {
            public const string INTERACT_BUTTON = "Tok_Button";
            public const string INTERACT_PILLAR = "Tok_Pillar";
            public const string INTERACT_JUMP_BLOCK = "Tok_JumpBlock";
        }

        /// <summary>
        /// 6/13/2024-LYI
        /// Labels of Addressable
        /// </summary>
        public static class Label
        {
            public const string LABEL_STAGE = "Stage";
            public const string LABEL_AUDIO_CLIP = "AudioClip";
        }

        public static class TAG
        {
            public const string TAG_GROUND = "Ground";
            public const string TAG_CHARACTER = "Character";
            public const string TAG_HAND = "Hand";

        }

    }
}