using UnityEngine;

public class DogController : MonoBehaviour
{
   public GameObject ballPrefab;
    public Transform mouthPoint;
    public float moveSpeed = 5f;
    public float jumpForce = 9f;
    public int maxJumps = 3;      // 2 = 二段跳,想三段跳改 3
    public float fallMultiplier = 2.5f; // 下落时的额外重力倍率


    Rigidbody2D rb;
    int jumpsLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
        GetComponent<SquashStretch>().JumpStretch();
    }

    void Update()
    {
        // —— 左右移动:A/D 或 ←/→ ——
        float h = Input.GetAxisRaw("Horizontal"); // -1、0、1
        rb.linearVelocity = new Vector2(h * moveSpeed, rb.linearVelocity.y);
        //                                    ↑ 只改x        ↑ y保持原样,交给重力/跳跃

        // —— 跳跃:还有次数就能跳 ——
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //     二段跳时这样直接设 y,比叠加力手感干脆 ↑
            jumpsLeft--;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Instantiate(ballPrefab, mouthPoint.position, Quaternion.identity);
        
        // 下落阶段加重力,让弧线"脆"起来
        if (rb.linearVelocity.y < 0)
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }

    // 实体碰撞:落回地面
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            jumpsLeft = maxJumps; // 落地,次数重置
            GetComponent<SquashStretch>().LandSquash();
    }

    // 触发碰撞:被乌鸦碰到 —— 老师要的就是这段
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crow"))
        {
            GameManager.Instance.DamageDog(1);
            GetComponent<HitFlash>().Flash();
            other.GetComponent<Crow>().StartFleeing();
        }
    }
}
