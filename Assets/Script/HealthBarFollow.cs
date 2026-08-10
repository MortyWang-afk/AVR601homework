using UnityEngine;

[ExecuteAlways]  
public class HealthBarFollow : MonoBehaviour
{
    public Transform target;        // 要跟随的对象(狗或食物)
    public Vector3 offset = new Vector3(0, 1f, 0); // 头顶偏移

    void LateUpdate()
    {
        if (target == null)
        {
            gameObject.SetActive(false); // 对象没了就隐藏血条
            return;
        }
        // 世界坐标 → 屏幕坐标
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }
}