using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get => sprite; }
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    [SerializeField] private new string name;
    public string Name { get => name; }
    [SerializeField] private int health;
    public int Health { get => health; }
    [SerializeField] private string description;
    public string Description { get => description; }
    [SerializeField] private List<CardSO> enemyCards;
    public List<CardSO> EnemyCards { get => enemyCards; }
}
