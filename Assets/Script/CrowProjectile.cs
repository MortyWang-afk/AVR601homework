using UnityEngine;

public class CrowProjectile : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;
    public float lifeTime = 4f;

    Vector2 direction;

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle); // 让子丨弹朝向飞行方向
        Destroy(gameObject, lifeTime); // 飞出屏幕也能自动清理
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<DogController>(out var dog))
        {
            dog.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.TryGetComponent<Ball>(out var ball))
        {
            Destroy(ball.gameObject);   // 球也一起没
            Destroy(gameObject);
        }

         else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}