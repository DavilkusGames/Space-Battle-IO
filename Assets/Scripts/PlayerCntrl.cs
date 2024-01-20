using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using System.Runtime.InteropServices;

public class PlayerCntrl : NetworkBehaviour
{
    public TMP_Text nicknameTxt;
    [SyncVar(hook = nameof(NicknameChanged))] private string nickname = string.Empty;
    [SyncVar(hook = nameof(HPChanged))] private int hp = 100;
    [SyncVar(hook = nameof(AliveStateChanged))] private bool isAlive = true;
    [SyncVar(hook = nameof(ScoreChanged))] public int score = 0;

    public float moveSpeed = 3.0f;
    public float rotSpeed = 4.0f;
    public GameObject sprite;
    public Transform cameraTargetPoint;
    public HPProgressBar hpProgressBar;
    public int respawnTime = 5;

    private Rigidbody2D rb;
    private Transform trans;
    private CircleCollider2D coll;
    private bool isInDangerZone = false;
    [SyncVar] public int Id = -1;

    public static PlayerCntrl LocalPlayer;

    private void Start()
    {
        trans = transform;
        coll = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (isLocalPlayer)
        {
            LocalPlayer = this;
            SetNicknameCmd((GameData.nickname == string.Empty ? "UNKNOWN" : GameData.nickname));
            Camera.main.gameObject.GetComponent<CameraCntrl>().SetTarget(cameraTargetPoint);
            Camera.main.gameObject.GetComponent<CameraCntrl>().FocusCam();
            MinimapCntrl.Instance.SetTarget(trans);
        }
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer) GameManager.Instance.RemovePlayer(Id);
    }

    public Vector3 GetMoveVector()
    {
        return trans.TransformDirection(Vector2.up) * moveSpeed;
    }
    public string GetNickname()
    {
        return nickname;
    }

    private void Update()
    {
        if (!isLocalPlayer || !isAlive) return;
        rb.angularVelocity = -Input.GetAxis("Horizontal") * rotSpeed;
        rb.velocity = GetMoveVector();
    }

    public void SetDangerZoneState(bool state)
    {
        isInDangerZone = state;
        if (isLocalPlayer) UIManager.Instance.SetDangerZoneState(state);
        if (isServer) StartCoroutine(nameof(SelfExplodeTimer));
    }

    private IEnumerator SelfExplodeTimer()
    {
        yield return new WaitForSeconds(5f);
        if (isInDangerZone && isAlive)
        {
            TakeDamage(100, string.Empty);
            if (!isAlive) LogCntrl.Instance.ShowText(nickname + " взорвался в опасной зоне");
        }
    }

    [Server]
    public void Respawn()
    {
        hp = 100;
        isAlive = true;
        RespawnRpc(GameManager.Instance.GetRandomSpawnPoint());
    }

    [ClientRpc]
    private void RespawnRpc(Vector3 pos)
    {
        if (isLocalPlayer)
        {
            trans.position = pos;
            UIManager.Instance.SetRespawnPanelState(false);
            Camera.main.gameObject.GetComponent<CameraCntrl>().FocusCam();
        }
    }

    [Server]
    public void TakeDamage(int damage, string damageNickname)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        if (hp == 0)
        {
            isAlive = false;
            if (damageNickname != string.Empty) LogCntrl.Instance.ShowText(damageNickname + " взорвал " + nickname);
            score -= 20;
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    private IEnumerator RespawnTimerClient()
    {
        UIManager.Instance.SetRespawnPanelState(true);
        for (int i = respawnTime; i > 0; i--)
        {
            UIManager.Instance.SetRespawnTime(i);
            yield return new WaitForSeconds(1f);
        }
    }

    [Command]
    private void SetNicknameCmd(string nickname)
    {
        this.nickname = nickname;
        GameManager.Instance.AddPlayer(this);
    }

    private void HPChanged(int prev, int now)
    {
        hpProgressBar.SetProgress(now);
    }

    private void AliveStateChanged(bool prev, bool now)
    {
        sprite.SetActive(now);
        coll.enabled = now;
        nicknameTxt.gameObject.SetActive(now);
        hpProgressBar.gameObject.SetActive(now);

        if (!now && isLocalPlayer)
        {
            rb.angularVelocity = 0f;
            rb.velocity = Vector2.zero;
            StartCoroutine(nameof(RespawnTimerClient));
        }
    }

    private void ScoreChanged(int prev, int now)
    {
        if (isLocalPlayer) UIManager.Instance.UpdateScoreTxt(now);
    }

    private void NicknameChanged(string prev, string now)
    {
        nicknameTxt.text = now;
        if (!isServer) GameManager.Instance.AddPlayer(this);
    }
}
