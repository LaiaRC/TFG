using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public string id;
    public string monsterName;
    public int time;
    public List<Requirement> requirements;
    public List<List<Requirement>> upgradeRequirements;
    public List<Requirement> unlockRequirements;
    public List<float> velocity; //Each position corresponds to each upgrade
    public List<int> health;
    public List<int> damage;
    public List<float> attackRate;
    public List<float> attackRange;
    public List<int> level;
    public Sprite icon;
    public string description;
    public int upgradeLevel; //from 1 to 3
    public bool isUnlocked;

    public MonsterInfo(string id, string monsterName, int time, List<Requirement> requirements, List<List<Requirement>> upgradeRequirements, List<Requirement> unlockRequirements, List<float> velocity, List<int> health, List<int> damage, List<float> attackRate, List<float> attackRange, List<int> level, Sprite icon, string description, int upgradeLevel, bool isUnlocked)
    {
        this.id = id;
        this.monsterName = monsterName;
        this.time = time;
        this.requirements = requirements;
        this.upgradeRequirements = upgradeRequirements;
        this.unlockRequirements = unlockRequirements;
        this.velocity = velocity;
        this.health = health;
        this.damage = damage;
        this.attackRate = attackRate;
        this.attackRange = attackRange;
        this.level = level;
        this.icon = icon;
        this.description = description;
        this.upgradeLevel = upgradeLevel; //Nomes m'haure de guardar aixo i despres de reinici forçar que sigui 1
        this.isUnlocked = isUnlocked;
    }
}
