using UnityEngine;

public class PlayerCommon : MonoBehaviour
{
    public static Rigidbody2D rb;

    public static float dirXR;
    public static float dirYR;

    public static bool isNotGrounded = false;
    public static bool isWallJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        dirXR = Input.GetAxisRaw("Horizontal");
        dirYR = Input.GetAxisRaw("Vertical");
    }   
}
