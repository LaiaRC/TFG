using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public string id;
    public string monsterName;
    public int time;
    public List<Requirement> requirements;
    public float velocity;
    public int health;
    public int damage;
    public float attackRate;
    public float attackRange;
    public int level;
    public Sprite icon;

    public MonsterInfo(string id, string monsterName, int time, List<Requirement> requirements, float velocity, int health, int damage, float attackRate, float attackRange, int level, Sprite icon)
    {
        this.id = id;
        this.monsterName = monsterName;
        this.time = time;
        this.requirements = requirements;
        this.velocity = velocity;
        this.health = health;
        this.damage = damage;
        this.attackRate = attackRate;
        this.attackRange = attackRange;
        this.level = level;
        this.icon = icon;
    }
}
