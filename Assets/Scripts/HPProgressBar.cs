using UnityEngine;

public class HPProgressBar : MonoBehaviour
{
    public float minValue = 0f;
    public float maxValue = 100f;
    public Transform progressBarFill;

    public float tmp;

    private float currentValue = 100f;

    public void SetProgress(float value)
    {
        currentValue = Mathf.Clamp(value, minValue, maxValue);
        float scale = currentValue / maxValue;
        float positionOffset = (1f - scale) / 2f;

        progressBarFill.transform.localScale = new Vector3(scale, 1f, 1f);
        progressBarFill.transform.position = transform.position + new Vector3(positionOffset, 0f, 0f);
    }
}
