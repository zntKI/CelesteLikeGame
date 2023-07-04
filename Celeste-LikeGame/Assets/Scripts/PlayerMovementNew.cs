using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;

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
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private bool isJumping = false;
    private bool canJumpAgain = true; /*if the player tries hold down the jump button and bunnyhop*/
    private float lastTimeOnGround = 0f;

    [Space(10)]
    [Header("Wall Interaction")]

    [SerializeField] private float wallSlidingSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        coll = gameObject.GetComponent<CapsuleCollider2D>();
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

        #region Wall Movement

        //if (IsStickingToWall())
        //{
        //    WallSlide();
        //}

        #endregion
    }

    private void FixedUpdate()
    {
        #region Run

        float targetSpeed = dirXR * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Abs(speedDif) * accelRate * Mathf.Sign(speedDif);

        //Debug.Log($"TargetSpeed: {targetSpeed}; SpeedDif: {speedDif:f2}; AccelRate: {accelRate}; Movement: {movement:f2}; Vel: {rb.velocity.x}");

        rb.AddForce(movement * Vector2.right);

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

    private void WallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
    }

    private bool IsStickingToWall()
    {
        var stickFromRight = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
        var stickFromLeft = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround);

        //wallIsRight = stickFromRight ? true : false;

        return (stickFromRight || stickFromLeft);

        //if (stickFromRight || stickFromLeft)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
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
