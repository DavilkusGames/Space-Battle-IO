using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject dangerZonePanel;
    public GameObject respawnPanel;
    public TMP_Text respawnTimerTxt;
    public TMP_Text scoreTxt;
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void UpdateScoreTxt(int score)
    {
        scoreTxt.text = "SCORE: " + score.ToString();
    }

    public void SetRespawnTime(int time)
    {
        respawnTimerTxt.text = time.ToString() + "...";
    }

    public void SetRespawnPanelState(bool state)
    {
        respawnPanel.SetActive(state);
    }

    public void SetDangerZoneState(bool state)
    {
        dangerZonePanel.SetActive(state);
    }
}
