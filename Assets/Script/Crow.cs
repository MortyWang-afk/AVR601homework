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
    string myTargetTag = "Bowl";
    public float bowlSpeedMultiplier = 1.5f;    // 偷食物的乌鸦速度倍率

    // ===== 发射子弹 =====
    public GameObject projectilePrefab;   // 拖 CrowProjectile 的 prefab 进来
    public float fireInterval = 2.5f;     // 每隔几秒发一发
    public float fireRange = 8f;          // 离小狗多近才开火
    public int projectileDamage = 1;      // 一发扣多少血
    public float spawnOffset = 0.5f;      // 生成点往小狗方向挪一点，别卡在自己身上
    public Transform firePoint;           // 乌鸦嘴的位置

    float nextFireTime;
    Transform dog;

    // Spawner 在生成时调用，把这一波的配置传进来
    public void Setup(float speed, string targetTag)
    {
        this.speed = speed;
        this.myTargetTag = targetTag;
    }

    void Start()
    {
        health = maxHealth;
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        // 偷食物的鸟飞得更快
        if (myTargetTag == "Bowl")
            speed *= bowlSpeedMultiplier;

        // 飞行目标：食盆 或 小狗，看这一波的配置
        GameObject go = GameObject.FindWithTag(myTargetTag);
        if (go != null)
            target = go.transform;

        // 射击目标：永远是小狗
        GameObject dogGo = GameObject.FindWithTag("Dog");
        if (dogGo != null)
            dog = dogGo.transform;

        // 错开首发时间，不然一波乌鸦会齐射
        nextFireTime = Time.time + Random.Range(0.5f, fireInterval);
    }

    void Update()
    {
        if (fleeing)
        {
            // 得手后朝右上方溜走
            transform.position += new Vector3(1f, 0.7f, 0f).normalized * fleeSpeed * Time.deltaTime;

            if (transform.position.x > 12f || transform.position.y > 7f)
            {
                SFXManager.Instance.PlayVaried(SFXManager.Instance.crowEscape, 0.5f);

                //逃出屏幕 = 逃走，上报 Spawner 补一只，不算战果
                Spawner spawner = FindFirstObjectByType<Spawner>();
                if (spawner != null)
                    spawner.CrowEscaped();

                Destroy(gameObject);
            }
        }
        else
        {
            if (target == null) return;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime );

            if (myTargetTag == "Bowl") return;
                TryShoot(); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (fleeing) return;

        if (other.CompareTag("Bowl"))
        {
            SFXManager.Instance.Play(SFXManager.Instance.bowlHit); 

            GameManager.Instance.DamageFood(1);
            other.GetComponent<HitFlash>().Flash(); // 食盆红闪
            StartFleeing();
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (health <= 0)
        {
            SFXManager.Instance.PlayVaried(SFXManager.Instance.crowDeath);  // ← 死亡音

            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            //被打死 = 真战果，上报 Spawner 记数
            Spawner spawner = FindFirstObjectByType<Spawner>();
            if (spawner != null)
                spawner.CrowKilled();

            Destroy(gameObject);
        }

            else
        {
            SFXManager.Instance.PlayVaried(SFXManager.Instance.crowHit, 0.7f);  // ← 受击音
        }
    }

    void TryShoot()
    {
        if (projectilePrefab == null || dog == null) return;
        if (Time.time < nextFireTime) return;

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 toDog = (Vector2)dog.position - (Vector2)origin;
        if (toDog.magnitude > fireRange) return;      // 太远不开火

        nextFireTime = Time.time + fireInterval;
        GameObject p = Instantiate(projectilePrefab, origin, Quaternion.identity);

        if (p.TryGetComponent<CrowProjectile>(out var proj))
        {
         proj.damage = projectileDamage;
         proj.Launch(toDog);
        }

        SFXManager.Instance.PlayVaried(SFXManager.Instance.crowShoot, 0.6f); 
    }

    public void SwitchToBowl()
    {
        if (myTargetTag == "Bowl") return;      // 本来就是冲食盆的，不用改

        myTargetTag = "Bowl";
        speed *= bowlSpeedMultiplier;           // 换目标后也加速

        GameObject go = GameObject.FindWithTag("Bowl");

        if (go != null)
            target = go.transform;
    }

    public void StartFleeing() // public:狗的脚本要调用它
    {
        fleeing = true;
    }
}