using UnityEngine;
using System;
using System.Collections;

namespace Rabyrinth.ReadOnlys
{
    public static class Defines
    {
        public const string NTP_PATH = "time.google.com";

        //리소스 파일 경로
        public const string RESOURCE_OBJECT_PLAYER = "Object/Player";
        public const string RESOURCE_OBJECT_MAIN_CAMERA = "Object/MainCamera";
        public const string RESOURCE_OBJECT_UI_CAMERA = "Object/UI Camera";
        public const string RESOURCE_OBJECT_ENEMY_MUSHROOM = "Object/EnemyMushroom";
        public const string RESOURCE_OBJECT_ENEMY_GHOST = "Object/EnemyGhost";
        public const string RESOURCE_OBJECT_BULLET = "Object/Bullet";

        public const string MANAGER_GAMERESTARTER = "Manager/GameRestarter";
        public const string CHARACTER_PLAYER_PATH = "Character/Player/Player";

        //카메라
        public const string MAIN_CAM_PATH = "Camera/Main_Camera";
        public const string UI_CAM_PATH = "Camera/UI_Camera";

        //스킬 리소스
        public const string RESOURCE_OBJECT_SKILL_1 = "Object/SKILL_1";
        public const string RESOURCE_OBJECT_SKILL_3 = "Object/SKILL_3";


        public const string RESOURCE_SOUND_BGM = "Sounds/BGM";
        public const string RESOURCE_SOUND_SFX = "Sounds/SFX";

        public const string RESOURCE_LOADING_PATH = "UI/Loading";
        public const string RESOURCE_UI_PATH = "UI/UI";

        public const string RESOURCE_SPAWN_MANAGER_PATH = "Object/SpawnManager";
        public const string RESOURCE_SOUND_MANAGER_PATH = "Object/SoundManager";

        public const string UI_MAIN_PATH = "UI/Main_UI";
        public const string UI_LOGO_PATH = "UI/Logo";
        public const string UI_MAGIC_PATH = "UI/Magic";


        //Define
        public const string MONSTER_NAME = "Monster_";

        //Tag
        public const string TAG_NPC = "NPC";
        public const string TAG_PLAYER = "Player";

        public const string ANI_PARAM_POP = "isPop";
        public const string ANI_PARAM_ATTACK = "isAttack";
        public const string ANI_PARAM_TRACE = "isTrace";
        public const string ANI_PARAM_DIE = "isDie";
        public const string ANI_PARAM_BOOL_DIE = "bDie";
        public const string ANI_PARAM_Hit = "isHit";
        public const string ANI_PARAM_USE_SKILL = "UseSkill";

        public const string SHADER_PROPERTY_EMISSION = "_Emission";

        //public const string CSV_PARSING_BALLDATA = "CSVdatas/BallData";

        public const int SKILL_MAX_INDEX = 8;

        public const int BOSS_KPM = 40; 

        public enum LoadingIndex
        {
            BackGroundImage = 0,
            ProgressBar = 1,
            LodingText = 2,
        };
        //씬 번호
        public enum SceneIndex
        {
            Logo = 0,
            Loading = 1,
            Stage1 = 2,
            Stage2,
            Stage3,
        };
        //스테이지 최대 필드 수
        public enum Stage_Field_MaxIndex
        {
            Stage1 = 6,
            Stage2 = 6,
        };


    }


    public enum DDB_Table
    {
        PlayerDataTable = 0,
        GameDataTable = 1,
        AuthDataTable = 2,
        RankingDataTable = 3,
    }

    public enum UI_Pool_Object
    {
        HP_Bar = 0,
        Damage_Text,
    }

    //캐릭터 현재 상태
    public enum CharacterState
    {
        idle = 0,
        trace,
        attack,
        skill,
        die,
        hit,
    };

    public enum ShakeType
    {
        def = 0,
        ver,
        hor,
    }

    public enum SkillType
    {
        Meteor = 0,
        IceField,
        DimensionField,
        PlassmaSword,
        FlameSword,
        RecoveryField,
        SP_RecoveryField,
        Barrier,
    };

    public enum WeaponState
    {
        Default = 0,
        Elect,
        Fire,
        Ice,
    }

    public enum HitEffect
    {
        Elect = 0,
        Fire,
        Default,
        Freez,
        Dizziness,
        RecoveryHP,
        RecoverySP,
    };


    public enum UI_GetChild
    {
        TopBar = 0,
        KpmBar = 1,
        SpBar = 2,
        SkillButton = 3,
        MainMenu = 4,
        HpBarPool = 5,
        DamagePool = 6,
        CharDamagePool = 7,
        RewordTextPool = 8,
        PlayerHpBar = 9,
        Target_HpBar = 10,
        PopupMenu = 11,

    };
    public enum DamageText_Type
    {
        Player = 0,
        NPC = 1,
        Reword = 2,
    };
    
    //public enum Stage
    //{

    //}
    //public enum BGMdatas
    //{

    //}

    //public enum SFXdatas
    //{

    //}

}