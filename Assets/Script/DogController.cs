using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform mouthPoint;
    public float mouthOffset = 1.63f;   // 吐球点距离狗中心的水平偏移,可在 Inspector 调
    public float moveSpeed = 5f;
    public float jumpImpulse = 9f;
    public int maxJumps = 3;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float hangThreshold = 0.5f;
    public float hangMultiplier = 0.6f;
    bool isHit;
    bool isInvincible = false; 
    public float invincibleTime = 3f;  

    public int ballDamage = 1;      // 狗的球打多少伤害

    // 吐球冷却
    public float shootCooldown = 1f;
    float nextShootTime = 0f;

    SpriteRenderer sr;                  // 改名,避免和基类的 renderer 冲突 (CS0108 警告)
    Rigidbody2D rb;
    SquashStretch squash;
    int jumpsLeft;
    float h;
    bool jumpQueued;
    bool jumpHeld;

    void Start()
    {
        isHit = false;
        rb = GetComponent<Rigidbody2D>();
        squash = GetComponent<SquashStretch>();
        sr = GetComponent<SpriteRenderer>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        if (h < 0)
            sr.flipX = true;
        if (h > 0)
            sr.flipX = false;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && jumpsLeft > 0)
            jumpQueued = true;

        jumpHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        // 吐球(带冷却)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + shootCooldown;

            float dir = sr.flipX ? -1f : 1f;
            Vector3 spawnPos = mouthPoint.position + Vector3.right * (mouthOffset * dir);
            Quaternion rotation = Quaternion.Euler(0, sr.flipX ? 180 : 0, 0);

            GameObject ball = Instantiate(ballPrefab, spawnPos, rotation);
            if (ball.TryGetComponent<Ball>(out var b))
                b.damage = ballDamage;

            SFXManager.Instance.PlayVaried(SFXManager.Instance.playerShoot); 
        }
    }

    void FixedUpdate()
    {
        if(!isHit)
            rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);

        if (jumpQueued)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            jumpsLeft--;
            jumpQueued = false;

            if (squash != null && squash.enabled)
                squash.JumpStretch();
            
            float pitch = 1f + (maxJumps - jumpsLeft) * 0.12f;   // 注意这时 jumpsLeft 已经减过了
            SFXManager.Instance.Play(SFXManager.Instance.jump, 0.3f, pitch); 

        }

        // 可变重力:悬停判断放最前,让顶点两侧都有漂浮感
        float vy = rb.linearVelocity.y;
        if (Mathf.Abs(vy) < hangThreshold)
            rb.gravityScale = hangMultiplier;
        else if (vy < 0)
            rb.gravityScale = fallMultiplier;
        else if (vy > 0 && !jumpHeld)
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 1f;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 只有从上方落到地面时才重置跳跃次数(法线朝上),侧面撞墙不算
        if (col.gameObject.CompareTag("Ground") && col.contacts[0].normal.y > 0.5f)
        {
            jumpsLeft = maxJumps;

            if (squash != null && squash.enabled)
                squash.LandSquash();
            
            SFXManager.Instance.PlayVaried(SFXManager.Instance.land, 0.2f); 
        }
    }
    void BackOn()=> isHit = false;

    public void StartInvincible()
    {
        StartCoroutine(InvincibleRoutine());
    }

    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

         // 无敌期间暂停生成新乌鸦
        Spawner spawner = FindAnyObjectByType<Spawner>();      
        if (spawner != null)                                    
            spawner.spawnPaused = true;                        


        // 闪烁:每 0.1 秒切换一次显示
        float timer = 0f;
        while (timer < invincibleTime)
        {
            sr.enabled = !sr.enabled;       // 取反:显示变隐藏,隐藏变显示
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        sr.enabled = true;      // 结束时确保是显示状态
        isInvincible = false;

        if (spawner != null)              
            spawner.spawnPaused = false; 
    }

    public void TakeDamage(int amount)
{
    if (isInvincible) return;

    SFXManager.Instance.Play(SFXManager.Instance.playerHurt); 

    GameManager.Instance.DamageDog(amount);
    rb.AddForce(Vector2.left * 5f, ForceMode2D.Impulse);
    isHit = true;
    Invoke("BackOn", 0.5f);
    StartInvincible();

    if (TryGetComponent<HitFlash>(out var flash))
        flash.Flash();
}

   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crow"))
    {
        if (other.TryGetComponent<Crow>(out var crow))
            crow.SwitchToBowl();

        TakeDamage(1);
    }
    }
}