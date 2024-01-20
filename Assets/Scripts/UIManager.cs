using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject dangerZonePanel;
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetDangerZoneState(bool state)
    {
        dangerZonePanel.SetActive(state);
    }
}
