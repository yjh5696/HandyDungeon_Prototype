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
        // 적이 없거나 체력이 0이면 버튼 비활성화
        if (EnemyManager.Instance == null || EnemyManager.Instance.Enemy == null || EnemyManager.Instance.Enemy.GetCurrentHp() <= 0)
            _button.interactable = false;
        else
            _button.interactable = true;
    }
}
