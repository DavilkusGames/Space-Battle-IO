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
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public Transform cameraTargetPoint;
    public Transform bulletSpawn;
    public HPProgressBar hpProgressBar;
    public int respawnTime = 5;
    public float shootDelay = 0.2f;

    private Rigidbody2D rb;
    private Transform trans;
    private CircleCollider2D coll;
    private CircleCollider2D trigger;
    private bool isInDangerZone = false;
    private float nextShootTime = 0f;
    private bool isShooting = false;
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
        if (!isAlive) return;
        if (isServer && isShooting && Time.time >= nextShootTime)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletSpawn.position;
            bullet.transform.rotation = trans.rotation;
            bullet.GetComponent<BulletCntrl>().Init(this);
            NetworkServer.Spawn(bullet);
            nextShootTime = Time.time + shootDelay;
        }

        if (!isLocalPlayer) return;
        rb.angularVelocity = -Input.GetAxis("Horizontal") * rotSpeed;
        rb.velocity = GetMoveVector();

        if (Input.GetMouseButtonDown(0)) ShootStateCmd(true);
        if (Input.GetMouseButtonUp(0)) ShootStateCmd(false);
    }

    [Command]
    private void ShootStateCmd(bool isShooting)
    {
        this.isShooting = isShooting;
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
            TakeDamage(100, string.Empty, string.Empty);
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
    public void TakeDamage(int damage, string damageNickname, string weapon)
    {
        if (!isAlive) return;
        hp -= damage;
        if (hp < 0) hp = 0;
        if (hp == 0)
        {
            isAlive = false;
            if (damageNickname != string.Empty) LogCntrl.Instance.ShowText(damageNickname + " взорвал " + nickname + weapon);
            score -= 20;
            Invoke(nameof(Respawn), respawnTime);

            GameObject exp = Instantiate(explosionPrefab, trans.position, Quaternion.identity);
            NetworkServer.Spawn(exp);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (other.CompareTag("Player"))
        {
            PlayerCntrl otherPlayer = other.gameObject.GetComponent<PlayerCntrl>();
            TakeDamage(30, otherPlayer.nickname, " тараном");
            otherPlayer.TakeDamage(30, nickname, " тараном");
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
        trigger.enabled = now;
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
