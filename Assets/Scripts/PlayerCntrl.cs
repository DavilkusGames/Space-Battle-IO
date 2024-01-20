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
    [SyncVar] public int score = 0;

    public float moveSpeed = 3.0f;
    public float rotSpeed = 4.0f;
    public Transform cameraTargetPoint;
    public HPProgressBar hpProgressBar;

    private Rigidbody2D rb;
    private Transform trans;
    private bool isInDangerZone = false;
    [SyncVar] public int Id = -1;

    public static PlayerCntrl LocalPlayer;

    private void Start()
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();

        if (isLocalPlayer)
        {
            LocalPlayer = this;
            SetNicknameCmd((GameData.nickname == string.Empty ? "UNKNOWN" : GameData.nickname));
            Camera.main.gameObject.GetComponent<CameraCntrl>().SetTarget(cameraTargetPoint);
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
        if (!isLocalPlayer) return;
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
            TakeDamage(100);
        }
    }

    [Server]
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        if (hp == 0) isAlive = false;
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

    }

    private void NicknameChanged(string prev, string now)
    {
        nicknameTxt.text = now;
        if (!isServer) GameManager.Instance.AddPlayer(this);
    }
}
