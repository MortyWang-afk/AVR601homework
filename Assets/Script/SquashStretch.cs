using UnityEngine;
using System.Collections;

public class SquashStretch : MonoBehaviour
{
    public float duration = 0.12f;       // 单程时长
    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    public void JumpStretch()  // 起跳:瘦高
    {
        Play(new Vector3(0.8f, 1.25f, 1f));
    }

    public void LandSquash()   // 落地:矮胖
    {
        Play(new Vector3(1.25f, 0.75f, 1f));
    }

    void Play(Vector3 targetMul)
    {
        StopAllCoroutines();
        transform.localScale = baseScale;
        StartCoroutine(DoSquash(targetMul));
    }

    IEnumerator DoSquash(Vector3 mul)
    {
        Vector3 target = Vector3.Scale(baseScale, mul);
        float t = 0;

        // 去:快速到达变形状态
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale, target, t / duration);
            yield return null;
        }
        // 回:弹回原形
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(target, baseScale, t / duration);
            yield return null;
        }
        transform.localScale = baseScale;
    }
}