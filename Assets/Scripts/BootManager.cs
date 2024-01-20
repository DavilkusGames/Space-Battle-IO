using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    public float loadDelay = 3.0f;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        Invoke(nameof(LoadMenu), loadDelay);
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(1);
    }
}
