using UnityEngine;

public class BirdhealthBar : MonoBehaviour
{
    float fullWidth;

    void Awake()
    {
        fullWidth = transform.localScale.x;
    }

    public void SetHealth(int current, int max)
    {
        float pct = (float)current / max;
        if (pct < 0f) pct = 0f;

        Vector3 s = transform.localScale;
        s.x = fullWidth * pct;
        transform.localScale = s;
    }
}