using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    private Rigidbody2D rb;

    private float dirXR;

    [Header("Movement")]

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float decceleration = 7f;

    [Space(5)]
    [SerializeField] private float frictionAmount = 0.22f;

    [Space(10)]
    [Header("Jump")]

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private bool isJumping = false;
    private bool canJumpAgain = true; /*if the player tries hold down the jump button and bunnyhop*/
    private float lastTimeOnGround = 0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        dirXR = Input.GetAxisRaw("Horizontal");

        #region Jump

        JumpHeightController();

        if (isJumping)
            lastTimeOnGround += Time.deltaTime;

        //Normal jump
        if (Input.GetButton("Jump") && !isJumping && canJumpAgain)
            Jump();
        //Coyote time
        if (isJumping && lastTimeOnGround <= jumpBufferTime && Input.GetButton("Jump") && canJumpAgain)
            Jump();

        if (Input.GetButtonUp("Jump"))
            canJumpAgain = true;

        #endregion
    }

    private void FixedUpdate()
    {
        #region Run

        if (!PlayerCommon.isWallJumping)
        {
            float targetSpeed = dirXR * moveSpeed;
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            float movement = Mathf.Abs(speedDif) * accelRate * Mathf.Sign(speedDif);

            //Debug.Log($"TargetSpeed: {targetSpeed}; SpeedDif: {speedDif:f2}; AccelRate: {accelRate}; Movement: {movement:f2}; Vel: {rb.velocity.x}");

            rb.AddForce(movement * Vector2.right);
        }

        #endregion


        #region Friction

        if (isJumping == false && dirXR == 0)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion
    }

    private void Jump()
    {
        //tempDirXForJump = Mathf.Clamp(dirX, -dirXConstraintForJumpHeight, dirXConstraintForJumpHeight);
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce/* - Mathf.Abs(jumpForce * tempDirXForJump)*/);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void JumpHeightController()
    {
        if (rb.velocity.y < 0)
            rb.velocity += (fallMultiplier - 1) * rb.gravityScale * Time.deltaTime * Vector2.down;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += (lowJumpMultiplier - 1) * rb.gravityScale * Time.deltaTime * Vector2.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
            lastTimeOnGround = 0f;
            if (Input.GetButton("Jump"))
                canJumpAgain = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = true;
        }
    }
}
