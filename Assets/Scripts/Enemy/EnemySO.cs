// EnemySO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("--- CSV 데이터 ---")]
    [SerializeField] private string enemyID;
    public string EnemyID { get => enemyID; set => enemyID = value; }

    // UnityEngine.Object의 'name'과 충돌을 피하기 위해 'enemyName'으로 변경
    [SerializeField] private string enemyName;
    public string EnemyName { get => enemyName; set => enemyName = value; }

    [SerializeField] private string enemyTribe;
    public string EnemyTribe { get => enemyTribe; set => enemyTribe = value; }

    [SerializeField] private string enemyChapter;
    public string EnemyChapter { get => enemyChapter; set => enemyChapter = value; }

    [SerializeField] private string element;
    public string Element { get => element; set => element = value; }

    [SerializeField] private string rank;
    public string Rank { get => rank; set => rank = value; }

    [SerializeField] private float health;
    public float Health { get => health; set => health = value; }

    [Header("--- 수동 할당 데이터 ---")]
    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get => sprite; set => sprite = value; }

    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }

    [SerializeField] private string description;
    public string Description { get => description; set => description = value; }

    [Header("--- 카드 데이터 ---")]
    [SerializeField] private List<CardDataSO> enemyCards = new List<CardDataSO>();
    public List<CardDataSO> EnemyCards { get => enemyCards; set => enemyCards = value; }
    //public List<CardDataSO> EnemyCards { get; set; } = new List<CardDataSO>();
}