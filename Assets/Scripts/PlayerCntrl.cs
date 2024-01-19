using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerCntrl : NetworkBehaviour
{
    public TMP_Text nicknameTxt;
    [SyncVar(hook = nameof(NicknameChanged))] private string nickname = string.Empty;
    public float moveSpeed = 3.0f;

    private Rigidbody2D rb;
    private Transform trans;
    private Vector2 velocity;

    private void Start()
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();
        if (isLocalPlayer)
        {
            SetNicknameCmd(GameData.nickname);
            Camera.main.gameObject.GetComponent<CameraCntrl>().SetTarget(trans);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = velocity * moveSpeed;
    }

    [Command]
    private void SetNicknameCmd(string nickname)
    {
        this.nickname = nickname;
    }

    private void NicknameChanged(string prev, string now)
    {
        nicknameTxt.text = now;
    }
}
