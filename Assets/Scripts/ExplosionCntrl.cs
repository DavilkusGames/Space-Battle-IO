using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCntrl : MonoBehaviour
{
    public float deleteTime = 5f;

    private void Start()
    {
        Invoke(nameof(Remove), deleteTime);
    }

    private void Remove()
    {
        Destroy(gameObject);
    }
}
