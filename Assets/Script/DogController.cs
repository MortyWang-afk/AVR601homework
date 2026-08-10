using UnityEngine;

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
        }
    }
    void BackOn()=> isHit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crow"))
        {
            GameManager.Instance.DamageDog(1);
            rb.AddForce (Vector2.left*5f, ForceMode2D.Impulse);
            isHit = true;
            Invoke("BackOn",1f);

            if (TryGetComponent<HitFlash>(out var flash))
                flash.Flash();

            if (other.TryGetComponent<Crow>(out var crow))
                crow.StartFleeing();
        }
    }
}