using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1; 
    public float range = 6f;        // 射程  
    Vector3 startPos;               // 记住出生位置

    void Start()                    // 新增
    {
        startPos = transform.position;
    }



   void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        // 飞过射程就销毁
        if (Vector3.Distance(startPos, transform.position) > range)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("球碰到了: " + other.name + " / Tag: " + other.tag); 
     
    if (other.CompareTag("Crow"))
    {    
        Crow crow = other.GetComponent<Crow>();   
            if (crow != null)                          
                crow.TakeDamage(damage);              

        Destroy(gameObject);       // 销毁球自己
    }
}
}
