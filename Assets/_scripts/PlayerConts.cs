using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConts : MonoBehaviour
{
    public float lastDirection, knockback;
    bool usingAbility;
    PlayerCombat playComb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (lastDirection == 0)
        {
            lastDirection = 1;
        }
        playComb = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCamera();

        if (WallGrab())
        {
            SetWallGarbState();
        }

        if (onWall)
        {
            HandleOnWall();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(PlayerDash());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(jumpCheck.position, 0.2f);
        Gizmos.DrawWireCube(wallCheckL.position, boxCheck);
        Gizmos.DrawWireCube(wallCheckR.position, boxCheck);
    }

    public void KnockBackPlayer(Vector2 direction, float force)
    {
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    bool pogoing;
    public void PoGoPlayer(Vector2 newVelo)
    {
        Vector2 setVelo = rb.velocity;
        setVelo.y = newVelo.y;
        setVelo.x += newVelo.x;
        rb.velocity = setVelo;
        dashed = false;
        pogoing = true;
        Invoke("PogoOut", 0.2f);
    }

    void PogoOut()
    {
        pogoing = false;
    }

    bool stunned, invcib;
    public IEnumerator TakeHit(Transform hitter)
    {
        if (invcib || pogoing)
        {
            yield break;
        }
        stunned = true;
        invcib = true;
        Vector2 hitForce = (transform.position - hitter.position).normalized;
        hitForce.x = hitForce.x > 0 ? 1 : -1;
        if (IsGrounded())
        {
            hitForce.x *= knockback;
            hitForce.y = 3;
        }
        else
        {
            hitForce.x *= knockback * 4;
            hitForce.y = 3;
        }
        playComb.HpHit();
        //print(hitForce);
        rb.velocity = hitForce;
        yield return new WaitForSeconds(1);
        stunned = false;
        yield return new WaitForSeconds(1);
        invcib = false;
    }

    [Header("Camera Stuff")]
    [SerializeField] Transform camLock;
    [SerializeField] float yOffset;
    void HandleCamera()
    {
        //allow player to look up and down in the world
        if (rb.velocity != Vector2.zero)
        {
            camLock.transform.localPosition = Vector3.zero;
            return;
        }
        float yInput = Input.GetAxisRaw("Vertical");
        camLock.transform.localPosition = Vector3.up * yInput * yOffset;
    }

    [Header("Run Stuff")]
    [SerializeField] float speed;
    Rigidbody2D rb;
    [SerializeField] float slowDown, maxXVelo, airPen, changeMod;
    public void HandleMovement()
    {
        if (usingAbility)
        {
            return;
        }

        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (stunned)
        {
            moveDir = Vector2.zero;
        }

        //set direction player is moving for attacks and abilities
        if (moveDir.x < 0)
        {
            lastDirection = -1;
        }
        else if (moveDir.x > 0)
        {
            lastDirection = 1;
        }

        //check if player is on a wall and stop them from movong into the wall they are attached to (this prevents sliding because of physics)
        if (moveDir.x > 0 && RightWallGrab())
        {
            moveDir.x = 0;
        }

        if (moveDir.x < 0 && LeftWallGrab())
        {
            moveDir.x = 0;
        }

        // modifiers for player states such as being in mid air or changeing direction
        float dirMult = 1;

        if (!IsGrounded())
        {
            dirMult *= airPen;
        }

        if ((rb.velocity.x > 0 && moveDir.x < 0) || (rb.velocity.x < 0 && moveDir.x > 0))
        {
            dirMult *= changeMod;
        }

        if (wallJumping)
        {
            dirMult = 0;
        }


        // slow player down while moving whithout using drag because of vertical movement
        if (moveDir.x == 0 && !stunned)
        {
            Vector2 artSlow = rb.velocity;
            if (rb.velocity.x > 0)
            {
                artSlow.x -= slowDown * Time.deltaTime * airPen;
            }
            else if (rb.velocity.x < 0)
            {
                artSlow.x += slowDown * Time.deltaTime * airPen;
            }
            rb.velocity = artSlow;
        }
        else// move player
        {
            rb.AddForce(moveDir * speed * Time.deltaTime * dirMult);
        }

        // clamp players velocity
        Vector2 veloClamp = rb.velocity;
        veloClamp.x = Mathf.Clamp(veloClamp.x, -maxXVelo, maxXVelo);
        rb.velocity = veloClamp;
    }

    [Header("Dash Stuff")]
    public bool dashed;
    [SerializeField] float dashTime, dashPower, dashCD;
    [Range(0, 1)] public float postDashSpeed;

    // dash disableing other movement and powers 
    public IEnumerator PlayerDash()
    {
        if (dashed || stunned)
        {
            yield break;
        }

        if (onWall)
        {
            if (LeftWallGrab())
            {
                lastDirection = 1;
            }
            else
            {
                lastDirection = -1;
            }

        }

        rb.gravityScale = 0;
        usingAbility = true;
        dashed = true;
        float locTime = 0;
        rb.velocity = Vector2.zero;
        while (locTime < dashTime)
        {
            locTime += Time.deltaTime;
            rb.velocity = Vector3.right * lastDirection * dashPower;
            yield return new WaitForEndOfFrame();
        }
        rb.gravityScale = 1;
        rb.velocity = rb.velocity.normalized * (maxXVelo * postDashSpeed);
        usingAbility = false;

        yield return new WaitForSeconds(dashCD);
        while (!IsGrounded() && !WallGrab())
        {
            print("waitfordash");
            yield return new WaitForEndOfFrame();
        }
        dashed = false;

        yield break;
    }

    [Header("Jump Stuff")]
    [SerializeField] Transform jumpCheck;
    [SerializeField] float jumpTime, jumpPower;
    [SerializeField] float fallingGravity, peakGravity, peakVeloVar;
    [SerializeField] LayerMask jumpLayer;
    bool jumping;
    float airTime;
    public void HandleJump()
    {
        if (usingAbility)
        {
            return;
        }

        // change result if jumping from wall
        if (Input.GetKeyDown(KeyCode.Space) && onWall)
        {
            wallJumping = true;
            airTime = 0;
            rb.velocity = Vector2.zero;

            if (LeftWallGrab())
            {
                xDirPower = xPower;
                lastDirection = 1;
            }
            else
            {
                xDirPower = -xPower;
                lastDirection = -1; 
            }
        }

        // allow the player to jump higher if they hold the jump button
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            jumping = true;
            airTime = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space) || stunned)
        {
            jumping = false;
        }

        if (wallJumping)
        {
            WallJumping();
        }


        // apply jump force and check if the jump time is over 
        if (jumping)
        {
            airTime += Time.deltaTime;

            rb.AddForce(Vector3.up * jumpPower * Time.deltaTime);

            if (airTime >= jumpTime)
            {
                jumping = false;
            }
        }

        if (!IsGrounded() && rb.velocity.y < -peakVeloVar)
        {
            rb.gravityScale = fallingGravity;
        }
        else if (!IsGrounded() && Mathf.Abs(rb.velocity.y) < peakVeloVar)
        {
            rb.gravityScale = peakGravity;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    public bool IsGrounded()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(jumpCheck.position, 0.2f, jumpLayer);
        return hits.Length > 0;
    }

    [Header("Wall Jump Stuff")]
    [SerializeField] Vector2 boxCheck;
    [SerializeField] Transform wallCheckL, wallCheckR;
    [SerializeField] float wallSlide, xPower, yBoost;
    float xDirPower;
    bool onWall, wallJumping;

    void WallJumping()
    {
        airTime += Time.deltaTime;

        rb.AddForce(new Vector2(xDirPower, (jumpPower * yBoost)) * Time.deltaTime);

        if (airTime >= jumpTime)
        {
            wallJumping = false;
        }
    }

    // check if the player is greabbing a wall;
    bool WallGrab()
    {
        if (stunned)
        {
            return false;
        }
        Collider2D[] hits = Physics2D.OverlapBoxAll(wallCheckL.position, boxCheck, 0, jumpLayer);
        Collider2D[] hitsB = Physics2D.OverlapBoxAll(wallCheckR.position, boxCheck, 0, jumpLayer);
        return (hits.Length + hitsB.Length) > 0 && !IsGrounded();
    }


    // find which wall the player is grabbing
    bool LeftWallGrab()
    {
        if (stunned)
        {
            return false;
        }
        Collider2D[] hits = Physics2D.OverlapBoxAll(wallCheckL.position, boxCheck, 0, jumpLayer);
        return hits.Length > 0 && !IsGrounded();
    }

    bool RightWallGrab()
    {
        if (stunned)
        {
            return false;
        }
        Collider2D[] hits = Physics2D.OverlapBoxAll(wallCheckR.position, boxCheck, 0, jumpLayer);
        return hits.Length > 0 && !IsGrounded();
    }
    void SetWallGarbState()
    {
        onWall = true;
        Vector2 curVelo = rb.velocity;
        curVelo.y = 0;
        rb.velocity = curVelo;
    }

    void OffWallGrab()
    {
        onWall = false;
    }


    // have the player slide down the wall they are on
    void HandleOnWall()
    {
        Vector2 curVelo = rb.velocity;
        curVelo.y = -wallSlide;
        rb.velocity = curVelo;

        if (!WallGrab())
        {
            OffWallGrab();
        }
    }
}
