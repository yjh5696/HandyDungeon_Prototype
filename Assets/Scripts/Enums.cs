using UnityEngine;


public enum Style
{
    Attack,
    Defence,
    Special
}

public enum State
{
    None = 0,
    Fire = 1,
    Water = 2,
    Wind= 3,
    Earth = 4,
    Ignition = 5,
    Fervor = 6,
    Gale = 7,
    Guard = 8,
    Recovery = 9,
    //Shield = 10,
    //Healing = 11,
    Vibration = 12,
    Burndown = 13,
}

public enum EnemyStatusEffect
{
    None = 0,
    Fire = 1,
    Water = 2,
    Wind = 3,
}

public enum PlayerStatusEffect
{
    None = 0,
    Fire = 1,
    Water = 2,
    Wind = 3,
}

public enum ChoiceType
{
    Event = 0,
    Treasure = 1,
    Battle = 2,
    Rest = 3,
}