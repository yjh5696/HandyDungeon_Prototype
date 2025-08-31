using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance;
    
    private TouchAction _touchAction;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _touchAction = new TouchAction();
    }
    
    private void OnEnable()
    {
        _touchAction.Enable();
        _touchAction.Touch.Tap.performed += OnLogTapped;
    }

    private void OnLogTapped(InputAction.CallbackContext obj)
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("No camera found");
            return;
        }

        if (graphicRaycaster == null)
        {
            return;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = _touchAction.Touch.Position.ReadValue<Vector2>();
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);
        foreach (RaycastResult result in results.Where(result => result.gameObject.name == "Log"))
        {
            LogManager.Instance.ExpandLog();
        }
    }
    
    private void OnDisable()
    {
        _touchAction.Disable();
        _touchAction.Touch.Tap.performed -= OnLogTapped;
    }
}
