using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public List<PlayerCntrl> allPlayers = new List<PlayerCntrl>();
    public static GameManager Instance;
    public Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public int GetPlayersCount() { return allPlayers.Count; }
    public PlayerCntrl GetPlayer(int id) { return allPlayers[id]; }

    public Vector3 GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    public void AddPlayer(PlayerCntrl player)
    {
        allPlayers.Add(player);
        if (isServer) LogCntrl.Instance.ShowText(player.GetNickname() + " присоединился");
        player.Id = allPlayers.Count-1;
    }

    public void RemovePlayer(int id)
    {
        if (isServer) LogCntrl.Instance.ShowText(allPlayers[id].GetNickname() + " вышел");
        allPlayers.RemoveAt(id);
        for (int i = id; i < allPlayers.Count; i++) allPlayers[i].Id = i;
    }
}
