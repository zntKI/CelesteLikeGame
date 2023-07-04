using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    private float dirY = 0f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float wallMoveSpeed = 3f;
    [SerializeField] private float jumpForce = 14f;
    //[SerializeField] private float dashPower = 32f;

    private bool canWallJump = true;
    private bool isWallJumping = false;
    private float wallSlidingSpeed = 4f;

    private bool isGrabbed = false;

    private bool wallIsRight = true;

    //private bool canDash = true;
    //private bool isDashing;
    //private float dashingTime = 0.2f;
    //private float dashingCooldown = 1f;
    //private float diagonalDashLengths = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        //diagonalDashLengths = MathF.Sqrt(dashPower / 2);
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");

        if (IsGrounded() || IsStickingToWall())
        {
            isWallJumping = false;
        }

        WallGrabing();
        if (isGrabbed)
        {
            return;
        }

        //Normal movements
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        //Normal jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce - Mathf.Abs(dirX));
        }

        //Checks
        //WallSliding();
        //WallJumping();
        if (IsStickingToWall())
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Debug.Log($"IN");
                //Vector2 wallDir = wallIsRight ? Vector2.left : Vector2.right;
                //rb.velocity = new Vector2(rb.velocity.x, 0);
                //rb.velocity += (Vector2.up / 1.5f + wallDir / 1.5f) * jumpForce;
                transform.Translate(new Vector2(-2.5f, 4f));

                //rb.velocity = new Vector2(rb.velocity.x, jumpForce - Mathf.Abs(dirX));
                canWallJump = false;
                isWallJumping = true;
            }
        }
        else
        {
            canWallJump = true;
        }
    }

    private void WallGrabing()
    {
        if (Input.GetKey(KeyCode.LeftControl) && IsStickingToWall() && !isWallJumping)
        {
            rb.bodyType = RigidbodyType2D.Static;
            isGrabbed = true;
        }
        if (!Input.GetKey(KeyCode.LeftControl) || dirY != 0 || Input.GetButtonDown("Jump"))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isGrabbed = false;
        }
        if (Input.GetKey(KeyCode.LeftControl) && dirY != 0 && IsStickingToWall())
        {
            WallMove();
        }
    }

    private void WallMove()
    {
        rb.velocity = new Vector2(rb.velocity.x, dirY * wallMoveSpeed);
    }

    //private void WallJumping()
    //{
    //    //Wall jumping check
    //    if (IsStickingToWall() && Input.GetButtonDown("Jump") && canWallJump)
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, jumpForce - Mathf.Abs(dirX));
    //        canWallJump = false;
    //    }
    //    if (!IsStickingToWall())
    //    {
    //        canWallJump = true;
    //    }
    //}

    //private void WallSliding()
    //{
    //    //Wall sliding check
    //    if (IsStickingToWall())
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    //    }
    //}

    //Checks for downwards collison with ground
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    //Checks for sideways collision with ground
    private bool IsStickingToWall()
    {
        var stickFromRight = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, jumpableGround);
        var stickFromLeft = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, jumpableGround);

        wallIsRight = stickFromRight ? true : false;

        if (stickFromRight || stickFromLeft)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //private IEnumerator Dash()
    //{
    //    canDash = false;
    //    isDashing = true;
    //    float originalGravity = rb.gravityScale;
    //    rb.gravityScale = 0f;

    //    if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
    //    {
    //        rb.velocity = new Vector2(-diagonalDashLengths, diagonalDashLengths);
    //    }
    //    else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
    //    {
    //        rb.velocity = new Vector2(diagonalDashLengths, diagonalDashLengths);
    //    }
    //    else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
    //    {
    //        rb.velocity = new Vector2(diagonalDashLengths, -diagonalDashLengths);
    //    }
    //    else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
    //    {
    //        rb.velocity = new Vector2(-diagonalDashLengths, -diagonalDashLengths);
    //    }
    //    else if (Input.GetKey(KeyCode.A))
    //    {
    //        rb.velocity = new Vector2(-dashPower, 0f);
    //    }
    //    else if (Input.GetKey(KeyCode.D))
    //    {
    //        rb.velocity = new Vector2(dashPower, 0f);
    //    }
    //    else if (Input.GetKey(KeyCode.W))
    //    {
    //        rb.velocity = new Vector2(0f, dashPower);
    //    }
    //    else if (Input.GetKey(KeyCode.S))
    //    {
    //        rb.velocity = new Vector2(0f, -dashPower);
    //    }

    //    //rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
    //    yield return new WaitForSeconds(dashingTime);
    //    rb.gravityScale = originalGravity;
    //    isDashing = false;
    //    canDash = true;

    //    //if (Input.GetButtonDown("Horizontal"))
    //    //{
    //    //    rb.velocity = new Vector2(dirX * dashPower, rb.velocity.y);
    //    //}
    //    //else if (Input.GetButtonDown("Vertical"))
    //    //{
    //    //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    //    //}
    //}
}
