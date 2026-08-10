using UnityEngine;

public class Crow : MonoBehaviour
{
    public float speed = 2f;
    public float fleeSpeed = 3.5f;

    public int maxHealth = 2;              
    public BirdhealthBar healthBar;  
    public GameObject deathEffect;       
    int health;                            

    Transform target;
    bool fleeing = false;

    void Start()
    {
        health = maxHealth;                              
        if (healthBar != null)                           
            healthBar.SetHealth(health, maxHealth);     

        // 抽签:30% 概率盯上狗,否则盯食盆
    string targetTag = (Random.value < 0.3f) ? "Dog" : "Bowl";

    GameObject go = GameObject.FindWithTag(targetTag);
    if (go != null)
        target = go.transform;
    }

    void Update()
    {
        if (fleeing)
        {
            // 得手后朝右上方溜走
            transform.position += new Vector3(1f, 0.7f, 0f).normalized
                                  * fleeSpeed * Time.deltaTime;
            if (transform.position.x > 12f || transform.position.y > 7f)
                Destroy(gameObject);
        }
        else
        {
            if (target == null) return;
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (fleeing) return;

        if (other.CompareTag("Bowl"))
        {
            GameManager.Instance.DamageFood(1);
            other.GetComponent<HitFlash>().Flash(); // 食盆红闪
            StartFleeing();
        }
    }
    public void TakeDamage(int amount)
    {
        if (fleeing) return;

        health -= amount;

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (health <= 0)
        {
            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    public void StartFleeing() // public:狗的脚本要调用它
    {
        fleeing = true;
        GetComponent<Collider2D>().enabled = false; // 逃跑路上不再触发碰撞
    }

    void OnDestroy()
{
    // 找到Spawner上报(场景卸载时Spawner可能已经没了,要判空)
    Spawner spawner = FindFirstObjectByType<Spawner>();
    if (spawner != null)
        spawner.CrowRemoved();
}
}
