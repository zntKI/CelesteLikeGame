using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    //TODO: Fix dashing sideways only once when being grounded

    [SerializeField] private float dashingTime = 0.5f;
    [SerializeField] private float dashingDuration = 0.5f;
    //[SerializeField] private Vector2 dashPower;
    [SerializeField] private float dashPower = 30f;
    [SerializeField] private float linearDragMultiplier = 10f;

    private float dashingTimeCounter = 0f;
    private float dashingDurationCounter = 0f;

    private bool canDashAgain = true;
    private bool hasLeftGroundWhenDashing = false; //used for enabling the player to dash again if his previous dash has been only sideways

    // Update is called once per frame
    void Update()
    {
        if (PlayerCommon.isDashingForMovementStop)
        {
            dashingTimeCounter -= Time.deltaTime;
        }
        if (PlayerCommon.isDashingForDuration)
        {
            dashingDurationCounter -= Time.deltaTime;
            PlayerCommon.rb.drag += Time.deltaTime * linearDragMultiplier;
        }

        if (dashingTimeCounter < 0)
        {
            PlayerCommon.isDashingForMovementStop = false;
            dashingTimeCounter = 0f;
        }
        if (dashingDurationCounter < 0)
        {
            PlayerCommon.rb.gravityScale = PlayerCommon.gravityScale;
            dashingDurationCounter = 0f;
            PlayerCommon.rb.drag = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDashAgain)
        {
            //var x = PlayerCommon.dirXR == 0 ? PlayerCommon.rb.velocity.x : dashPower.x * PlayerCommon.dirXR;
            //var y = PlayerCommon.dirYR == 0 ? PlayerCommon.rb.velocity.y : dashPower.y * PlayerCommon.dirYR;
            //PlayerCommon.rb.velocity = new Vector2(/*dashPower.x * PlayerCommon.dirXR*/x, /*dashPower.y * PlayerCommon.dirYR*/y);

            PlayerCommon.rb.velocity = Vector2.zero;
            PlayerCommon.rb.velocity += new Vector2(PlayerCommon.dirXR, PlayerCommon.dirYR).normalized * dashPower;

            dashingTimeCounter = dashingTime;
            PlayerCommon.isDashingForMovementStop = true;

            PlayerCommon.rb.gravityScale = 0f;
            dashingDurationCounter = dashingDuration;
            PlayerCommon.isDashingForDuration = true;

            if (!hasLeftGroundWhenDashing)
            {
                canDashAgain = true;
            }
            else
            {
                canDashAgain = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDashAgain = true;
            hasLeftGroundWhenDashing = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasLeftGroundWhenDashing = true;
        }
    }
}
