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
    #endregion

    #region DROPS KEYS

    public static string SCARE = "scare";
    public static string LOLLIPOP = "lollipop";
    public static string RING = "ring";
    public static string BEER = "beer";
    public static string SWORD = "sword";
    public static string SHIELD = "shield";
    public static string STICK = "stick";
    public static string GEM = "gem";
    #endregion

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
    #endregion

    #region BOOSTS KEYS
    public static string PRODUCER_BOOST = "gargoyle";
    public static string CONVERTER_BOOST = "obelisk";
    public static string SUMMONING_BOOST = "bloodMoonTower";
    public static string OFFLINE_MAXTIME_BOOST = "spectre";
    public static string OFFLINE_PRODUCTIVITY_BOOST = "necromancer";
    public static string SCARES_BOOST = "mageGuardian";
    public static string DROPS_BOOST = "demonLord";
    public static string MERCHANT_BOOST = "merchant";
    #endregion

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
    #endregion

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
        RESOURCES.Add(BONE, new Resource("Bone", BONE, true, 5, true, requirements1, resourcesIcons[5]));

        //Rotten Flesh
        Requirement[] requirements2;
        Requirement requirement2 = new Requirement();

        requirements2 = new Requirement[1];

        requirement2.resourceNameKey = POISONIVY;
        requirement2.quantity = 4;
        requirements2[0] = requirement2;

        RESOURCES.Add(ROTTEN_FLESH, new Resource("Rotten flesh", ROTTEN_FLESH, false, 12, false, requirements2, resourcesIcons[26]));

        //Tombstone
        Requirement[] requirements3;
        Requirement requirement3A = new Requirement();
        Requirement requirement3B = new Requirement();

        requirements3 = new Requirement[2];

        requirement3A.resourceNameKey = SPIDERWEB;
        requirement3A.quantity = 50;
        requirements3[0] = requirement3A;

        requirement3B.resourceNameKey = GARGOYLESTONE;
        requirement3B.quantity = 8;
        requirements3[1] = requirement3B;

        RESOURCES.Add(TOMBSTONE, new Resource("Tombstone", TOMBSTONE, false, 60, false, requirements3, resourcesIcons[33]));
        #endregion

        #region FOREST
        //SpiderWeb
        Requirement[] requirements4;
        Requirement requirement4 = new Requirement();

        requirements4 = new Requirement[1];

        requirement4.resourceNameKey = BONE;
        requirement4.quantity = 2;
        requirements4[0] = requirement4;

        RESOURCES.Add(SPIDERWEB, new Resource("Spiderweb", SPIDERWEB, false, 8, false, requirements4, resourcesIcons[30]));

        //Lantern
        Requirement[] requirements5;
        Requirement requirement5 = new Requirement();

        requirements5 = new Requirement[1];

        requirement5.resourceNameKey = MUD;
        requirement5.quantity = 6;
        requirements5[0] = requirement2;

        RESOURCES.Add(LANTERN, new Resource("Lantern", LANTERN, false, 15, false, requirements5, resourcesIcons[19]));

        //Dead tree branch
        Requirement[] requirements6;
        Requirement requirement6A = new Requirement();
        Requirement requirement6B = new Requirement();

        requirements6 = new Requirement[2];

        requirement6A.resourceNameKey = BROOMSTICK;
        requirement6A.quantity = 9;
        requirements6[0] = requirement3A;

        requirement6B.resourceNameKey = POISONAPPLE;
        requirement6B.quantity = 30;
        requirements6[1] = requirement3B;

        RESOURCES.Add(DEADTREEBRANCH, new Resource("Dead tree branch", DEADTREEBRANCH, false, 90, false, requirements6, resourcesIcons[9]));
        #endregion

        #region VEGETABLE PATCH

        //Poison Ivy
        Requirement[] requirements7;
        requirements7 = new Requirement[0];
        RESOURCES.Add(POISONIVY, new Resource("Poison ivy", POISONIVY, true, 5, false, requirements7, resourcesIcons[24]));

        //Pumpkin
        Requirement[] requirements8;
        requirements8 = new Requirement[0];
        RESOURCES.Add(PUMPKIN, new Resource("Pumpkin", PUMPKIN, true, 25, false, requirements8, resourcesIcons[25]));

        //Black Rose
        Requirement[] requirements9;
        requirements9 = new Requirement[0];
        RESOURCES.Add(BLACKROSE, new Resource("Black rose", BLACKROSE, true, 50, false, requirements9, resourcesIcons[3]));
        #endregion

        #region SWAMP

        //Mud
        Requirement[] requirements10;
        requirements10 = new Requirement[0];
        RESOURCES.Add(MUD, new Resource("Mud", MUD, true, 10, false, requirements10, resourcesIcons[20]));

        //Swamp water
        Requirement[] requirements11;
        requirements11 = new Requirement[0];
        RESOURCES.Add(SWAMPWATER, new Resource("Swamp water", SWAMPWATER, true, 30, false, requirements11, resourcesIcons[32]));

        //Frog leg
        Requirement[] requirements12;
        requirements12 = new Requirement[0];
        RESOURCES.Add(FROGLEG, new Resource("Frog leg", FROGLEG, true, 90, false, requirements12, resourcesIcons[14]));
        #endregion

        #region WELL

        //Dead fish
        Requirement[] requirements13;
        requirements13 = new Requirement[0];
        RESOURCES.Add(DEADFISH, new Resource("Dead fish", DEADFISH, true, 8, false, requirements13, resourcesIcons[8]));

        //Bat wing
        Requirement[] requirements14;
        requirements14 = new Requirement[0];
        RESOURCES.Add(BATWING, new Resource("Bat wing", BATWING, true, 20, false, requirements14, resourcesIcons[1]));

        //Rusty Nail (clavo)
        Requirement[] requirements15;
        requirements15 = new Requirement[0];
        RESOURCES.Add(RUSTYNAIL, new Resource("Rusty nail", RUSTYNAIL, true, 40, false, requirements15, resourcesIcons[27]));
        #endregion

        #region ABANDONED HOSPITAL

        //Bandages
        Requirement[] requirements16;
        requirements16 = new Requirement[0];
        RESOURCES.Add(BANDAGES, new Resource("Bandages", BANDAGES, true, 10, false, requirements16, resourcesIcons[0]));

        //Eye
        Requirement[] requirements17;
        Requirement requirement17 = new Requirement();

        requirements17 = new Requirement[1];

        requirement17.resourceNameKey = DEADFISH;
        requirement17.quantity = 10;
        requirements17[0] = requirement17;

        RESOURCES.Add(EYE, new Resource("Eye", EYE, false, 25, false, requirements17, resourcesIcons[12]));

        //Blood
        Requirement[] requirements18;
        Requirement requirement18A = new Requirement();
        Requirement requirement18B = new Requirement();

        requirements18 = new Requirement[2];

        requirement18A.resourceNameKey = SPIRITSOUL;
        requirement18A.quantity = 3;
        requirements18[0] = requirement18A;

        requirement18B.resourceNameKey = RUSTYNAIL;
        requirement18B.quantity = 15;
        requirements18[1] = requirement18B;

        RESOURCES.Add(BLOOD, new Resource("Blood", BLOOD, false, 180, false, requirements18, resourcesIcons[4]));
        #endregion

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
        requirement19B.quantity = 6;
        requirements19[1] = requirement19B;

        RESOURCES.Add(PLASMA, new Resource("Plasma", PLASMA, false, 20, false, requirements19, resourcesIcons[22]));

        //Ouija Board
        Requirement[] requirements20;
        Requirement requirement20 = new Requirement();

        requirements20 = new Requirement[1];

        requirement20.resourceNameKey = LANTERN;
        requirement20.quantity = 3;
        requirements20[0] = requirement20;

        RESOURCES.Add(OUIJABOARD, new Resource("Ouija board", OUIJABOARD, false, 20, false, requirements20, resourcesIcons[21]));

        //Spirit soul
        Requirement[] requirements21;
        Requirement requirement21A = new Requirement();
        Requirement requirement21B = new Requirement();

        requirements21 = new Requirement[2];

        requirement21A.resourceNameKey = FANG;
        requirement21A.quantity = 10;
        requirements21[0] = requirement21A;

        requirement21B.resourceNameKey = BLACKROSE;
        requirement21B.quantity = 15;
        requirements21[1] = requirement21B;

        RESOURCES.Add(SPIRITSOUL, new Resource("Spirit soul", SPIRITSOUL, false, 20, false, requirements21, resourcesIcons[31]));
        #endregion

        #region CRYPT

        //Gargoyle stone
        Requirement[] requirements22;
        Requirement requirement22A = new Requirement();
        Requirement requirement22B = new Requirement();

        requirements22 = new Requirement[2];

        requirement22A.resourceNameKey = BANDAGES;
        requirement22A.quantity = 5;
        requirements22[0] = requirement22A;

        requirement22B.resourceNameKey = MUD;
        requirement22B.quantity = 20;
        requirements22[1] = requirement22B;

        RESOURCES.Add(GARGOYLESTONE, new Resource("Gargoyle stone", GARGOYLESTONE, false, 20, false, requirements22, resourcesIcons[15]));

        //Death essence
        Requirement[] requirements23;
        Requirement requirement23A = new Requirement();
        Requirement requirement23B = new Requirement();

        requirements23 = new Requirement[2];

        requirement23A.resourceNameKey = BLACKCATHAIR;
        requirement23A.quantity = 40;
        requirements23[0] = requirement23A;

        requirement23B.resourceNameKey = WOLFCLAW;
        requirement23B.quantity = 10;
        requirements23[1] = requirement23B;

        RESOURCES.Add(DEATHESSENCE, new Resource("Death essence", DEATHESSENCE, false, 20, false, requirements23, resourcesIcons[10]));

        //Coffin
        Requirement[] requirements24;
        Requirement requirement24A = new Requirement();
        Requirement requirement24B = new Requirement();

        requirements24 = new Requirement[2];

        requirement24A.resourceNameKey = RUSTYNAIL;
        requirement24A.quantity = 15;
        requirements24[0] = requirement24A;

        requirement24B.resourceNameKey = DEADTREEBRANCH;
        requirement24B.quantity = 10;
        requirements24[1] = requirement24B;

        RESOURCES.Add(COFFIN, new Resource("Coffin", COFFIN, false, 20, false, requirements24, resourcesIcons[7]));
        #endregion

        #region WITCH COVEN

        //Black cat hair
        Requirement[] requirements25;
        Requirement requirement25A = new Requirement();
        Requirement requirement25B = new Requirement();

        requirements25 = new Requirement[2];

        requirement25A.resourceNameKey = POISONIVY;
        requirement25A.quantity = 20;
        requirements25[0] = requirement25A;

        requirement25B.resourceNameKey = OUIJABOARD;
        requirement25B.quantity = 2;
        requirements25[1] = requirement25B;

        RESOURCES.Add(BLACKCATHAIR, new Resource("Black cat hair", BLACKCATHAIR, false, 20, false, requirements25, resourcesIcons[2]));

        //Broomstick
        Requirement[] requirements26;
        Requirement requirement26A = new Requirement();
        Requirement requirement26B = new Requirement();

        requirements26 = new Requirement[2];

        requirement26A.resourceNameKey = PUMPKIN;
        requirement26A.quantity = 15;
        requirements26[0] = requirement26A;

        requirement26B.resourceNameKey = IMMORTALITYELIXIR;
        requirement26B.quantity = 5;
        requirements26[1] = requirement26B;

        RESOURCES.Add(BROOMSTICK, new Resource("Broomstick", BROOMSTICK, false, 40, false, requirements26, resourcesIcons[6]));

        //Witch hat
        Requirement[] requirements27;
        Requirement requirement27A = new Requirement();
        Requirement requirement27B = new Requirement();

        requirements27 = new Requirement[2];

        requirement27A.resourceNameKey = DEATHESSENCE;
        requirement27A.quantity = 8;
        requirements27[0] = requirement27A;

        requirement27B.resourceNameKey = HORNS;
        requirement27B.quantity = 6;
        requirements27[1] = requirement27B;

        RESOURCES.Add(WITCHHAT, new Resource("Witch hat", WITCHHAT, false, 240, false, requirements27, resourcesIcons[34]));
        #endregion

        #region HELL ISLAND

        //Skull
        Requirement[] requirements28;
        Requirement requirement28A = new Requirement();
        Requirement requirement28B = new Requirement();

        requirements28 = new Requirement[2];

        requirement28A.resourceNameKey = EYE;
        requirement28A.quantity = 10;
        requirements28[0] = requirement28A;

        requirement28B.resourceNameKey = DIVINATIONBALL;
        requirement28B.quantity = 5;
        requirements28[1] = requirement28B;

        RESOURCES.Add(SKULL, new Resource("Skull", SKULL, false, 30, false, requirements28, resourcesIcons[28]));

        //Horns
        Requirement[] requirements29;
        Requirement requirement29A = new Requirement();
        Requirement requirement29B = new Requirement();

        requirements29 = new Requirement[2];

        requirement29A.resourceNameKey = BATWING;
        requirement29A.quantity = 9;
        requirements29[0] = requirement29A;

        requirement29B.resourceNameKey = SPELLBOOK;
        requirement29B.quantity = 3;
        requirements29[1] = requirement29B;

        RESOURCES.Add(HORNS, new Resource("Horns", HORNS, false, 60, false, requirements29, resourcesIcons[17]));

        //Hell fire
        Requirement[] requirements30;
        Requirement requirement30A = new Requirement();
        Requirement requirement30B = new Requirement();

        requirements30 = new Requirement[2];

        requirement30A.resourceNameKey = SKULL;
        requirement30A.quantity = 10;
        requirements30[0] = requirement30A;

        requirement30B.resourceNameKey = ROTTEN_FLESH;
        requirement30B.quantity = 80;
        requirements30[1] = requirement30B;

        RESOURCES.Add(HELLFIRE, new Resource("Hell fire", HELLFIRE, false, 270, false, requirements30, resourcesIcons[16]));
        #endregion

        #region MAGIC WORKSHOP

        //Divination ball
        Requirement[] requirements31;
        Requirement requirement31 = new Requirement();

        requirements31 = new Requirement[1];

        requirement31.resourceNameKey = SWAMPWATER;
        requirement31.quantity = 3;
        requirements31[0] = requirement31;

        RESOURCES.Add(DIVINATIONBALL, new Resource("Divination ball", DIVINATIONBALL, false, 20, false, requirements31, resourcesIcons[11]));

        //Immortality elixir
        Requirement[] requirements32;
        Requirement requirement32 = new Requirement();

        requirements32 = new Requirement[1];

        requirement32.resourceNameKey = DEADFISH;
        requirement32.quantity = 10;
        requirements32[0] = requirement32;

        RESOURCES.Add(IMMORTALITYELIXIR, new Resource("Immortality elixir", IMMORTALITYELIXIR, false, 20, false, requirements32, resourcesIcons[18]));

        //Spellbook
        Requirement[] requirements33;
        Requirement requirement33A = new Requirement();
        Requirement requirement33B = new Requirement();

        requirements33 = new Requirement[2];

        requirement33A.resourceNameKey = FROGLEG;
        requirement33A.quantity = 3;
        requirements33[0] = requirement33A;

        requirement33B.resourceNameKey = TOMBSTONE;
        requirement33B.quantity = 9;
        requirements33[1] = requirement33B;

        RESOURCES.Add(SPELLBOOK, new Resource("Spellbook", SPELLBOOK, false, 20, false, requirements33, resourcesIcons[29]));
        #endregion

        #region DEEP FOREST

        //Poison apple
        Requirement[] requirements34;
        Requirement requirement34 = new Requirement();

        requirements34 = new Requirement[1];

        requirement34.resourceNameKey = BONE;
        requirement34.quantity = 10;
        requirements34[0] = requirement34;

        RESOURCES.Add(POISONAPPLE, new Resource("Poison apple", POISONAPPLE, false, 3, false, requirements34, resourcesIcons[23]));

        //Fang
        Requirement[] requirements35;
        Requirement requirement35 = new Requirement();

        requirements35 = new Requirement[1];

        requirement35.resourceNameKey = SKULL;
        requirement35.quantity = 8;
        requirements35[0] = requirement35;

        RESOURCES.Add(FANG, new Resource("Fang", FANG, false, 12, false, requirements35, resourcesIcons[13]));

        //Wolf's claw
        Requirement[] requirements36;
        Requirement requirement36 = new Requirement();

        requirements36 = new Requirement[1];

        requirement36.resourceNameKey = PLASMA;
        requirement36.quantity = 20;
        requirements36[0] = requirement36;

        RESOURCES.Add(WOLFCLAW, new Resource("Wolf's claw", WOLFCLAW, false, 30, false, requirements36, resourcesIcons[35]));
        #endregion
        #endregion
        
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
            #endregion
        }
        #endregion

        #region PLAYER
        if (GameManager.Instance != null)
        {
            PLAYER.Add("Hour", GameManager.Instance.localDate.Hour);
            PLAYER.Add("Minute", GameManager.Instance.localDate.Minute);
            PLAYER.Add("Second", GameManager.Instance.localDate.Second);
            PLAYER.Add("Day", GameManager.Instance.localDate.Day);
            PLAYER.Add("Month", GameManager.Instance.localDate.Month);
            PLAYER.Add("Year", GameManager.Instance.localDate.Year);
        }
        else
        {
            PLAYER.Add("Hour", OfflineCalculator.Instance.localDate.Hour);
            PLAYER.Add("Minute", OfflineCalculator.Instance.localDate.Minute);
            PLAYER.Add("Second", OfflineCalculator.Instance.localDate.Second);
            PLAYER.Add("Day", OfflineCalculator.Instance.localDate.Day);
            PLAYER.Add("Month", OfflineCalculator.Instance.localDate.Month);
            PLAYER.Add("Year", OfflineCalculator.Instance.localDate.Year);
        }
        #endregion

        #region MONSTERS 

        //Skeleton
        List<Requirement> reqSkeleton = new List<Requirement>();

        reqSkeleton.Add(new Requirement(BONE, 3000));
        reqSkeleton.Add(new Requirement(PLASMA, 1500));

        List<List<Requirement>> reqUpgradeSkeleton = new List<List<Requirement>>();

        List<Requirement> reqUpgradeSkeleton1 = new List<Requirement>();
        reqUpgradeSkeleton1.Add(new Requirement(PLASMA, 3000));
        reqUpgradeSkeleton1.Add(new Requirement(BONE, 1500));

        List<Requirement> reqUpgradeSkeleton2 = new List<Requirement>();
        reqUpgradeSkeleton2.Add(new Requirement(DIVINATIONBALL, 2000));
        reqUpgradeSkeleton2.Add(new Requirement(BONE, 1500));

        reqUpgradeSkeleton.Add(reqUpgradeSkeleton1);
        reqUpgradeSkeleton.Add(reqUpgradeSkeleton2);

        List<Requirement> reqUnlockSkeleton = new List<Requirement>();

        reqUnlockSkeleton.Add(new Requirement(PLASMA, 30));

        #region STATS

        List<float> velSkeleton = new List<float>();
        velSkeleton.Add(4);
        velSkeleton.Add(4.5f);
        velSkeleton.Add(5);

        List<int> healthSkeleton = new List<int>();
        healthSkeleton.Add(2);
        healthSkeleton.Add(2);
        healthSkeleton.Add(3);

        List<int> damageSkeleton = new List<int>();
        damageSkeleton.Add(1);
        damageSkeleton.Add(2);
        damageSkeleton.Add(2);

        List<float> aRateSkeleton = new List<float>();
        aRateSkeleton.Add(2);
        aRateSkeleton.Add(1.5f);
        aRateSkeleton.Add(1);

        List<float> aRangeSkeleton = new List<float>();
        aRangeSkeleton.Add(4.5f);
        aRangeSkeleton.Add(5);
        aRangeSkeleton.Add(5.5f);

        List<int> levelSkeleton = new List<int>();
        levelSkeleton.Add(1);
        levelSkeleton.Add(1);
        levelSkeleton.Add(2);
        #endregion

        string desSkeleton = "It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";
        MONSTERS.Add(SKELETON, new MonsterInfo(SKELETON, "Skeleton", 3600, reqSkeleton, reqUpgradeSkeleton, reqUnlockSkeleton, velSkeleton, healthSkeleton, damageSkeleton, aRateSkeleton, aRangeSkeleton, levelSkeleton, monstersIcons[0], desSkeleton, 1, true));

        //JackOLantern
        List<Requirement> reqJack = new List<Requirement>();

        reqJack.Add(new Requirement(PUMPKIN, 3000));
        reqJack.Add(new Requirement(LANTERN, 1500));

        List<List<Requirement>> reqUpgradeJack = new List<List<Requirement>>();
        List<Requirement> reqUpgradeJack1 = new List<Requirement>();
        List<Requirement> reqUpgradeJack2 = new List<Requirement>();

        reqUpgradeJack1.Add(new Requirement(PLASMA, 3000));
        reqUpgradeJack1.Add(new Requirement(BONE, 1500));

        reqUpgradeJack2.Add(new Requirement(PLASMA, 30));
        reqUpgradeJack2.Add(new Requirement(BONE, 1500));

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
        healthJack.Add(6);
        healthJack.Add(8);
        healthJack.Add(10);

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
        levelJack.Add(2);
        levelJack.Add(3);
        #endregion

        string desJack = "JackO It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(JACK_LANTERN, new MonsterInfo(JACK_LANTERN, "Jack-o'-lantern", 5400, reqJack, reqUpgradeJack, reqUnlockJack, velJack, healthJack, damageJack, aRateJack, aRangeJack, levelJack, monstersIcons[1], desJack, 1, false));

        //Bat
        List<Requirement> reqBat = new List<Requirement>();

        reqBat.Add(new Requirement(BATWING, 2500));
        reqBat.Add(new Requirement(DEADTREEBRANCH, 1000));

        List<List<Requirement>> reqUpgradeBat = new List<List<Requirement>>();
        List<Requirement> reqUpgradeBat1 = new List<Requirement>();
        List<Requirement> reqUpgradeBat2 = new List<Requirement>();

        reqUpgradeBat1.Add(new Requirement(PLASMA, 3000));
        reqUpgradeBat1.Add(new Requirement(BONE, 1500));

        reqUpgradeBat2.Add(new Requirement(EYE, 3000));
        reqUpgradeBat2.Add(new Requirement(BONE, 1500));

        reqUpgradeBat.Add(reqUpgradeBat1);
        reqUpgradeBat.Add(reqUpgradeBat2);

        List<Requirement> reqUnlockBat = new List<Requirement>();

        reqUnlockBat.Add(new Requirement(BATWING, 30));

        #region STATS

        List<float> velBat = new List<float>();
        velBat.Add(5);
        velBat.Add(5.5f);
        velBat.Add(6);

        List<int> healthBat = new List<int>();
        healthBat.Add(2);
        healthBat.Add(3);
        healthBat.Add(4);

        List<int> damageBat = new List<int>();
        damageBat.Add(2);
        damageBat.Add(3);
        damageBat.Add(4);

        List<float> aRateBat = new List<float>();
        aRateBat.Add(2);
        aRateBat.Add(1.5f);
        aRateBat.Add(1);

        List<float> aRangeBat = new List<float>();
        aRangeBat.Add(4.5f);
        aRangeBat.Add(5);
        aRangeBat.Add(5.5f);

        List<int> levelBat = new List<int>();
        levelBat.Add(2);
        levelBat.Add(2);
        levelBat.Add(3);
        #endregion

        string desBat = "Bat It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(BAT, new MonsterInfo(BAT, "Bat", 7200, reqBat, reqUpgradeBat, reqUnlockBat, velBat, healthBat, damageBat, aRateBat, aRangeBat, levelBat, monstersIcons[2], desBat, 1, false));

        //Goblin
        List<Requirement> reqGoblin = new List<Requirement>();

        reqGoblin.Add(new Requirement(DEADFISH, 2500));
        reqGoblin.Add(new Requirement(SWAMPWATER, 1000));
        reqGoblin.Add(new Requirement(LOLLIPOP, 10)); //Si te 3r parametre el ultim sera un drop

        List<List<Requirement>> reqUpgradeGoblin = new List<List<Requirement>>();
        List<Requirement> reqUpgradeGoblin1 = new List<Requirement>();
        List<Requirement> reqUpgradeGoblin2 = new List<Requirement>();

        reqUpgradeGoblin1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeGoblin1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeGoblin2.Add(new Requirement(EYE, 2500));
        reqUpgradeGoblin2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeGoblin.Add(reqUpgradeGoblin1);
        reqUpgradeGoblin.Add(reqUpgradeGoblin2);

        List<Requirement> reqUnlockGoblin = new List<Requirement>();

        reqUnlockGoblin.Add(new Requirement(SWAMPWATER, 30));

        #region STATS

        List<float> velGoblin = new List<float>();
        velGoblin.Add(6);
        velGoblin.Add(7);
        velGoblin.Add(8);

        List<int> healthGoblin = new List<int>();
        healthGoblin.Add(3);
        healthGoblin.Add(3);
        healthGoblin.Add(4);

        List<int> damageGoblin = new List<int>();
        damageGoblin.Add(2);
        damageGoblin.Add(3);
        damageGoblin.Add(4);

        List<float> aRateGoblin = new List<float>();
        aRateGoblin.Add(1);
        aRateGoblin.Add(0.5f);
        aRateGoblin.Add(0.25f);

        List<float> aRangeGoblin = new List<float>();
        aRangeGoblin.Add(4.5f);
        aRangeGoblin.Add(5);
        aRangeGoblin.Add(5.5f);

        List<int> levelGoblin = new List<int>();
        levelGoblin.Add(2);
        levelGoblin.Add(2);
        levelGoblin.Add(3);
        #endregion

        string desGoblin = "goblin It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(GOBLIN, new MonsterInfo(GOBLIN, "Goblin", 9000, reqGoblin, reqUpgradeGoblin, reqUnlockGoblin, velGoblin, healthGoblin, damageGoblin, aRateGoblin, aRangeGoblin, levelGoblin, monstersIcons[3], desGoblin, 1, false));

        //Ghost
        List<Requirement> reqGhost = new List<Requirement>();

        reqGhost.Add(new Requirement(IMMORTALITYELIXIR, 2000));
        reqGhost.Add(new Requirement(SPIRITSOUL, 800));
        reqGhost.Add(new Requirement(RING, 10));

        List<List<Requirement>> reqUpgradeGhost = new List<List<Requirement>>();
        List<Requirement> reqUpgradeGhost1 = new List<Requirement>();
        List<Requirement> reqUpgradeGhost2 = new List<Requirement>();

        reqUpgradeGhost1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeGhost1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeGhost2.Add(new Requirement(DEATHESSENCE, 2500));
        reqUpgradeGhost2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeGhost.Add(reqUpgradeGhost1);
        reqUpgradeGhost.Add(reqUpgradeGhost2);

        List<Requirement> reqUnlockGhost = new List<Requirement>();

        reqUnlockGhost.Add(new Requirement(SPIRITSOUL, 30));

        #region STATS

        List<float> velGhost = new List<float>();
        velGhost.Add(4.5f);
        velGhost.Add(5);
        velGhost.Add(5.5f);

        List<int> healthGhost = new List<int>();
        healthGhost.Add(6);
        healthGhost.Add(7);
        healthGhost.Add(8);

        List<int> damageGhost = new List<int>();
        damageGhost.Add(3);
        damageGhost.Add(3);
        damageGhost.Add(4);

        List<float> aRateGhost = new List<float>();
        aRateGhost.Add(1.5f);
        aRateGhost.Add(1);
        aRateGhost.Add(1);

        List<float> aRangeGhost = new List<float>();
        aRangeGhost.Add(4.5f);
        aRangeGhost.Add(5);
        aRangeGhost.Add(5.5f);

        List<int> levelGhost = new List<int>();
        levelGhost.Add(3);
        levelGhost.Add(3);
        levelGhost.Add(4);
        #endregion

        string desGhost = "ghost It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(GHOST, new MonsterInfo(GHOST, "Ghost", 10800, reqGhost, reqUpgradeGhost, reqUnlockGhost, velGhost, healthGhost, damageGhost, aRateGhost, aRangeGhost, levelGhost, monstersIcons[4], desGhost, 1, false));

        //Clown
        List<Requirement> reqClown = new List<Requirement>();

        reqClown.Add(new Requirement(BLACKCATHAIR, 2000));
        reqClown.Add(new Requirement(HORNS, 2000));
        reqClown.Add(new Requirement(BEER, 10));

        List<List<Requirement>> reqUpgradeClown = new List<List<Requirement>>();
        List<Requirement> reqUpgradeClown1 = new List<Requirement>();
        List<Requirement> reqUpgradeClown2 = new List<Requirement>();

        reqUpgradeClown1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeClown1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeClown2.Add(new Requirement(DIVINATIONBALL, 2500));
        reqUpgradeClown2.Add(new Requirement(DEADFISH, 1000));

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
        healthClown.Add(4);
        healthClown.Add(5);
        healthClown.Add(6);

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
        aRangeClown.Add(7);
        aRangeClown.Add(8);

        List<int> levelClown = new List<int>();
        levelClown.Add(3);
        levelClown.Add(3);
        levelClown.Add(4);
        #endregion

        string desClown = "clown It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(CLOWN, new MonsterInfo(CLOWN, "Clown", 12600, reqClown, reqUpgradeClown, reqUnlockClown, velClown, healthClown, damageClown, aRateClown, aRangeClown, levelClown, monstersIcons[5], desClown, 1, false));

        //Zombie
        List<Requirement> reqZombie = new List<Requirement>();

        reqZombie.Add(new Requirement(ROTTEN_FLESH, 5000));
        reqZombie.Add(new Requirement(DEATHESSENCE, 300));
        reqZombie.Add(new Requirement(SWORD, 10));

        List<List<Requirement>> reqUpgradeZombie = new List<List<Requirement>>();
        List<Requirement> reqUpgradeZombie1 = new List<Requirement>();
        List<Requirement> reqUpgradeZombie2 = new List<Requirement>();

        reqUpgradeZombie1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeZombie1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeZombie2.Add(new Requirement(BLACKROSE, 2500));
        reqUpgradeZombie2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeZombie.Add(reqUpgradeZombie1);
        reqUpgradeZombie.Add(reqUpgradeZombie2);

        List<Requirement> reqUnlockZombie = new List<Requirement>();

        reqUnlockZombie.Add(new Requirement(DEATHESSENCE, 30));


        #region STATS

        List<float> velZombie = new List<float>();
        velZombie.Add(3);
        velZombie.Add(4);
        velZombie.Add(4.5f);

        List<int> healthZombie = new List<int>();
        healthZombie.Add(8);
        healthZombie.Add(9);
        healthZombie.Add(10);

        List<int> damageZombie = new List<int>();
        damageZombie.Add(5);
        damageZombie.Add(6);
        damageZombie.Add(8);

        List<float> aRateZombie = new List<float>();
        aRateZombie.Add(2);
        aRateZombie.Add(1.5f);
        aRateZombie.Add(1);

        List<float> aRangeZombie = new List<float>();
        aRangeZombie.Add(4);
        aRangeZombie.Add(4.5f);
        aRangeZombie.Add(5);

        List<int> levelZombie = new List<int>();
        levelZombie.Add(3);
        levelZombie.Add(4);
        levelZombie.Add(4);
        #endregion

        string desZombie = "zombie It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(ZOMBIE, new MonsterInfo(ZOMBIE, "Zombie", 14400, reqZombie, reqUpgradeZombie, reqUnlockZombie, velZombie, healthZombie, damageZombie, aRateZombie, aRangeZombie, levelZombie, monstersIcons[6], desZombie, 1, false));

        //Vampire
        List<Requirement> reqVampire = new List<Requirement>();

        reqVampire.Add(new Requirement(BLOOD, 1000));
        reqVampire.Add(new Requirement(COFFIN, 300));
        reqVampire.Add(new Requirement(STICK, 10));

        List<List<Requirement>> reqUpgradeVampire = new List<List<Requirement>>();
        List<Requirement> reqUpgradeVampire1 = new List<Requirement>();
        List<Requirement> reqUpgradeVampire2 = new List<Requirement>();

        reqUpgradeVampire1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeVampire1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeVampire2.Add(new Requirement(HELLFIRE, 2500));
        reqUpgradeVampire2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeVampire.Add(reqUpgradeVampire1);
        reqUpgradeVampire.Add(reqUpgradeVampire2);

        List<Requirement> reqUnlockVampire = new List<Requirement>();

        reqUnlockVampire.Add(new Requirement(COFFIN, 30));

        #region STATS

        List<float> velVampire = new List<float>();
        velVampire.Add(5.5f);
        velVampire.Add(6);
        velVampire.Add(6.5f);

        List<int> healthVampire = new List<int>();
        healthVampire.Add(9);
        healthVampire.Add(10);
        healthVampire.Add(11);

        List<int> damageVampire = new List<int>();
        damageVampire.Add(5);
        damageVampire.Add(5);
        damageVampire.Add(6);

        List<float> aRateVampire = new List<float>();
        aRateVampire.Add(1.5f);
        aRateVampire.Add(1);
        aRateVampire.Add(1);

        List<float> aRangeVampire = new List<float>();
        aRangeVampire.Add(5);
        aRangeVampire.Add(5.5f);
        aRangeVampire.Add(6);

        List<int> levelVampire = new List<int>();
        levelVampire.Add(4);
        levelVampire.Add(4);
        levelVampire.Add(5);
        #endregion

        string desVampire = "vampire It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(VAMPIRE, new MonsterInfo(VAMPIRE, "Vampire", 16200, reqVampire, reqUpgradeVampire, reqUnlockVampire, velVampire, healthVampire, damageVampire, aRateVampire, aRangeVampire, levelVampire, monstersIcons[7], desVampire, 1, false));

        //Witch
        List<Requirement> reqWitch = new List<Requirement>();

        reqWitch.Add(new Requirement(WITCHHAT, 2000));
        reqWitch.Add(new Requirement(SPELLBOOK, 4000));
        reqWitch.Add(new Requirement(SHIELD, 10));

        List<List<Requirement>> reqUpgradeWitch = new List<List<Requirement>>();
        List<Requirement> reqUpgradeWitch1 = new List<Requirement>();
        List<Requirement> reqUpgradeWitch2 = new List<Requirement>();

        reqUpgradeWitch1.Add(new Requirement(SWAMPWATER, 2500));
        reqUpgradeWitch1.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeWitch2.Add(new Requirement(GEM, 2500));
        reqUpgradeWitch2.Add(new Requirement(DEADFISH, 1000));

        reqUpgradeWitch.Add(reqUpgradeWitch1);
        reqUpgradeWitch.Add(reqUpgradeWitch2);

        List<Requirement> reqUnlockWitch = new List<Requirement>();

        reqUnlockWitch.Add(new Requirement(WITCHHAT, 30));

        #region STATS

        List<float> velWitch = new List<float>();
        velWitch.Add(4);
        velWitch.Add(4.5f);
        velWitch.Add(5);

        List<int> healthWitch = new List<int>();
        healthWitch.Add(11);
        healthWitch.Add(13);
        healthWitch.Add(15);

        List<int> damageWitch = new List<int>();
        damageWitch.Add(4);
        damageWitch.Add(5);
        damageWitch.Add(6);

        List<float> aRateWitch = new List<float>();
        aRateWitch.Add(2);
        aRateWitch.Add(1.5f);
        aRateWitch.Add(1);

        List<float> aRangeWitch = new List<float>();
        aRangeWitch.Add(4.5f);
        aRangeWitch.Add(5);
        aRangeWitch.Add(5.5f);

        List<int> levelWitch = new List<int>();
        levelWitch.Add(4);
        levelWitch.Add(5);
        levelWitch.Add(5);
        #endregion

        string desWitch = "witch It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(WITCH, new MonsterInfo(WITCH, "Witch", 18000, reqWitch, reqUpgradeWitch, reqUnlockWitch, velWitch, healthWitch, damageWitch, aRateWitch, aRangeWitch, levelWitch, monstersIcons[8], desWitch, 1, false));

        //Reaper
        List<Requirement> reqReaper = new List<Requirement>();

        reqReaper.Add(new Requirement(LOLLIPOP, 100)); //S'haurà de modificar UI perque accepti tants requirements
        reqReaper.Add(new Requirement(RING, 85));
        reqReaper.Add(new Requirement(BEER, 70));
        reqReaper.Add(new Requirement(SWORD, 55));
        reqReaper.Add(new Requirement(STICK, 40));
        reqReaper.Add(new Requirement(SHIELD, 25));
        reqReaper.Add(new Requirement(GEM, 15));

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
        velReaper.Add(8);
        velReaper.Add(8);
        velReaper.Add(8);

        List<int> healthReaper = new List<int>();
        healthReaper.Add(100);
        healthReaper.Add(100);
        healthReaper.Add(100);

        List<int> damageReaper = new List<int>();
        damageReaper.Add(20);
        damageReaper.Add(20);
        damageReaper.Add(20);

        List<float> aRateReaper = new List<float>();
        aRateReaper.Add(0.5f);
        aRateReaper.Add(0.5f);
        aRateReaper.Add(0.5f);

        List<float> aRangeReaper = new List<float>();
        aRangeReaper.Add(8);
        aRangeReaper.Add(8);
        aRangeReaper.Add(8);

        List<int> levelReaper = new List<int>();
        levelReaper.Add(6);
        levelReaper.Add(6);
        levelReaper.Add(6);
        #endregion

        string desReaper = "reaper It's the most basic type of monster. Can only be invoked outside the village and it's favourite type of villager are children.";

        MONSTERS.Add(REAPER, new MonsterInfo(REAPER, "The Reaper", 28800, reqReaper, reqUpgradeReaper, reqUnlockReaper, velReaper, healthReaper, damageReaper, aRateReaper, aRangeReaper, levelReaper, monstersIcons[9], desReaper, 1, false));

        #endregion

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
            #endregion

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
            #endregion
        }
    }
    #endregion

    //Al start de la shopManager es passa el array de buldings (prefabs) omplert manualment cap aqui i es guarda en el diccionari
    public void setBuildings(List<GameObject> buildings)
    {
        foreach(GameObject building in buildings)
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
