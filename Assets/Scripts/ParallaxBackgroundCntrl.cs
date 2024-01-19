using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackgroundCntrl : MonoBehaviour
{
    [SerializeField] private Image[] backgroundLayers;
    public float[] layerSpeeds;

    private List<Vector2> offsets;

    void Awake()
    {
        offsets = new List<Vector2>();
        for (int i = 0; i < backgroundLayers.Length; i++) offsets.Add(new Vector2(0, 0));
    }

    void Update()
    {
        for (int i = 0; i < backgroundLayers.Length; i++) {
            offsets[i] += Vector2.right * layerSpeeds[i] * Time.deltaTime;
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
