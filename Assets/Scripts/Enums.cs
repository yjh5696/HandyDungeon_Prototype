using UnityEngine;


public enum Style
{
    Action,
    Support,
    Special
}

public enum State
{
    None = 0,
    Fire = 1, // 점화
    Water = 2, // 침식
    Air = 3, // 풍식
    Land = 4, // 진창
    Ignition = 5, // 발화
    Fervor = 6, // 열정
    Gale = 7, // 질풍
    Guard = 8, // 수호
    Recovery = 9, // 회복
    //Shield = 10, // 보호막
    //Healing = 11, // 치유
    Vibration = 12, // 반동
    Burndown = 13, // 소화
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

public enum EventType
{
    MainStory = 0,
    SubStory = 1,
    Battle = 2,
    Boss = 3,
}

[System.Serializable]
public class GameCard
{
    public int C_Id;
    public string C_Name;
    public string C_Type;
    public string Element;
    public string Tier;
    public string Rare;
    public string Effect_Type;
    public float min_Value;
    public float Max_Vlaue;
    public string Calculation;
    public string Debuff_Type;
    public int Debuff_Stack;
    public string Buff_Type;
    public int Buff_Stack;
    public string Enhanceable;
    public int? Enhance_Count;
    public string Target;
    public string Card_Description;
    public string Unnamed_18; // 빈값 컬럼 대응
    public string CardConcept;
    public string Unnamed_20;
    public string Unnamed_21;
    public string Unnamed_22;
    public string Formula;
}

[System.Serializable]
public class ChoiceEvent
{
    public string choiceID;
    public int choiceChapterNumber;
    public int choiceEventNumber;
    public bool isRootEvent;
    public string parentChoiceID;
    public ChoiceType choiceEventType;
    public string choiceName;
    public string choiceText;
    public float choiceRate;
    public string choiceSuccessText;
    public string choiceFailText;
    public string choiceReward;
    public string choiceLoss;
    public string choiceRequirement;
}

[System.Serializable]
public class MainEvent
{
    public string choiceID;
    public int choiceStageNumber;
    public int choiceEventNumber;
    public ChoiceType choiceEventType;
    public string choiceName;
    public string choiceText;
    public float choiceRate;
    public string choiceSuccessText;
    public string choiceFailText;
    public string choiceReward;
    public string choiceLoss;
}

[System.Serializable]
public class StartScript
{
    public string scriptID;
    public string chapterID;
    public string eventID;
    public string scriptText;
    public float delayTime;
}