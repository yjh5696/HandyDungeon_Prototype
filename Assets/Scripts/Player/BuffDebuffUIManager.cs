using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffUIManager : MonoBehaviour
{
    public static BuffDebuffUIManager Instance { get; private set; }

    [Header("UI Containers")]
    public Transform playerBuffContainer;
    public Transform playerDebuffContainer;
    public Transform enemyBuffContainer;
    public Transform enemyDebuffContainer;

    [Header("Icon Prefabs")]
    public GameObject buffIconPrefab;
    public GameObject debuffIconPrefab;

    [Header("Sprites")]
    public Sprite fireIcon;
    public Sprite waterIcon;
    public Sprite airIcon;
    public Sprite landIcon;
    public Sprite ignitionIcon;
    public Sprite fervorIcon;
    public Sprite galeIcon;
    public Sprite guardIcon;
    public Sprite recoveryIcon;
    public Sprite vibrationIcon;
    public Sprite burndownIcon;

    private void Awake()
    {
        Instance = this;
    }

    bool IsDebuff(State state) => (int)state >= 1 && (int)state <= 5;

    Sprite GetIconSprite(State state)
    {
        switch (state)
        {
            case State.Fire: return fireIcon;
            case State.Water: return waterIcon;
            case State.Air: return airIcon;
            case State.Land: return landIcon;
            case State.Ignition: return ignitionIcon;
            case State.Fervor: return fervorIcon;
            case State.Gale: return galeIcon;
            case State.Guard: return guardIcon;
            case State.Recovery: return recoveryIcon;
            case State.Vibration: return vibrationIcon;
            case State.Burndown: return burndownIcon;
            default: return null;
        }
    }

    public void UpdateBuffDebuffUI(Dictionary<State, int> statesWithStacks, bool isPlayer)
    {
        Transform buffContainer = isPlayer ? playerBuffContainer : enemyBuffContainer;
        Transform debuffContainer = isPlayer ? playerDebuffContainer : enemyDebuffContainer;
        Debug.Log($"[UpdateBuffDebuffUI] isPlayer={isPlayer}, 버프 컨테이너={buffContainer?.name}, 디버프 컨테이너={debuffContainer?.name}");
        ClearChildren(buffContainer);
        ClearChildren(debuffContainer);
        foreach (var kvp in statesWithStacks)
        {
            State state = kvp.Key;
            int stacks = kvp.Value;
            Debug.Log($"상태: {state}, 스택: {stacks}");
            GameObject prefab = IsDebuff(state) ? debuffIconPrefab : buffIconPrefab;
            Transform container = IsDebuff(state) ? debuffContainer : buffContainer;

            if (prefab == null)
            {
                Debug.LogError($"아이콘 프리팹이 null입니다. State: {state}");
                continue;
            }
            if (container == null)
            {
                Debug.LogError($"UI 컨테이너가 null입니다. State: {state}, isDebuff: {IsDebuff(state)}");
                continue;
            }

            var sprite = GetIconSprite(state);
            if (sprite == null)
            {
                Debug.LogWarning($"스프라이트 없음. State: {state} → 아이콘 생성하지 않음");
                continue; // 예외처리: 스프라이트 없으면 아이콘 생성하지 않음
            }

            var iconObj = Instantiate(prefab, container);
            if (iconObj == null)
            {
                Debug.LogError($"아이콘 프리팹 인스턴스화 실패. State: {state}");
                continue;
            }

            var img = iconObj.GetComponentInChildren<Image>();
            if (img == null)
            {
                Debug.LogError($"Image 컴포넌트 없음. State: {state}");
            }
            else
            {
                img.sprite = sprite;
                Debug.Log($"스프라이트 할당 완료. State: {state}");
            }

            var stackText = iconObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (stackText == null)
            {
                Debug.LogWarning($"스택 텍스트 컴포넌트 없음. State: {state}");
            }
            else
            {
                stackText.text = stacks > 1 ? stacks.ToString() : "";
                Debug.Log($"스택 텍스트 설정 완료: {stackText.text}");
            }
        }
    }




    public void ClearChildren(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    public void ClearEnemyContainer()
    {
        ClearChildren(enemyBuffContainer);
        ClearChildren(enemyDebuffContainer);
    }
}

