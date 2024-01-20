using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCntrl : MonoBehaviour
{
    public float lerpSpeed = 1.0f;

    private Transform trans;
    private Transform target = null;

    private void Start()
    {
        trans = transform;
    }

    public void SetTarget(Transform trans)
    {
        target = trans;
    }

    private void FixedUpdate()
    {
        if (target != null) trans.position = Vector3.Lerp(trans.position, 
            new Vector3(target.position.x, target.position.y, trans.position.z), 
            lerpSpeed * Time.fixedDeltaTime);
    }
}
