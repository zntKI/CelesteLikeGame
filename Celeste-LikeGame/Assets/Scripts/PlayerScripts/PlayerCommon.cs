using UnityEngine;

public class PlayerCommon : MonoBehaviour
{
    public static Rigidbody2D rb;

    public static float dirXR;
    public static float dirYR;

    public static float gravityScale;

    public static bool isNotGrounded = false;
    public static bool isWallJumping = false;
    public static bool isDashingForMovementStop = false;
    public static bool isDashingForDuration = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScale = rb.gravityScale;
    }
    
    void Update()
    {
        dirXR = Input.GetAxisRaw("Horizontal");
        dirYR = Input.GetAxisRaw("Vertical");
    }   
}
