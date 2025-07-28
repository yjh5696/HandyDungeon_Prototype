using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    [SerializeField] private GameObject prefab;
    public Enemy _enemy;
    
    public void SpawnEnemy(Sprite sprite, AnimatorController animatorController)
    {
        var enemy = Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);

        _enemy = enemy.GetComponent<Enemy>();
        _enemy.SetEnemy(sprite, animatorController);
    }
}
