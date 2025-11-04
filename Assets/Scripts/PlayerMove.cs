using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    public Animator animator;
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x, y).normalized;

        animator.SetInteger("X", (int)x);
        animator.SetInteger("Y", (int)y);

        if (x != 0)
        {
            transform.localScale = new Vector3(-x, 1, 1);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}
