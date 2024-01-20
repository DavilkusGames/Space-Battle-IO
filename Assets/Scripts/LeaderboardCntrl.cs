using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;
using System;

[Serializable]
public class LeaderboardSlot 
{
    public GameObject obj;
    public TMP_Text nicknameTxt;
    public TMP_Text scoreTxt;
}


public class LeaderboardCntrl : NetworkBehaviour
{
    public LeaderboardSlot[] lbSlots;
    public float updateDelay = 1f;
    private SyncList<int> leaderboardIDs = new SyncList<int>();

    private void Start()
    {
        if (isServer) InvokeRepeating(nameof(UpdateLeaderboardOnServer), updateDelay, updateDelay);
        if (isClient) InvokeRepeating(nameof(UpdateLeaderboardOnClient), updateDelay, updateDelay);
    }

    [Client]
    private void UpdateLeaderboardOnClient()
    {
        for (int i = 0; i < lbSlots.Length; i++)
        {
            if (i >= leaderboardIDs.Count) lbSlots[i].obj.SetActive(false);
            else
            {
                lbSlots[i].obj.SetActive(true);
                lbSlots[i].nicknameTxt.text = GameManager.Instance.GetPlayer(leaderboardIDs[i]).GetNickname();
                lbSlots[i].scoreTxt.text = GameManager.Instance.GetPlayer(leaderboardIDs[i]).score.ToString();
            }
        }
    }

    [Server]
    private void UpdateLeaderboardOnServer()
    {
        leaderboardIDs.Clear();

        List<PlayerCntrl> playersInLb = GameManager.Instance.allPlayers.ToList();
        playersInLb.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        foreach (var player in playersInLb)
        {
            leaderboardIDs.Add(player.Id);
        }
    }
}
