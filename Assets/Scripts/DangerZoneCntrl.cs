using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZoneCntrl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCntrl>().SetDangerZoneState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCntrl>().SetDangerZoneState(false);
        }
    }
}
