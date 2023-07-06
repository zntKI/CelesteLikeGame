using UnityEngine;

public class PlayerWallMovement : MonoBehaviour
{
    //private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private float gravityScale;

    //private float dirXR;
    //private float dirYR;

    [Header("Wall Moving")]

    [SerializeField] private LayerMask ground;
    [SerializeField] private float wallSlidingSpeed = 0.2f;
    [SerializeField] private float stickingToWallMaxTime = 5f;
    [SerializeField] private Vector2 overlapBoxSize;
    [SerializeField] private Vector2 rightCollisionOffset, leftCollisionOffset;

    private float stickingToWallTimer = 0f;
    private bool canHoldOntoWalls = true;

    [Space(10)]
    [Header("Wall Jumping")]

    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private Vector2 wallJumpingPower;

    //private bool isWallJumping;
    private bool isWallRight;
    private float wallJumpingDirection;
    private float wallJumpingCounter;


    // Start is called before the first frame update
    void Start()
    {
        PlayerCommon.rb = GetComponent<Rigidbody2D>();
        gravityScale = PlayerCommon.rb.gravityScale;
        coll = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerCommon.dirXR = Input.GetAxisRaw("Horizontal");
        //PlayerCommon.dirYR = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyUp(KeyCode.LeftControl) || !IsStickingToWall())
        {
            PlayerCommon.rb.gravityScale = gravityScale;
        }

        if (stickingToWallTimer > stickingToWallMaxTime)
        {
            PlayerCommon.rb.gravityScale = gravityScale;
            canHoldOntoWalls = false;
        }

        if (IsStickingToWall() && Input.GetKey(KeyCode.LeftControl) && canHoldOntoWalls)
        {
            PlayerCommon.rb.gravityScale = 0f;
            PlayerCommon.rb.velocity = new Vector2(PlayerCommon.rb.velocity.x, PlayerCommon.dirYR * wallSlidingSpeed);
            stickingToWallTimer += Time.deltaTime;
        }

        WallJump();
    }

    private void WallJump()
    {
        if (PlayerCommon.isWallJumping)
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        bool timePeriodCanMove = wallJumpingCounter > 0 && wallJumpingCounter < wallJumpingTime / 2;
        if (wallJumpingCounter < 0 || (IsStickingToWall() && PlayerCommon.isWallJumping && timePeriodCanMove))
        {
            PlayerCommon.isWallJumping = false;
            wallJumpingCounter = 0f;
        }

        if (IsStickingToWall() && Input.GetButtonDown("Jump") && PlayerCommon.isNotGrounded)
        {
            PlayerCommon.isWallJumping = true;
            wallJumpingDirection = isWallRight ? -1f : 1f;
            PlayerCommon.rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = wallJumpingTime;
        }
    }

    private bool IsStickingToWall()
    {
        var stickFromRight = Physics2D.OverlapBox((Vector2)coll.bounds.center + rightCollisionOffset, overlapBoxSize, 0f, ground);
        var stickFromLeft = Physics2D.OverlapBox((Vector2)coll.bounds.center + leftCollisionOffset, overlapBoxSize, 0f, ground);

        //only used when sure that the player is sticking to a wall
        isWallRight = stickFromRight ? true : false;

        return (stickFromRight || stickFromLeft);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube((Vector2)coll.bounds.center + rightCollisionOffset, overlapBoxSize);
        Gizmos.DrawWireCube((Vector2)coll.bounds.center + leftCollisionOffset, overlapBoxSize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canHoldOntoWalls = true;
            stickingToWallTimer = 0f;
        }
    }
}
