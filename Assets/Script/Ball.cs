using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        // 飞出屏幕右边就自我销毁,不然会越积越多
        if (transform.position.x > 12f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Crow"))
    {
        Destroy(other.gameObject); // 销毁乌鸦
        Destroy(gameObject);       // 销毁球自己
    }
}
}
