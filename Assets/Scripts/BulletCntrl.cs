using UnityEngine;
using Mirror;

public class BulletCntrl : NetworkBehaviour
{
    public float moveSpeed = 6.0f;
    private Transform trans;

    private PlayerCntrl owner;

    private void Start()
    {
        trans = transform;
    }

    private void Update()
    {
        if (!isServer) return;
        trans.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }

    public void Init(PlayerCntrl owner)
    {
        this.owner = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCntrl>().TakeDamage(10, owner.GetNickname(), string.Empty);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("DEATH_ZONE")) Destroy(gameObject);
    }
}
