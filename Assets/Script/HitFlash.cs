using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    public Color flashColor = Color.red;
    public float flashTime = 0.1f;
    public int flashCount = 2;

    SpriteRenderer sr;
    Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        sr.color = originalColor;
        StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        for (int i = 0; i < flashCount; i++)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashTime);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashTime);
        }
    }
}
