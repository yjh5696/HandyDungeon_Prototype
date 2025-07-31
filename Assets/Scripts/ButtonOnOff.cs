using UnityEngine;
using UnityEngine.UI;

public class ButtonOnOff : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        // ���� ���ų� ü���� 0�̸� ��ư ��Ȱ��ȭ
        if (EnemyManager.Instance == null || EnemyManager.Instance.Enemy == null || EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
            _button.interactable = false;
        else
            _button.interactable = true;
    }
}
