using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerCntrl : NetworkBehaviour
{
    public TMP_Text nicknameTxt;
    [SyncVar(hook = nameof(NicknameChanged))] private string nickname = string.Empty;
    [SyncVar(hook = nameof(HPChanged))] private int hp = 100;

    public float moveSpeed = 3.0f;
    public float rotSpeed = 4.0f;
    public Transform cameraTargetPoint;
    public HPProgressBar hpProgressBar;

    private Rigidbody2D rb;
    private Transform trans;

    public static PlayerCntrl LocalPlayer;

    private void Start()
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();
        if (isLocalPlayer)
        {
            LocalPlayer = this;
            SetNicknameCmd(GameData.nickname);
            Camera.main.gameObject.GetComponent<CameraCntrl>().SetTarget(cameraTargetPoint);
        }
    }

    public Vector3 GetMoveVector()
    {
        return trans.TransformDirection(Vector2.up) * moveSpeed;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - trans.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
        trans.rotation = targetRot;

        rb.velocity = GetMoveVector();
    }

    [Command]
    private void SetNicknameCmd(string nickname)
    {
        this.nickname = nickname;
    }

    private void HPChanged(int prev, int now)
    {
        hpProgressBar.SetProgress(now);
    }

    private void NicknameChanged(string prev, string now)
    {
        nicknameTxt.text = now;
    }
}
