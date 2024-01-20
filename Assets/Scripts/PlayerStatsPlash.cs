using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsPlash : MonoBehaviour
{
    private Transform trans;

    private void Start()
    {
        trans = transform;
    }

    private void Update()
    {
        trans.rotation = Quaternion.identity;
    }
}
