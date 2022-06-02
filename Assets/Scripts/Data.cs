using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public Dictionary<string, GameObject> BUILDINGS = new Dictionary<string, GameObject>();
    public Dictionary<string, Resource> RESOURCES = new Dictionary<string, Resource>();
    public Dictionary<string, int> INVENTORY = new Dictionary<string, int>();
    public Dictionary<string, int> BUILDING_INVENTORY = new Dictionary<string, int>();


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
    public static string NAIL = "nail";
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
    public static string HELLFIRE = "hellFire";
    public static string DIVINATIONBALL = "divinationBall";
    public static string IMMORTALITYELIXIR = "immortalityElixir";
    public static string SPELLBOOK = "spellBook";
    public static string POISONAPPLE = "poisonApple";
    public static string FANG = "fang";
    public static string WOLFCLAW = "wolfClaw";
      
    #endregion


    #region SINGLETON PATTERN
    public static Data Instance;

    //crear diccionaris publics de inventari i de crafteos

    //(x accedir desde altres scripts fer Data.Instance.inventaryDictionary. eetc
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        #region RESOURCES 

        #region GRAVEYARD

        //Bone
        Requirement[] requirements1;
        requirements1 = new Requirement[0];
        RESOURCES.Add(BONE, new Resource("Bone", BONE, true, 5, true, requirements1));

        //Rotten Flesh
        Requirement[] requirements2;
        Requirement requirement2 = new Requirement();

        requirements2 = new Requirement[1];

        requirement2.resourceNameKey = POISONIVY;
        requirement2.quantity = 4;
        requirements2[0] = requirement2;

        RESOURCES.Add(ROTTEN_FLESH, new Resource("Rotten flesh", ROTTEN_FLESH, false, 12, false, requirements2));

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

        RESOURCES.Add(TOMBSTONE, new Resource("Tombstone", TOMBSTONE, false, 60, false, requirements3));
        #endregion

        #region FOREST
        //SpiderWeb
        Requirement[] requirements4;
        Requirement requirement4 = new Requirement();

        requirements4 = new Requirement[1];

        requirement4.resourceNameKey = BONE;
        requirement4.quantity = 2;
        requirements4[0] = requirement4;

        RESOURCES.Add(SPIDERWEB, new Resource("Spiderweb", SPIDERWEB, false, 8, false, requirements4));

        //Lantern
        Requirement[] requirements5;
        Requirement requirement5 = new Requirement();

        requirements5 = new Requirement[1];

        requirement5.resourceNameKey = MUD;
        requirement5.quantity = 6;
        requirements5[0] = requirement2;

        RESOURCES.Add(LANTERN, new Resource("Lantern", LANTERN, false, 15, false, requirements5));

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

        RESOURCES.Add(DEADTREEBRANCH, new Resource("Dead tree branch", DEADTREEBRANCH, false, 90, false, requirements6));
        #endregion

        #region VEGETABLE PATCH

        //Poison Ivy
        Requirement[] requirements7;
        requirements7 = new Requirement[0];
        RESOURCES.Add(POISONIVY, new Resource("Poison ivy", POISONIVY, true, 5, false, requirements7));

        //Pumpkin
        Requirement[] requirements8;
        requirements8 = new Requirement[0];
        RESOURCES.Add(PUMPKIN, new Resource("Pumpkin", PUMPKIN, true, 25, false, requirements8));

        //Black Rose
        Requirement[] requirements9;
        requirements9 = new Requirement[0];
        RESOURCES.Add(BLACKROSE, new Resource("Black rose", BLACKROSE, true, 50, false, requirements9));
        #endregion

        #region SWAMP

        //Mud
        Requirement[] requirements10;
        requirements10 = new Requirement[0];
        RESOURCES.Add(MUD, new Resource("Mud", MUD, true, 10, false, requirements10));

        //Swamp water
        Requirement[] requirements11;
        requirements11 = new Requirement[0];
        RESOURCES.Add(SWAMPWATER, new Resource("Swamp water", SWAMPWATER, true, 30, false, requirements11));

        //Frog leg
        Requirement[] requirements12;
        requirements12 = new Requirement[0];
        RESOURCES.Add(FROGLEG, new Resource("Frog leg", FROGLEG, true, 90, false, requirements12));
        #endregion

        #region WELL

        //Dead fish
        Requirement[] requirements13;
        requirements13 = new Requirement[0];
        RESOURCES.Add(DEADFISH, new Resource("Dead fish", DEADFISH, true, 8, false, requirements13));

        //Bat wing
        Requirement[] requirements14;
        requirements14 = new Requirement[0];
        RESOURCES.Add(BATWING, new Resource("Bat wing", BATWING, true, 20, false, requirements14));

        //Nail (clavo)
        Requirement[] requirements15;
        requirements15 = new Requirement[0];
        RESOURCES.Add(NAIL, new Resource("Nail", NAIL, true, 40, false, requirements15));
        #endregion

        #region ABANDONED HOSPITAL

        //Bandages
        Requirement[] requirements16;
        requirements16 = new Requirement[0];
        RESOURCES.Add(BANDAGES, new Resource("Bandages", BANDAGES, true, 10, false, requirements16));

        //Eye
        Requirement[] requirements17;
        Requirement requirement17 = new Requirement();

        requirements17 = new Requirement[1];

        requirement17.resourceNameKey = DEADFISH;
        requirement17.quantity = 10;
        requirements17[0] = requirement17;

        RESOURCES.Add(EYE, new Resource("Eye", EYE, false, 25, false, requirements17));

        //Blood
        Requirement[] requirements18;
        Requirement requirement18A = new Requirement();
        Requirement requirement18B = new Requirement();

        requirements18 = new Requirement[2];

        requirement18A.resourceNameKey = SPIRITSOUL;
        requirement18A.quantity = 3;
        requirements18[0] = requirement18A;

        requirement18B.resourceNameKey = NAIL;
        requirement18B.quantity = 15;
        requirements18[1] = requirement18B;

        RESOURCES.Add(BLOOD, new Resource("Blood", BLOOD, false, 180, false, requirements18));
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

        RESOURCES.Add(PLASMA, new Resource("Plasma", PLASMA, false, 20, false, requirements19));

        //Ouija Board
        Requirement[] requirements20;
        Requirement requirement20 = new Requirement();

        requirements20 = new Requirement[1];

        requirement20.resourceNameKey = LANTERN;
        requirement20.quantity = 3;
        requirements20[0] = requirement20;

        RESOURCES.Add(OUIJABOARD, new Resource("Ouija board", OUIJABOARD, false, 20, false, requirements20));

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

        RESOURCES.Add(SPIRITSOUL, new Resource("Spirit soul", SPIRITSOUL, false, 20, false, requirements21));
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

        RESOURCES.Add(GARGOYLESTONE, new Resource("Gargoyle stone", GARGOYLESTONE, false, 20, false, requirements22));

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

        RESOURCES.Add(DEATHESSENCE, new Resource("Death essence", DEATHESSENCE, false, 20, false, requirements23));

        //Coffin
        Requirement[] requirements24;
        Requirement requirement24A = new Requirement();
        Requirement requirement24B = new Requirement();

        requirements24 = new Requirement[2];

        requirement24A.resourceNameKey = NAIL;
        requirement24A.quantity = 15;
        requirements24[0] = requirement24A;

        requirement24B.resourceNameKey = DEADTREEBRANCH;
        requirement24B.quantity = 10;
        requirements24[1] = requirement24B;

        RESOURCES.Add(COFFIN, new Resource("Coffin", COFFIN, false, 20, false, requirements24));
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

        RESOURCES.Add(BLACKCATHAIR, new Resource("Black cat hair", BLACKCATHAIR, false, 20, false, requirements25));

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

        RESOURCES.Add(BROOMSTICK, new Resource("Broomstick", BROOMSTICK, false, 40, false, requirements26));

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

        RESOURCES.Add(WITCHHAT, new Resource("Witch hat", WITCHHAT, false, 240, false, requirements27));
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

        RESOURCES.Add(SKULL, new Resource("Skull", SKULL, false, 30, false, requirements28));

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

        RESOURCES.Add(HORNS, new Resource("Horns", HORNS, false, 60, false, requirements29));

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

        RESOURCES.Add(HELLFIRE, new Resource("Hell fire", HELLFIRE, false, 270, false, requirements30));
        #endregion

        #region MAGIC WORKSHOP

        //Divination ball
        Requirement[] requirements31;
        Requirement requirement31 = new Requirement();

        requirements31 = new Requirement[1];

        requirement31.resourceNameKey = SWAMPWATER;
        requirement31.quantity = 3;
        requirements31[0] = requirement31;

        RESOURCES.Add(DIVINATIONBALL, new Resource("Divination ball", DIVINATIONBALL, false, 20, false, requirements31));

        //Immortality elixir
        Requirement[] requirements32;
        Requirement requirement32 = new Requirement();

        requirements32 = new Requirement[1];

        requirement32.resourceNameKey = DEADFISH;
        requirement32.quantity = 10;
        requirements32[0] = requirement32;

        RESOURCES.Add(IMMORTALITYELIXIR, new Resource("Immortality elixir", IMMORTALITYELIXIR, false, 20, false, requirements32));

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

        RESOURCES.Add(SPELLBOOK, new Resource("Spellbook", SPELLBOOK, false, 20, false, requirements33));
        #endregion

        #region DEEP FOREST

        //Poison apple
        Requirement[] requirements34;
        Requirement requirement34 = new Requirement();

        requirements34 = new Requirement[1];

        requirement34.resourceNameKey = BONE;
        requirement34.quantity = 10;
        requirements34[0] = requirement34;

        RESOURCES.Add(POISONAPPLE, new Resource("Poison apple", POISONAPPLE, false, 3, false, requirements34));

        //Fang
        Requirement[] requirements35;
        Requirement requirement35 = new Requirement();

        requirements35 = new Requirement[1];

        requirement35.resourceNameKey = SKULL;
        requirement35.quantity = 8;
        requirements35[0] = requirement35;

        RESOURCES.Add(FANG, new Resource("Fang", FANG, false, 12, false, requirements35));

        //Wolf's claw
        Requirement[] requirements36;
        Requirement requirement36 = new Requirement();

        requirements36 = new Requirement[1];

        requirement36.resourceNameKey = PLASMA;
        requirement36.quantity = 20;
        requirements36[0] = requirement36;

        RESOURCES.Add(WOLFCLAW, new Resource("Wolf's claw", WOLFCLAW, false, 30, false, requirements36));
        #endregion

        #endregion
    }
    #endregion

    //Al start de la shopManager es passa el array de buldings (prefabs) omplert manualment cap aqui i es guarda en el diccionari
    public void setBuildings(List<GameObject> buildings)
    {
        foreach(GameObject building in buildings)
        {
            BUILDINGS.Add(building.GetComponent<Building>().id, building);
        }
    }

    public void updateInventory(string key, int quantity)
    {
        INVENTORY.Remove(key);
        INVENTORY.Add(key, quantity);
    }
}
