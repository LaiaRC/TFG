using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public Dictionary<string, GameObject> BUILDINGS = new Dictionary<string, GameObject>();
    public Dictionary<string, Resource> RESOURCES = new Dictionary<string, Resource>();
    public Dictionary<string, DropInfo> DROPS = new Dictionary<string, DropInfo>();
    public Dictionary<string, MonsterInfo> MONSTERS = new Dictionary<string, MonsterInfo>();
    public Dictionary<string, int> BUILDING_INVENTORY = new Dictionary<string, int>();

    /*********************DICTIONARIES SAVED****************************/
    public Dictionary<string, int> INVENTORY = new Dictionary<string, int>();
    public Dictionary<string, float[]> CONSTRUCTIONS = new Dictionary<string, float[]>(); //x i y posicio i la z el level del building (x no fer 2 diccionaris)
    public Dictionary<string, int> PLAYER = new Dictionary<string, int>();
    public Dictionary<string, int[]> MONSTERS_STATS = new Dictionary<string, int[]>(); //To save if each monster is unlocked and upgrade level
    public Dictionary<string, int> BOOSTS = new Dictionary<string, int>();

    public List<Sprite> resourcesIcons;
    public List<Sprite> monstersIcons;
    public List<Sprite> dropsIcons;
    public List<Sprite> villagersIcons;

    #region RESOURCES KEYS

    public static string BONE = "bone";
    public static string ROTTEN_FLESH = "rottenFlesh";
    public static string TOMBSTONE = "tombstone";
    public static string SPIDERWEB = "spiderweb";
    public static string LANTERN = "lantern";
    public static string DEADTREEBRANCH = "deadTreeBranch";
    public static string POISONIVY = "poisonIvy";
    public static string PUMPKIN = "pumpkin";
    public static string BLACKROSE = "blackRose";
    public static string MUD = "mud";
    public static string SWAMPWATER = "swampWater";
    public static string FROGLEG = "frogLeg";
    public static string DEADFISH = "deadFish";
    public static string BATWING = "batWing";
    public static string RUSTYNAIL = "rustyNail";
    public static string BANDAGES = "bandages";
    public static string EYE = "eye";
    public static string BLOOD = "blood";
    public static string PLASMA = "plasma";
    public static string OUIJABOARD = "ouijaBoard";
    public static string SPIRITSOUL = "spiritSoul";
    public static string GARGOYLESTONE = "gargoyleStone";
    public static string DEATHESSENCE = "deathEssence";
    public static string COFFIN = "coffin";
    public static string BLACKCATHAIR = "blackCatHair";
    public static string BROOMSTICK = "broomstick";
    public static string WITCHHAT = "witchHat";
    public static string SKULL = "skull";
    public static string HORNS = "horns";
    public static string HELLFIRE = "hellfire";
    public static string DIVINATIONBALL = "divinationBall";
    public static string IMMORTALITYELIXIR = "immortalityElixir";
    public static string SPELLBOOK = "spellbook";
    public static string POISONAPPLE = "poisonApple";
    public static string FANG = "fang";
    public static string WOLFCLAW = "wolfClaw";

    #endregion RESOURCES KEYS

    #region DROPS KEYS

    public static string SCARE = "scare";
    public static string LOLLIPOP = "lollipop";
    public static string RING = "ring";
    public static string BEER = "beer";
    public static string SWORD = "sword";
    public static string SHIELD = "shield";
    public static string STICK = "stick";
    public static string GEM = "gem";

    #endregion DROPS KEYS

    #region MONSTER KEYS

    public static string SKELETON = "skeleton";
    public static string ZOMBIE = "zombie";
    public static string GHOST = "ghost";
    public static string JACK_LANTERN = "jackOLantern";
    public static string BAT = "bat";
    public static string GOBLIN = "goblin";
    public static string VAMPIRE = "vampire";
    public static string WITCH = "witch";
    public static string CLOWN = "clown";
    public static string REAPER = "reaper";

    #endregion MONSTER KEYS

    #region BOOSTS KEYS

    public static string PRODUCER_BOOST = "gargoyle";
    public static string CONVERTER_BOOST = "obelisk";
    public static string SUMMONING_BOOST = "bloodMoonTower";
    public static string OFFLINE_MAXTIME_BOOST = "spectre";
    public static string OFFLINE_PRODUCTIVITY_BOOST = "necromancer";
    public static string SCARES_BOOST = "mageGuardian";
    public static string DROPS_BOOST = "demonLord";
    public static string MERCHANT_BOOST = "merchant";

    #endregion BOOSTS KEYS

    #region CONSTRUCTION KEYS

    public static int POS_X = 0;
    public static int POS_Y = 1;
    public static int LEVEL = 2; //HIDDEN_MONSTER_INDEX (summoningCircle)
    public static int ACTIVE_RESOURCE = 3;
    public static int TIME_LEFT = 4;
    public static int PRODUCING = 5;
    public static int PAUSED = 6;
    public static int NUM_TYPE = 7;
    public static int ACTIVE_RESOURCE_TIME = 8;
    public static int CONSTRUCTION_TYPE = 9;
    public static int IS_PRODUCER = 10;
    public static int IS_CONVERTER = 11;

    #endregion CONSTRUCTION KEYS

    #region SINGLETON PATTERN

    public static Data Instance;

    //crear diccionaris publics de inventari i de crafteos
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (GameManager.Instance != null)
        {
            this.resourcesIcons = GameManager.Instance.resourcesIcons;
            this.monstersIcons = GameManager.Instance.monstersIcons;
            this.dropsIcons = GameManager.Instance.dropsIcons;
            this.villagersIcons = GameManager.Instance.villagersIcons;
        }

        #region RESOURCES

        #region BUILDING RESOURCES

        #region GRAVEYARD

        //Bone
        Requirement[] requirements1;
        requirements1 = new Requirement[0];
        RESOURCES.Add(BONE, new Resource("Bone", BONE, true, 5 / 2, true, requirements1, resourcesIcons[5]));

        //Rotten Flesh
        Requirement[] requirements2;
        Requirement requirement2 = new Requirement();

        requirements2 = new Requirement[1];

        requirement2.resourceNameKey = POISONIVY;
        requirement2.quantity = 4 / 2;
        requirements2[0] = requirement2;

        RESOURCES.Add(ROTTEN_FLESH, new Resource("Rotten flesh", ROTTEN_FLESH, false, 12 / 2, false, requirements2, resourcesIcons[26]));

        //Tombstone
        Requirement[] requirements3;
        Requirement requirement3A = new Requirement();
        Requirement requirement3B = new Requirement();

        requirements3 = new Requirement[2];

        requirement3A.resourceNameKey = SPIDERWEB;
        requirement3A.quantity = 50 / 2;
        requirements3[0] = requirement3A;

        requirement3B.resourceNameKey = GARGOYLESTONE;
        requirement3B.quantity = 8 / 2;
        requirements3[1] = requirement3B;

        RESOURCES.Add(TOMBSTONE, new Resource("Tombstone", TOMBSTONE, false, 60 / 2, false, requirements3, resourcesIcons[33]));

        #endregion GRAVEYARD

        #region FOREST

        //SpiderWeb
        Requirement[] requirements4;
        Requirement requirement4 = new Requirement();

        requirements4 = new Requirement[1];

        requirement4.resourceNameKey = BONE;
        requirement4.quantity = 2;
        requirements4[0] = requirement4;

        RESOURCES.Add(SPIDERWEB, new Resource("Spiderweb", SPIDERWEB, false, 8 / 2, false, requirements4, resourcesIcons[30]));

        //Lantern
        Requirement[] requirements5;
        Requirement requirement5 = new Requirement();

        requirements5 = new Requirement[1];

        requirement5.resourceNameKey = MUD;
        requirement5.quantity = 6 / 2;
        requirements5[0] = requirement2;

        RESOURCES.Add(LANTERN, new Resource("Lantern", LANTERN, false, 15 / 2, false, requirements5, resourcesIcons[19]));

        //Dead tree branch
        Requirement[] requirements6;
        Requirement requirement6A = new Requirement();
        Requirement requirement6B = new Requirement();

        requirements6 = new Requirement[2];

        requirement6A.resourceNameKey = BROOMSTICK;
        requirement6A.quantity = 9 / 2;
        requirements6[0] = requirement3A;

        requirement6B.resourceNameKey = POISONAPPLE;
        requirement6B.quantity = 30 / 2;
        requirements6[1] = requirement3B;

        RESOURCES.Add(DEADTREEBRANCH, new Resource("Dead tree branch", DEADTREEBRANCH, false, 90 / 2, false, requirements6, resourcesIcons[9]));

        #endregion FOREST

        #region VEGETABLE PATCH

        //Poison Ivy
        Requirement[] requirements7;
        requirements7 = new Requirement[0];
        RESOURCES.Add(POISONIVY, new Resource("Poison ivy", POISONIVY, true, 5 / 2, false, requirements7, resourcesIcons[24]));

        //Pumpkin
        Requirement[] requirements8;
        requirements8 = new Requirement[0];
        RESOURCES.Add(PUMPKIN, new Resource("Pumpkin", PUMPKIN, true, 25 / 2, false, requirements8, resourcesIcons[25]));

        //Black Rose
        Requirement[] requirements9;
        requirements9 = new Requirement[0];
        RESOURCES.Add(BLACKROSE, new Resource("Black rose", BLACKROSE, true, 50 / 2, false, requirements9, resourcesIcons[3]));

        #endregion VEGETABLE PATCH

        #region SWAMP

        //Mud
        Requirement[] requirements10;
        requirements10 = new Requirement[0];
        RESOURCES.Add(MUD, new Resource("Mud", MUD, true, 10 / 2, false, requirements10, resourcesIcons[20]));

        //Swamp water
        Requirement[] requirements11;
        requirements11 = new Requirement[0];
        RESOURCES.Add(SWAMPWATER, new Resource("Swamp water", SWAMPWATER, true, 30 / 2, false, requirements11, resourcesIcons[32]));

        //Frog leg
        Requirement[] requirements12;
        requirements12 = new Requirement[0];
        RESOURCES.Add(FROGLEG, new Resource("Frog leg", FROGLEG, true, 90 / 2, false, requirements12, resourcesIcons[14]));

        #endregion SWAMP

        #region WELL

        //Dead fish
        Requirement[] requirements13;
        requirements13 = new Requirement[0];
        RESOURCES.Add(DEADFISH, new Resource("Dead fish", DEADFISH, true, 8 / 2, false, requirements13, resourcesIcons[8]));

        //Bat wing
        Requirement[] requirements14;
        requirements14 = new Requirement[0];
        RESOURCES.Add(BATWING, new Resource("Bat wing", BATWING, true, 20 / 2, false, requirements14, resourcesIcons[1]));

        //Rusty Nail (clavo)
        Requirement[] requirements15;
        requirements15 = new Requirement[0];
        RESOURCES.Add(RUSTYNAIL, new Resource("Rusty nail", RUSTYNAIL, true, 40 / 2, false, requirements15, resourcesIcons[27]));

        #endregion WELL

        #region ABANDONED HOSPITAL

        //Bandages
        Requirement[] requirements16;
        requirements16 = new Requirement[0];
        RESOURCES.Add(BANDAGES, new Resource("Bandages", BANDAGES, true, 10 / 2, false, requirements16, resourcesIcons[0]));

        //Eye
        Requirement[] requirements17;
        Requirement requirement17 = new Requirement();

        requirements17 = new Requirement[1];

        requirement17.resourceNameKey = DEADFISH;
        requirement17.quantity = 10 / 2;
        requirements17[0] = requirement17;

        RESOURCES.Add(EYE, new Resource("Eye", EYE, false, 25 / 2, false, requirements17, resourcesIcons[12]));

        //Blood
        Requirement[] requirements18;
        Requirement requirement18A = new Requirement();
        Requirement requirement18B = new Requirement();

        requirements18 = new Requirement[2];

        requirement18A.resourceNameKey = SPIRITSOUL;
        requirement18A.quantity = 3 / 2;
        requirements18[0] = requirement18A;

        requirement18B.resourceNameKey = RUSTYNAIL;
        requirement18B.quantity = 15 / 2;
        requirements18[1] = requirement18B;

        RESOURCES.Add(BLOOD, new Resource("Blood", BLOOD, false, 180 / 2, false, requirements18, resourcesIcons[4]));

        #endregion ABANDONED HOSPITAL

        #region HAUNTED HOUSE

        //Plasma
        Requirement[] requirements19;
        Requirement requirement19A = new Requirement();
        Requirement requirement19B = new Requirement();

        requirements19 = new Requirement[2];

        requirement19A.resourceNameKey = POISONIVY;
        requirement19A.quantity = 2;
        requirements19[0] = requirement19A;

        requirement19B.resourceNameKey = SPIDERWEB;
        requirement19B.quantity = 6 / 2;
        requirements19[1] = requirement19B;

        RESOURCES.Add(PLASMA, new Resource("Plasma", PLASMA, false, 20 / 2, false, requirements19, resourcesIcons[22]));

        //Ouija Board
        Requirement[] requirements20;
        Requirement requirement20 = new Requirement();

        requirements20 = new Requirement[1];

        requirement20.resourceNameKey = LANTERN;
        requirement20.quantity = 3 / 2;
        requirements20[0] = requirement20;

        RESOURCES.Add(OUIJABOARD, new Resource("Ouija board", OUIJABOARD, false, 20 / 2, false, requirements20, resourcesIcons[21]));

        //Spirit soul
        Requirement[] requirements21;
        Requirement requirement21A = new Requirement();
        Requirement requirement21B = new Requirement();

        requirements21 = new Requirement[2];

        requirement21A.resourceNameKey = FANG;
        requirement21A.quantity = 10 / 2;
        requirements21[0] = requirement21A;

        requirement21B.resourceNameKey = BLACKROSE;
        requirement21B.quantity = 15 / 2;
        requirements21[1] = requirement21B;

        RESOURCES.Add(SPIRITSOUL, new Resource("Spirit soul", SPIRITSOUL, false, 20 / 2, false, requirements21, resourcesIcons[31]));

        #endregion HAUNTED HOUSE

        #region CRYPT

        //Gargoyle stone
        Requirement[] requirements22;
        Requirement requirement22A = new Requirement();
        Requirement requirement22B = new Requirement();

        requirements22 = new Requirement[2];

        requirement22A.resourceNameKey = BANDAGES;
        requirement22A.quantity = 5 / 2;
        requirements22[0] = requirement22A;

        requirement22B.resourceNameKey = MUD;
        requirement22B.quantity = 20 / 2;
        requirements22[1] = requirement22B;

        RESOURCES.Add(GARGOYLESTONE, new Resource("Gargoyle stone", GARGOYLESTONE, false, 20 / 2, false, requirements22, resourcesIcons[15]));

        //Death essence
        Requirement[] requirements23;
        Requirement requirement23A = new Requirement();
        Requirement requirement23B = new Requirement();

        requirements23 = new Requirement[2];

        requirement23A.resourceNameKey = BLACKCATHAIR;
        requirement23A.quantity = 40 / 2;
        requirements23[0] = requirement23A;

        requirement23B.resourceNameKey = WOLFCLAW;
        requirement23B.quantity = 10 / 2;
        requirements23[1] = requirement23B;

        RESOURCES.Add(DEATHESSENCE, new Resource("Death essence", DEATHESSENCE, false, 20 / 2, false, requirements23, resourcesIcons[10]));

        //Coffin
        Requirement[] requirements24;
        Requirement requirement24A = new Requirement();
        Requirement requirement24B = new Requirement();

        requirements24 = new Requirement[2];

        requirement24A.resourceNameKey = RUSTYNAIL;
        requirement24A.quantity = 15 / 2;
        requirements24[0] = requirement24A;

        requirement24B.resourceNameKey = DEADTREEBRANCH;
        requirement24B.quantity = 10 / 2;
        requirements24[1] = requirement24B;

        RESOURCES.Add(COFFIN, new Resource("Coffin", COFFIN, false, 20 / 2, false, requirements24, resourcesIcons[7]));

        #endregion CRYPT

        #region WITCH COVEN

        //Black cat hair
        Requirement[] requirements25;
        Requirement requirement25A = new Requirement();
        Requirement requirement25B = new Requirement();

        requirements25 = new Requirement[2];

        requirement25A.resourceNameKey = POISONIVY;
        requirement25A.quantity = 20 / 2;
        requirements25[0] = requirement25A;

        requirement25B.resourceNameKey = OUIJABOARD;
        requirement25B.quantity = 2;
        requirements25[1] = requirement25B;

        RESOURCES.Add(BLACKCATHAIR, new Resource("Black cat hair", BLACKCATHAIR, false, 20 / 2, false, requirements25, resourcesIcons[2]));

        //Broomstick
        Requirement[] requirements26;
        Requirement requirement26A = new Requirement();
        Requirement requirement26B = new Requirement();

        requirements26 = new Requirement[2];

        requirement26A.resourceNameKey = PUMPKIN;
        requirement26A.quantity = 15 / 2;
        requirements26[0] = requirement26A;

        requirement26B.resourceNameKey = IMMORTALITYELIXIR;
        requirement26B.quantity = 5 / 2;
        requirements26[1] = requirement26B;

        RESOURCES.Add(BROOMSTICK, new Resource("Broomstick", BROOMSTICK, false, 40 / 2, false, requirements26, resourcesIcons[6]));

        //Witch hat
        Requirement[] requirements27;
        Requirement requirement27A = new Requirement();
        Requirement requirement27B = new Requirement();

        requirements27 = new Requirement[2];

        requirement27A.resourceNameKey = DEATHESSENCE;
        requirement27A.quantity = 8 / 2;
        requirements27[0] = requirement27A;

        requirement27B.resourceNameKey = HORNS;
        requirement27B.quantity = 6 / 2;
        requirements27[1] = requirement27B;

        RESOURCES.Add(WITCHHAT, new Resource("Witch hat", WITCHHAT, false, 240 / 2, false, requirements27, resourcesIcons[34]));

        #endregion WITCH COVEN

        #region HELL ISLAND

        //Skull
        Requirement[] requirements28;
        Requirement requirement28A = new Requirement();
        Requirement requirement28B = new Requirement();

        requirements28 = new Requirement[2];

        requirement28A.resourceNameKey = EYE;
        requirement28A.quantity = 10 / 2;
        requirements28[0] = requirement28A;

        requirement28B.resourceNameKey = DIVINATIONBALL;
        requirement28B.quantity = 5 / 2;
        requirements28[1] = requirement28B;

        RESOURCES.Add(SKULL, new Resource("Skull", SKULL, false, 30 / 2, false, requirements28, resourcesIcons[28]));

        //Horns
        Requirement[] requirements29;
        Requirement requirement29A = new Requirement();
        Requirement requirement29B = new Requirement();

        requirements29 = new Requirement[2];

        requirement29A.resourceNameKey = BATWING;
        requirement29A.quantity = 9 / 2;
        requirements29[0] = requirement29A;

        requirement29B.resourceNameKey = SPELLBOOK;
        requirement29B.quantity = 3 / 2;
        requirements29[1] = requirement29B;

        RESOURCES.Add(HORNS, new Resource("Horns", HORNS, false, 60 / 2, false, requirements29, resourcesIcons[17]));

        //Hell fire
        Requirement[] requirements30;
        Requirement requirement30A = new Requirement();
        Requirement requirement30B = new Requirement();

        requirements30 = new Requirement[2];

        requirement30A.resourceNameKey = SKULL;
        requirement30A.quantity = 10 / 2;
        requirements30[0] = requirement30A;

        requirement30B.resourceNameKey = ROTTEN_FLESH;
        requirement30B.quantity = 80 / 2;
        requirements30[1] = requirement30B;

        RESOURCES.Add(HELLFIRE, new Resource("Hell fire", HELLFIRE, false, 270 / 2, false, requirements30, resourcesIcons[16]));

        #endregion HELL ISLAND

        #region MAGIC WORKSHOP

        //Divination ball
        Requirement[] requirements31;
        Requirement requirement31 = new Requirement();

        requirements31 = new Requirement[1];

        requirement31.resourceNameKey = SWAMPWATER;
        requirement31.quantity = 3 / 2;
        requirements31[0] = requirement31;

        RESOURCES.Add(DIVINATIONBALL, new Resource("Divination ball", DIVINATIONBALL, false, 20 / 2, false, requirements31, resourcesIcons[11]));

        //Immortality elixir
        Requirement[] requirements32;
        Requirement requirement32 = new Requirement();

        requirements32 = new Requirement[1];

        requirement32.resourceNameKey = DEADFISH;
        requirement32.quantity = 10 / 2;
        requirements32[0] = requirement32;

        RESOURCES.Add(IMMORTALITYELIXIR, new Resource("Immortality elixir", IMMORTALITYELIXIR, false, 20 / 2, false, requirements32, resourcesIcons[18]));

        //Spellbook
        Requirement[] requirements33;
        Requirement requirement33A = new Requirement();
        Requirement requirement33B = new Requirement();

        requirements33 = new Requirement[2];

        requirement33A.resourceNameKey = FROGLEG;
        requirement33A.quantity = 3 / 2;
        requirements33[0] = requirement33A;

        requirement33B.resourceNameKey = TOMBSTONE;
        requirement33B.quantity = 9 / 2;
        requirements33[1] = requirement33B;

        RESOURCES.Add(SPELLBOOK, new Resource("Spellbook", SPELLBOOK, false, 20 / 2, false, requirements33, resourcesIcons[29]));

        #endregion MAGIC WORKSHOP

        #region DEEP FOREST

        //Poison apple
        Requirement[] requirements34;
        Requirement requirement34 = new Requirement();

        requirements34 = new Requirement[1];

        requirement34.resourceNameKey = BONE;
        requirement34.quantity = 10 / 2;
        requirements34[0] = requirement34;

        RESOURCES.Add(POISONAPPLE, new Resource("Poison apple", POISONAPPLE, false, 3 / 2, false, requirements34, resourcesIcons[23]));

        //Fang
        Requirement[] requirements35;
        Requirement requirement35 = new Requirement();

        requirements35 = new Requirement[1];

        requirement35.resourceNameKey = SKULL;
        requirement35.quantity = 8 / 2;
        requirements35[0] = requirement35;

        RESOURCES.Add(FANG, new Resource("Fang", FANG, false, 12 / 2, false, requirements35, resourcesIcons[13]));

        //Wolf's claw
        Requirement[] requirements36;
        Requirement requirement36 = new Requirement();

        requirements36 = new Requirement[1];

        requirement36.resourceNameKey = PLASMA;
        requirement36.quantity = 20 / 2;
        requirements36[0] = requirement36;

        RESOURCES.Add(WOLFCLAW, new Resource("Wolf's claw", WOLFCLAW, false, 30 / 2, false, requirements36, resourcesIcons[35]));

        #endregion DEEP FOREST

        #endregion BUILDING RESOURCES

        if (GameManager.Instance != null)
        {
            #region DROPS

            RESOURCES.Add(LOLLIPOP, new DropInfo(LOLLIPOP, "Lollipop", "child", dropsIcons[0], villagersIcons[0]));
            RESOURCES.Add(RING, new DropInfo(RING, "Ring", "mom", dropsIcons[1], villagersIcons[1]));
            RESOURCES.Add(BEER, new DropInfo(BEER, "Beer", "adult", dropsIcons[2], villagersIcons[2]));
            RESOURCES.Add(SWORD, new DropInfo(SWORD, "Sword", "swashbuckler", dropsIcons[3], villagersIcons[3]));
            RESOURCES.Add(SHIELD, new DropInfo(SHIELD, "Shield", "shield man", dropsIcons[4], villagersIcons[4]));
            RESOURCES.Add(STICK, new DropInfo(STICK, "Stick", "elder", dropsIcons[5], villagersIcons[5]));
            RESOURCES.Add(GEM, new DropInfo(GEM, "Gem", "sorcerer", dropsIcons[6], villagersIcons[6]));
            RESOURCES.Add(SCARE, new DropInfo(SCARE, "Scare", "villagers", dropsIcons[7], null));

            #endregion DROPS
        }

        #endregion RESOURCES

        #region PLAYER

        if (GameManager.Instance != null)
        {
            PLAYER.Add("Hour", GameManager.Instance.localDate.Hour);
            PLAYER.Add("Minute", GameManager.Instance.localDate.Minute);
            PLAYER.Add("Second", GameManager.Instance.localDate.Second);
            PLAYER.Add("Day", GameManager.Instance.localDate.Day);
            PLAYER.Add("Month", GameManager.Instance.localDate.Month);
            PLAYER.Add("Year", GameManager.Instance.localDate.Year);
            PLAYER.Add("Tuto", GameManager.Instance.isTutoDone);
            PLAYER.Add("isRestart", GameManager.Instance.isTutoDone);
        }
        else
        {
            PLAYER.Add("Hour", OfflineCalculator.Instance.localDate.Hour);
            PLAYER.Add("Minute", OfflineCalculator.Instance.localDate.Minute);
            PLAYER.Add("Second", OfflineCalculator.Instance.localDate.Second);
            PLAYER.Add("Day", OfflineCalculator.Instance.localDate.Day);
            PLAYER.Add("Month", OfflineCalculator.Instance.localDate.Month);
            PLAYER.Add("Year", OfflineCalculator.Instance.localDate.Year);
            PLAYER.Add("Tuto", 0);
        }

        #endregion PLAYER

        #region MONSTERS

        #region SKELETON

        //Skeleton
        List<Requirement> reqSkeleton = new List<Requirement>();

        reqSkeleton.Add(new Requirement(BONE, 400));
        reqSkeleton.Add(new Requirement(PLASMA, 40));

        List<List<Requirement>> reqUpgradeSkeleton = new List<List<Requirement>>();

        List<Requirement> reqUpgradeSkeleton1 = new List<Requirement>();
        reqUpgradeSkeleton1.Add(new Requirement(BANDAGES, 20));
        reqUpgradeSkeleton1.Add(new Requirement(POISONIVY, 150));

        List<Requirement> reqUpgradeSkeleton2 = new List<Requirement>();
        reqUpgradeSkeleton2.Add(new Requirement(BLACKCATHAIR, 150));
        reqUpgradeSkeleton2.Add(new Requirement(PLASMA, 150));

        reqUpgradeSkeleton.Add(reqUpgradeSkeleton1);
        reqUpgradeSkeleton.Add(reqUpgradeSkeleton2);

        List<Requirement> reqUnlockSkeleton = new List<Requirement>();

        reqUnlockSkeleton.Add(new Requirement(PLASMA, 30));

        #region STATS

        List<float> velSkeleton = new List<float>();
        velSkeleton.Add(3);
        velSkeleton.Add(3);
        velSkeleton.Add(3.5f);

        List<int> healthSkeleton = new List<int>();
        healthSkeleton.Add(10);
        healthSkeleton.Add(10);
        healthSkeleton.Add(15);

        List<int> damageSkeleton = new List<int>();
        damageSkeleton.Add(1);
        damageSkeleton.Add(2);
        damageSkeleton.Add(2);

        List<float> aRateSkeleton = new List<float>();
        aRateSkeleton.Add(2);
        aRateSkeleton.Add(2);
        aRateSkeleton.Add(2);

        List<float> aRangeSkeleton = new List<float>();
        aRangeSkeleton.Add(5);
        aRangeSkeleton.Add(5);
        aRangeSkeleton.Add(5);

        List<int> levelSkeleton = new List<int>();
        levelSkeleton.Add(1);
        levelSkeleton.Add(1);
        levelSkeleton.Add(1);

        #endregion STATS

        string desSkeleton = "It's the most basic type of monster. Can only be invoked outside the village.";
        MONSTERS.Add(SKELETON, new MonsterInfo(SKELETON, "Skeleton", 3600 / 4, reqSkeleton, reqUpgradeSkeleton, reqUnlockSkeleton, velSkeleton, healthSkeleton, damageSkeleton, aRateSkeleton, aRangeSkeleton, levelSkeleton, monstersIcons[0], desSkeleton, 1, true));

        #endregion SKELETON

        #region JACK O LANTERN

        //JackOLantern
        List<Requirement> reqJack = new List<Requirement>();

        reqJack.Add(new Requirement(PUMPKIN, 200));
        reqJack.Add(new Requirement(LANTERN, 80));

        List<List<Requirement>> reqUpgradeJack = new List<List<Requirement>>();
        List<Requirement> reqUpgradeJack1 = new List<Requirement>();
        List<Requirement> reqUpgradeJack2 = new List<Requirement>();

        reqUpgradeJack1.Add(new Requirement(MUD, 100));
        reqUpgradeJack1.Add(new Requirement(GARGOYLESTONE, 50));

        reqUpgradeJack2.Add(new Requirement(BLACKCATHAIR, 300));
        reqUpgradeJack2.Add(new Requirement(HORNS, 200));

        reqUpgradeJack.Add(reqUpgradeJack1);
        reqUpgradeJack.Add(reqUpgradeJack2);

        List<Requirement> reqUnlockJack = new List<Requirement>();

        reqUnlockJack.Add(new Requirement(LANTERN, 30));

        #region STATS

        List<float> velJack = new List<float>();
        velJack.Add(0);
        velJack.Add(0);
        velJack.Add(0);

        List<int> healthJack = new List<int>();
        healthJack.Add(20);
        healthJack.Add(25);
        healthJack.Add(35);

        List<int> damageJack = new List<int>();
        damageJack.Add(0);
        damageJack.Add(0);
        damageJack.Add(0);

        List<float> aRateJack = new List<float>();
        aRateJack.Add(0);
        aRateJack.Add(0);
        aRateJack.Add(0);

        List<float> aRangeJack = new List<float>();
        aRangeJack.Add(0);
        aRangeJack.Add(0);
        aRangeJack.Add(0);

        List<int> levelJack = new List<int>();
        levelJack.Add(1);
        levelJack.Add(1);
        levelJack.Add(1);

        #endregion STATS

        string desJack = "Is used as an obstacle to block the path of the villagers. It can be invoked inside the village but doesn't move.";

        MONSTERS.Add(JACK_LANTERN, new MonsterInfo(JACK_LANTERN, "Jack-o'-lantern", 5400 / 4, reqJack, reqUpgradeJack, reqUnlockJack, velJack, healthJack, damageJack, aRateJack, aRangeJack, levelJack, monstersIcons[1], desJack, 1, false));

        #endregion JACK O LANTERN

        #region BAT

        //Bat
        List<Requirement> reqBat = new List<Requirement>();

        reqBat.Add(new Requirement(OUIJABOARD, 40));
        reqBat.Add(new Requirement(GARGOYLESTONE, 40));

        List<List<Requirement>> reqUpgradeBat = new List<List<Requirement>>();
        List<Requirement> reqUpgradeBat1 = new List<Requirement>();
        List<Requirement> reqUpgradeBat2 = new List<Requirement>();

        reqUpgradeBat1.Add(new Requirement(SPIDERWEB, 500));
        reqUpgradeBat1.Add(new Requirement(BANDAGES, 400));

        reqUpgradeBat2.Add(new Requirement(TOMBSTONE, 150));
        reqUpgradeBat2.Add(new Requirement(OUIJABOARD, 250));

        reqUpgradeBat.Add(reqUpgradeBat1);
        reqUpgradeBat.Add(reqUpgradeBat2);

        List<Requirement> reqUnlockBat = new List<Requirement>();

        reqUnlockBat.Add(new Requirement(BATWING, 30));

        #region STATS

        List<float> velBat = new List<float>();
        velBat.Add(2.5f);
        velBat.Add(3);
        velBat.Add(3);

        List<int> healthBat = new List<int>();
        healthBat.Add(7);
        healthBat.Add(13);
        healthBat.Add(16);

        List<int> damageBat = new List<int>();
        damageBat.Add(2);
        damageBat.Add(2);
        damageBat.Add(3);

        List<float> aRateBat = new List<float>();
        aRateBat.Add(1.5f);
        aRateBat.Add(1.5f);
        aRateBat.Add(1.5f);

        List<float> aRangeBat = new List<float>();
        aRangeBat.Add(5);
        aRangeBat.Add(5);
        aRangeBat.Add(5);

        List<int> levelBat = new List<int>();
        levelBat.Add(2);
        levelBat.Add(2);
        levelBat.Add(2);

        #endregion STATS

        string desBat = "Flies and passes through terrain obstacles and Shield Man. Can only be invoked outside village.";

        MONSTERS.Add(BAT, new MonsterInfo(BAT, "Bat", 7200 / 4, reqBat, reqUpgradeBat, reqUnlockBat, velBat, healthBat, damageBat, aRateBat, aRangeBat, levelBat, monstersIcons[2], desBat, 1, false));

        #endregion BAT

        #region GOBLIN

        //Goblin
        List<Requirement> reqGoblin = new List<Requirement>();

        reqGoblin.Add(new Requirement(DEADFISH, 400));
        reqGoblin.Add(new Requirement(BLACKCATHAIR, 50));

        List<List<Requirement>> reqUpgradeGoblin = new List<List<Requirement>>();
        List<Requirement> reqUpgradeGoblin1 = new List<Requirement>();
        List<Requirement> reqUpgradeGoblin2 = new List<Requirement>();

        reqUpgradeGoblin1.Add(new Requirement(EYE, 30));
        reqUpgradeGoblin1.Add(new Requirement(DEADFISH, 500));

        reqUpgradeGoblin2.Add(new Requirement(BLACKROSE, 300));
        reqUpgradeGoblin2.Add(new Requirement(RUSTYNAIL, 500));

        reqUpgradeGoblin.Add(reqUpgradeGoblin1);
        reqUpgradeGoblin.Add(reqUpgradeGoblin2);

        List<Requirement> reqUnlockGoblin = new List<Requirement>();

        reqUnlockGoblin.Add(new Requirement(SWAMPWATER, 30));

        #region STATS

        List<float> velGoblin = new List<float>();
        velGoblin.Add(5);
        velGoblin.Add(5.5f);
        velGoblin.Add(6);

        List<int> healthGoblin = new List<int>();
        healthGoblin.Add(12);
        healthGoblin.Add(16);
        healthGoblin.Add(20);

        List<int> damageGoblin = new List<int>();
        damageGoblin.Add(1);
        damageGoblin.Add(1);
        damageGoblin.Add(1);

        List<float> aRateGoblin = new List<float>();
        aRateGoblin.Add(0.5f);
        aRateGoblin.Add(0.5f);
        aRateGoblin.Add(0.25f);

        List<float> aRangeGoblin = new List<float>();
        aRangeGoblin.Add(6);
        aRangeGoblin.Add(6);
        aRangeGoblin.Add(6);

        List<int> levelGoblin = new List<int>();
        levelGoblin.Add(2);
        levelGoblin.Add(2);
        levelGoblin.Add(2);

        #endregion STATS

        string desGoblin = "The quickest and sneakiest monster. Can only be invoked outside village.";

        MONSTERS.Add(GOBLIN, new MonsterInfo(GOBLIN, "Goblin", 9000 / 4, reqGoblin, reqUpgradeGoblin, reqUnlockGoblin, velGoblin, healthGoblin, damageGoblin, aRateGoblin, aRangeGoblin, levelGoblin, monstersIcons[3], desGoblin, 1, false));

        #endregion GOBLIN

        #region GHOST

        //Ghost
        List<Requirement> reqGhost = new List<Requirement>();

        reqGhost.Add(new Requirement(TOMBSTONE, 40));
        reqGhost.Add(new Requirement(DIVINATIONBALL, 150));

        List<List<Requirement>> reqUpgradeGhost = new List<List<Requirement>>();
        List<Requirement> reqUpgradeGhost1 = new List<Requirement>();
        List<Requirement> reqUpgradeGhost2 = new List<Requirement>();

        reqUpgradeGhost1.Add(new Requirement(SPELLBOOK, 75));
        reqUpgradeGhost1.Add(new Requirement(PUMPKIN, 100));

        reqUpgradeGhost2.Add(new Requirement(DEADTREEBRANCH, 200));
        reqUpgradeGhost2.Add(new Requirement(HORNS, 100));

        reqUpgradeGhost.Add(reqUpgradeGhost1);
        reqUpgradeGhost.Add(reqUpgradeGhost2);

        List<Requirement> reqUnlockGhost = new List<Requirement>();

        reqUnlockGhost.Add(new Requirement(SPIRITSOUL, 30));

        #region STATS

        List<float> velGhost = new List<float>();
        velGhost.Add(3.5f);
        velGhost.Add(3.5f);
        velGhost.Add(3.5f);

        List<int> healthGhost = new List<int>();
        healthGhost.Add(20);
        healthGhost.Add(25);
        healthGhost.Add(30);

        List<int> damageGhost = new List<int>();
        damageGhost.Add(4);
        damageGhost.Add(4);
        damageGhost.Add(5);

        List<float> aRateGhost = new List<float>();
        aRateGhost.Add(1);
        aRateGhost.Add(1);
        aRateGhost.Add(1);

        List<float> aRangeGhost = new List<float>();
        aRangeGhost.Add(5);
        aRangeGhost.Add(5);
        aRangeGhost.Add(5);

        List<int> levelGhost = new List<int>();
        levelGhost.Add(3);
        levelGhost.Add(3);
        levelGhost.Add(3);

        #endregion STATS

        string desGhost = "Can be invoked from a dead monster's tomb inside the village. It is ethereal and passes through obstacles and Shield Man.";

        MONSTERS.Add(GHOST, new MonsterInfo(GHOST, "Ghost", 10800 / 4, reqGhost, reqUpgradeGhost, reqUnlockGhost, velGhost, healthGhost, damageGhost, aRateGhost, aRangeGhost, levelGhost, monstersIcons[4], desGhost, 1, false));

        #endregion GHOST

        #region CLOWN

        //Clown
        List<Requirement> reqClown = new List<Requirement>();

        reqClown.Add(new Requirement(EYE, 200));
        reqClown.Add(new Requirement(SKULL, 40));

        List<List<Requirement>> reqUpgradeClown = new List<List<Requirement>>();
        List<Requirement> reqUpgradeClown1 = new List<Requirement>();
        List<Requirement> reqUpgradeClown2 = new List<Requirement>();

        reqUpgradeClown1.Add(new Requirement(HORNS, 150));
        reqUpgradeClown1.Add(new Requirement(SPIRITSOUL, 300));

        reqUpgradeClown2.Add(new Requirement(WITCHHAT, 25));
        reqUpgradeClown2.Add(new Requirement(RUSTYNAIL, 500));

        reqUpgradeClown.Add(reqUpgradeClown1);
        reqUpgradeClown.Add(reqUpgradeClown2);

        List<Requirement> reqUnlockClown = new List<Requirement>();

        reqUnlockClown.Add(new Requirement(HORNS, 30));

        #region STATS

        List<float> velClown = new List<float>();
        velClown.Add(0);
        velClown.Add(0);
        velClown.Add(0);

        List<int> healthClown = new List<int>();
        healthClown.Add(5);
        healthClown.Add(7);
        healthClown.Add(10);

        List<int> damageClown = new List<int>();
        damageClown.Add(0);
        damageClown.Add(0);
        damageClown.Add(0);

        List<float> aRateClown = new List<float>();
        aRateClown.Add(0);
        aRateClown.Add(0);
        aRateClown.Add(0);

        List<float> aRangeClown = new List<float>();
        aRangeClown.Add(6);
        aRangeClown.Add(6.5f);
        aRangeClown.Add(7);

        List<int> levelClown = new List<int>();
        levelClown.Add(3);
        levelClown.Add(3);
        levelClown.Add(3);

        #endregion STATS

        string desClown = "A truly show man, it distracts villagers in range, paralizing them. Can be invoked inside village and each second loses 1HP";

        MONSTERS.Add(CLOWN, new MonsterInfo(CLOWN, "Clown", 12600 / 4, reqClown, reqUpgradeClown, reqUnlockClown, velClown, healthClown, damageClown, aRateClown, aRangeClown, levelClown, monstersIcons[5], desClown, 1, false));

        #endregion CLOWN

        #region ZOMBIE

        //Zombie
        List<Requirement> reqZombie = new List<Requirement>();

        reqZombie.Add(new Requirement(SWAMPWATER, 400));
        reqZombie.Add(new Requirement(POISONAPPLE, 200));

        List<List<Requirement>> reqUpgradeZombie = new List<List<Requirement>>();
        List<Requirement> reqUpgradeZombie1 = new List<Requirement>();
        List<Requirement> reqUpgradeZombie2 = new List<Requirement>();

        reqUpgradeZombie1.Add(new Requirement(POISONAPPLE, 200));
        reqUpgradeZombie1.Add(new Requirement(BATWING, 50));

        reqUpgradeZombie2.Add(new Requirement(DIVINATIONBALL, 500));
        reqUpgradeZombie2.Add(new Requirement(DEADFISH, 30));

        reqUpgradeZombie.Add(reqUpgradeZombie1);
        reqUpgradeZombie.Add(reqUpgradeZombie2);

        List<Requirement> reqUnlockZombie = new List<Requirement>();

        reqUnlockZombie.Add(new Requirement(DEATHESSENCE, 30));

        #region STATS

        List<float> velZombie = new List<float>();
        velZombie.Add(4);
        velZombie.Add(4.5f);
        velZombie.Add(4.5f);

        List<int> healthZombie = new List<int>();
        healthZombie.Add(35);
        healthZombie.Add(42);
        healthZombie.Add(50);

        List<int> damageZombie = new List<int>();
        damageZombie.Add(5);
        damageZombie.Add(6);
        damageZombie.Add(8);

        List<float> aRateZombie = new List<float>();
        aRateZombie.Add(3);
        aRateZombie.Add(3);
        aRateZombie.Add(3);

        List<float> aRangeZombie = new List<float>();
        aRangeZombie.Add(3);
        aRangeZombie.Add(3);
        aRangeZombie.Add(3);

        List<int> levelZombie = new List<int>();
        levelZombie.Add(4);
        levelZombie.Add(4);
        levelZombie.Add(4);

        #endregion STATS

        string desZombie = "The slowest monster but the one that deals more damage. It can be invoked from sewers inside the village.";

        MONSTERS.Add(ZOMBIE, new MonsterInfo(ZOMBIE, "Zombie", 14400 / 4, reqZombie, reqUpgradeZombie, reqUnlockZombie, velZombie, healthZombie, damageZombie, aRateZombie, aRangeZombie, levelZombie, monstersIcons[6], desZombie, 1, false));

        #endregion ZOMBIE

        #region VAMPIRE

        //Vampire
        List<Requirement> reqVampire = new List<Requirement>();

        reqVampire.Add(new Requirement(BLOOD, 20));
        reqVampire.Add(new Requirement(COFFIN, 5));

        List<List<Requirement>> reqUpgradeVampire = new List<List<Requirement>>();
        List<Requirement> reqUpgradeVampire1 = new List<Requirement>();
        List<Requirement> reqUpgradeVampire2 = new List<Requirement>();

        reqUpgradeVampire1.Add(new Requirement(FROGLEG, 500));
        reqUpgradeVampire1.Add(new Requirement(TOMBSTONE, 50));

        reqUpgradeVampire2.Add(new Requirement(HELLFIRE, 60));
        reqUpgradeVampire2.Add(new Requirement(GARGOYLESTONE, 50));

        reqUpgradeVampire.Add(reqUpgradeVampire1);
        reqUpgradeVampire.Add(reqUpgradeVampire2);

        List<Requirement> reqUnlockVampire = new List<Requirement>();

        reqUnlockVampire.Add(new Requirement(COFFIN, 30));

        #region STATS

        List<float> velVampire = new List<float>();
        velVampire.Add(4.5f);
        velVampire.Add(4.5f);
        velVampire.Add(4.5f);

        List<int> healthVampire = new List<int>();
        healthVampire.Add(22);
        healthVampire.Add(26);
        healthVampire.Add(30);

        List<int> damageVampire = new List<int>();
        damageVampire.Add(4);
        damageVampire.Add(5);
        damageVampire.Add(6);

        List<float> aRateVampire = new List<float>();
        aRateVampire.Add(1);
        aRateVampire.Add(1);
        aRateVampire.Add(1);

        List<float> aRangeVampire = new List<float>();
        aRangeVampire.Add(5);
        aRangeVampire.Add(5);
        aRangeVampire.Add(5);

        List<int> levelVampire = new List<int>();
        levelVampire.Add(4);
        levelVampire.Add(4);
        levelVampire.Add(4);

        #endregion STATS

        string desVampire = "Heals himself over time. It also transforms into a bat and avoids obstacles and Shield Man but can only be invoked outside village.";

        MONSTERS.Add(VAMPIRE, new MonsterInfo(VAMPIRE, "Vampire", 16200 / 4, reqVampire, reqUpgradeVampire, reqUnlockVampire, velVampire, healthVampire, damageVampire, aRateVampire, aRangeVampire, levelVampire, monstersIcons[7], desVampire, 1, false));

        #endregion VAMPIRE

        #region WITCH

        //Witch
        List<Requirement> reqWitch = new List<Requirement>();

        reqWitch.Add(new Requirement(WITCHHAT, 20));
        reqWitch.Add(new Requirement(HELLFIRE, 20));

        List<List<Requirement>> reqUpgradeWitch = new List<List<Requirement>>();
        List<Requirement> reqUpgradeWitch1 = new List<Requirement>();
        List<Requirement> reqUpgradeWitch2 = new List<Requirement>();

        reqUpgradeWitch1.Add(new Requirement(WOLFCLAW, 200));
        reqUpgradeWitch1.Add(new Requirement(IMMORTALITYELIXIR, 20));

        reqUpgradeWitch2.Add(new Requirement(COFFIN, 30));
        reqUpgradeWitch2.Add(new Requirement(BROOMSTICK, 50));

        reqUpgradeWitch.Add(reqUpgradeWitch1);
        reqUpgradeWitch.Add(reqUpgradeWitch2);

        List<Requirement> reqUnlockWitch = new List<Requirement>();

        reqUnlockWitch.Add(new Requirement(WITCHHAT, 30));

        #region STATS

        List<float> velWitch = new List<float>();
        velWitch.Add(2.5f);
        velWitch.Add(2.5f);
        velWitch.Add(3);

        List<int> healthWitch = new List<int>();
        healthWitch.Add(26);
        healthWitch.Add(32);
        healthWitch.Add(36);

        List<int> damageWitch = new List<int>();
        damageWitch.Add(4);
        damageWitch.Add(5);
        damageWitch.Add(7);

        List<float> aRateWitch = new List<float>();
        aRateWitch.Add(1);
        aRateWitch.Add(1);
        aRateWitch.Add(1);

        List<float> aRangeWitch = new List<float>();
        aRangeWitch.Add(6);
        aRangeWitch.Add(6.5f);
        aRangeWitch.Add(7);

        List<int> levelWitch = new List<int>();
        levelWitch.Add(5);
        levelWitch.Add(5);
        levelWitch.Add(5);

        #endregion STATS

        string desWitch = "Invokes skeletons over time and makes ranged attacks. It has great resistance.";

        MONSTERS.Add(WITCH, new MonsterInfo(WITCH, "Witch", 18000 / 4, reqWitch, reqUpgradeWitch, reqUnlockWitch, velWitch, healthWitch, damageWitch, aRateWitch, aRangeWitch, levelWitch, monstersIcons[8], desWitch, 1, false));

        #endregion WITCH

        #region REAPER

        //Reaper
        List<Requirement> reqReaper = new List<Requirement>();

        reqReaper.Add(new Requirement(GEM, 100)); //S'haurà de modificar UI perque accepti tants requirements
        reqReaper.Add(new Requirement(RING, 85));
        /*reqReaper.Add(new Requirement(BEER, 70));
        reqReaper.Add(new Requirement(SWORD, 55));
        reqReaper.Add(new Requirement(STICK, 40));
        reqReaper.Add(new Requirement(SHIELD, 25));
        reqReaper.Add(new Requirement(GEM, 15));*/

        List<List<Requirement>> reqUpgradeReaper = new List<List<Requirement>>(); //No es podrà upgradejar
        List<Requirement> reqUpgradeReaper1 = new List<Requirement>();
        List<Requirement> reqUpgradeReaper2 = new List<Requirement>();

        reqUpgradeReaper1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeReaper1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeReaper2.Add(new Requirement(ROTTEN_FLESH, 2500));
        reqUpgradeReaper2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeReaper.Add(reqUpgradeReaper1);
        reqUpgradeReaper.Add(reqUpgradeReaper2);

        List<Requirement> reqUnlockReaper = new List<Requirement>();

        reqUnlockReaper.Add(new Requirement(HELLFIRE, 30)); //Or scares

        #region STATS

        List<float> velReaper = new List<float>();
        velReaper.Add(12);
        velReaper.Add(12);
        velReaper.Add(12);

        List<int> healthReaper = new List<int>();
        healthReaper.Add(6666666);
        healthReaper.Add(6666666);
        healthReaper.Add(6666666);

        List<int> damageReaper = new List<int>();
        damageReaper.Add(6666666);
        damageReaper.Add(6666666);
        damageReaper.Add(6666666);

        List<float> aRateReaper = new List<float>();
        aRateReaper.Add(0.5f);
        aRateReaper.Add(0.5f);
        aRateReaper.Add(0.5f);

        List<float> aRangeReaper = new List<float>();
        aRangeReaper.Add(10);
        aRangeReaper.Add(10);
        aRangeReaper.Add(10);

        List<int> levelReaper = new List<int>();
        levelReaper.Add(6);
        levelReaper.Add(6);
        levelReaper.Add(6);

        #endregion STATS

        string desReaper = "If you want things done properly, do them yourself. The ultimate weapon to bring back Halloween's true spirit.";

        MONSTERS.Add(REAPER, new MonsterInfo(REAPER, "The Reaper", 28800 / 2, reqReaper, reqUpgradeReaper, reqUnlockReaper, velReaper, healthReaper, damageReaper, aRateReaper, aRangeReaper, levelReaper, monstersIcons[9], desReaper, 1, false));

        #endregion REAPER

        #endregion MONSTERS

        if (GameManager.Instance != null)
        {
            #region DROPS

            DROPS.Add(LOLLIPOP, new DropInfo(LOLLIPOP, "Lollipop", "child", dropsIcons[0], villagersIcons[0]));
            DROPS.Add(RING, new DropInfo(RING, "Ring", "mom", dropsIcons[1], villagersIcons[1]));
            DROPS.Add(BEER, new DropInfo(BEER, "Beer", "adult", dropsIcons[2], villagersIcons[2]));
            DROPS.Add(SWORD, new DropInfo(SWORD, "Sword", "swashbuckler", dropsIcons[3], villagersIcons[3]));
            DROPS.Add(SHIELD, new DropInfo(SHIELD, "Shield", "shield man", dropsIcons[4], villagersIcons[4]));
            DROPS.Add(STICK, new DropInfo(STICK, "Stick", "elder", dropsIcons[5], villagersIcons[5]));
            DROPS.Add(GEM, new DropInfo(GEM, "Gem", "sorcerer", dropsIcons[6], villagersIcons[6]));
            DROPS.Add(SCARE, new DropInfo(SCARE, "Scare", "villagers", dropsIcons[7], null));

            #endregion DROPS

            #region MONSTERS STATS

            MONSTERS_STATS.Add(SKELETON, new int[] { 0, 1 });
            MONSTERS_STATS.Add(JACK_LANTERN, new int[] { 0, 1 });
            MONSTERS_STATS.Add(BAT, new int[] { 0, 1 });
            MONSTERS_STATS.Add(GOBLIN, new int[] { 0, 1 });
            MONSTERS_STATS.Add(GHOST, new int[] { 0, 1 });
            MONSTERS_STATS.Add(CLOWN, new int[] { 0, 1 });
            MONSTERS_STATS.Add(ZOMBIE, new int[] { 0, 1 });
            MONSTERS_STATS.Add(VAMPIRE, new int[] { 0, 1 });
            MONSTERS_STATS.Add(WITCH, new int[] { 0, 1 });
            MONSTERS_STATS.Add(REAPER, new int[] { 0, 1 });

            #endregion MONSTERS STATS
        }
    }

    #endregion SINGLETON PATTERN

    //Al start de la shopManager es passa el array de buldings (prefabs) omplert manualment cap aqui i es guarda en el diccionari
    public void setBuildings(List<GameObject> buildings)
    {
        foreach (GameObject building in buildings)
        {
            BUILDINGS.Add(building.GetComponent<Construction>().id, building);
        }
    }

    public void updateInventory(string key, int quantity)
    {
        INVENTORY.Remove(key);
        INVENTORY.Add(key, quantity);
    }
}