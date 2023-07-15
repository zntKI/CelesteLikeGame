using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    //TODO: Make the movement less sloppy overall, more responsive; Fix jump height

    //private Rigidbody2D rb;

    //private float dirXR;

    [Header("Movement")]

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float decceleration = 7f;

    [Space(5)]
    [SerializeField] private float frictionAmount = 0.22f;

    [Space(10)]
    [Header("Jump")]

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    //private bool isGrounded = false;
    private bool canJumpAgain = true; /*if the player tries hold down the jump button and bunnyhop*/
    private float lastTimeOnGround = 0f;


    // Start is called before the first frame update
    void Start()
    {
        PlayerCommon.rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //dirXR = Input.GetAxisRaw("Horizontal");

        #region Jump

        JumpHeightController();

        if (PlayerCommon.isNotGrounded)
            lastTimeOnGround += Time.deltaTime;

        //Normal jump
        if (Input.GetButton("Jump") && !PlayerCommon.isNotGrounded && canJumpAgain)
            Jump();
        //Coyote time
        if (PlayerCommon.isNotGrounded && lastTimeOnGround <= jumpBufferTime && Input.GetButton("Jump") && canJumpAgain)
            Jump();

        if (Input.GetButtonUp("Jump"))
            canJumpAgain = true;

        #endregion
    }

    private void FixedUpdate()
    {
        #region Run

        if (!PlayerCommon.isWallJumping && !PlayerCommon.isDashingForMovementStop)
        {
            float targetSpeed = PlayerCommon.dirXR * moveSpeed;
            float speedDif = targetSpeed - PlayerCommon.rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            float movement = Mathf.Abs(speedDif) * accelRate * Mathf.Sign(speedDif);

            //Debug.Log($"TargetSpeed: {targetSpeed}; SpeedDif: {speedDif:f2}; AccelRate: {accelRate}; Movement: {movement:f2}; Vel: {rb.velocity.x}");

            PlayerCommon.rb.AddForce(movement * Vector2.right);
        }

        #endregion


        #region Friction

        if (PlayerCommon.isNotGrounded == false && PlayerCommon.dirXR == 0)
        {
            float amount = Mathf.Min(Mathf.Abs(PlayerCommon.rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(PlayerCommon.rb.velocity.x);
            PlayerCommon.rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion
    }

    private void Jump()
    {
        //tempDirXForJump = Mathf.Clamp(dirX, -dirXConstraintForJumpHeight, dirXConstraintForJumpHeight);
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce/* - Mathf.Abs(jumpForce * tempDirXForJump)*/);
        PlayerCommon.rb.velocity = new Vector2(PlayerCommon.rb.velocity.x, jumpForce);
    }
    private void JumpHeightController()
    {
        if (PlayerCommon.rb.velocity.y < 0)
            PlayerCommon.rb.velocity += (fallMultiplier - 1) * PlayerCommon.rb.gravityScale * Time.deltaTime * Vector2.down;
        else if (PlayerCommon.rb.velocity.y > 0 && !Input.GetButton("Jump"))
            PlayerCommon.rb.velocity += (lowJumpMultiplier - 1) * PlayerCommon.rb.gravityScale * Time.deltaTime * Vector2.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlayerCommon.isNotGrounded = false;
            lastTimeOnGround = 0f;
            if (Input.GetButton("Jump"))
                canJumpAgain = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlayerCommon.isNotGrounded = true;
        }
    }
}
