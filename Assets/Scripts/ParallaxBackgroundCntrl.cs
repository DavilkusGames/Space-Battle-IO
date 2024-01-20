using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackgroundCntrl : MonoBehaviour
{
    [SerializeField] private Image[] backgroundLayers;
    public Vector2[] layerSpeeds;
    public float moveScrollK = 1f;

    private List<Vector2> offsets;

    void Awake()
    {
        offsets = new List<Vector2>();
        for (int i = 0; i < backgroundLayers.Length; i++) offsets.Add(new Vector2(0, 0));
    }

    void Update()
    {
        for (int i = 0; i < backgroundLayers.Length; i++) {
            Vector2 scrollVector = new Vector2(1f, 0f);
            if (PlayerCntrl.LocalPlayer != null)
            {
                scrollVector = PlayerCntrl.LocalPlayer.GetMoveVector().normalized * moveScrollK;
            }
            offsets[i] += Vector2.right * layerSpeeds[i].x * -scrollVector.x * Time.deltaTime;
            offsets[i] += Vector2.up * layerSpeeds[i].y * -scrollVector.y * Time.deltaTime;
            backgroundLayers[i].material.mainTextureOffset = offsets[i];
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            backgroundLayers[i].material.mainTextureOffset = new Vector2(0, 0);
        }
    }
}
