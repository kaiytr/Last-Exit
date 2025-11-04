using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");


        moveDirection = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}
