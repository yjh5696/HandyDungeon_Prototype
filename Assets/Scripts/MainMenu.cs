using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void OnClick()
    {
        SceneManager.LoadScene("Scenes/PrototypeScene");
    }
}
