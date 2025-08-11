using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] Choice choice1;
    [SerializeField] Choice choice2;
    [SerializeField] Choice choice3;
    [SerializeField] Choice choice4;
    private List<ChoiceSO> _choiceSOsList;
    private List<Choice> _choiceList;
    public static ChoiceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _choiceSOsList = new List<ChoiceSO>();
        _choiceList = new List<Choice>();
        _choiceList.Add(choice1);
        _choiceList.Add(choice2);
        _choiceList.Add(choice3);
        _choiceList.Add(choice4);
    }

    public void GetRandomChoice()
    {
        string[] paths = System.IO.Directory.GetFiles("Assets/SO/Choices","*.asset");
        ChoiceSO[] choice = new ChoiceSO[paths.Length];
        
        for(int i= 0; i< paths.Length; ++i)
        {
            choice[i] = (ChoiceSO)AssetDatabase.LoadAssetAtPath(paths[i], typeof(ChoiceSO));
        }

        for (int i = 0; i < 4; ++i)
        {
            while (true)
            {
                int r = Random.Range(0, choice.Length);
                ChoiceSO choiceSO = choice[r];
                if (!_choiceSOsList.Contains(choiceSO))
                {
                    _choiceSOsList.Add(choiceSO);
                    _choiceList[i].SetChoice(choiceSO.ChoiceName, choiceSO.ChoiceType, choiceSO.ChoiceDifficulty, choiceSO.ChoiceSuccess, choiceSO.ChoiceFail);
                    break;
                }
            }
        }
        
    }
}
