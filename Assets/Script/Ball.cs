using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;   

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        // 飞出屏幕右边就自我销毁,不然会越积越多
        if (transform.position.x > 12f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Crow"))
    {    
        Crow crow = other.GetComponent<Crow>();   
            if (crow != null)                          
                crow.TakeDamage(damage);              

        Destroy(gameObject);       // 销毁球自己
    }
}
}
