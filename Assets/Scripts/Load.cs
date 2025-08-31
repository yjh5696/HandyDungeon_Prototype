using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Changed()
    {
        SceneManager.LoadScene("PrototypeScene");
    }
}
