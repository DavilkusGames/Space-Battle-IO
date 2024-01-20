using System.Collections.Generic;
using UnityEngine;

public class MinimapCntrl : MonoBehaviour
{
    public RectTransform minimapMark;
    public float scaleRatio = 1f;
    public GameObject enemyMarkPrefab;
    public Transform markParent;

    public static MinimapCntrl Instance;
    private Transform target;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (PlayerCntrl.LocalPlayer != null)
        {
            minimapMark.localPosition = new Vector3(target.position.x * scaleRatio, target.position.y * scaleRatio, 0f);
        }
    }
}
