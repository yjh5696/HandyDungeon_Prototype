using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get => sprite; set => sprite = value; }
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    [SerializeField] private new string name;
    public string Name { get => name; set => name = value; }
    [SerializeField] private float health;
    public float Health { get => health; set => health = value; }
    [SerializeField] private string description;
    public string Description { get => description; set => description = value; }
    [SerializeField] private List<CardSO> enemyCards;
    public List<CardSO> EnemyCards { get => enemyCards; }
}
