using System.Collections.Generic;
using UnityEngine;

public class EnemyMark
{
    public Transform enemyTrans;
    public Transform enemyMarkTrans;
}

public class MinimapCntrl : MonoBehaviour
{
    public RectTransform minimapMark;
    public float scaleRatio = 1f;
    public GameObject enemyMarkPrefab;
    public Transform markParent;

    public static MinimapCntrl Instance;
    private Transform target;

    private List<EnemyMark> enemies = new List<EnemyMark>();

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

    public void AddEnemy(Transform enemy)
    {
        EnemyMark enemyMark = new EnemyMark();
        enemyMark.enemyTrans = enemy;
        GameObject mark = Instantiate(enemyMarkPrefab);
        mark.transform.SetParent(markParent);
        enemyMark.enemyMarkTrans = mark.transform;
        enemies.Add(enemyMark);
    }

    public void RemoveEnemy(Transform enemy)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyTrans == enemy)
            {
                Destroy(enemies[i].enemyMarkTrans.gameObject);
                enemies.RemoveAt(i);
                return;
            }
        }
    }

    private void Update()
    {
        if (PlayerCntrl.LocalPlayer != null)
        {
            minimapMark.localPosition = new Vector3(target.position.x * scaleRatio, target.position.y * scaleRatio, 0f);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].enemyMarkTrans.localPosition = new Vector3(enemies[i].enemyTrans.position.x * scaleRatio, enemies[i].enemyTrans.position.y * scaleRatio, 0f);
        }
    }
}
