using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player's stats")]
    [SerializeField] float runningSpeed = 7f;
    [SerializeField] float jumpingPower = 22f;
    [SerializeField][Tooltip("Jumping power when player is climbing")]
    float climbingJumpPower = 17f;
    [SerializeField] float climbingSpeed = 3f;

    [Header("Player's config")]

    [SerializeField] [Tooltip("Collider that checks player is currently on ground")]
    Collider2D groundCheckCollider;

    [SerializeField]
    [Tooltip("Collider that represents for body (touching physical like stand on ground, where to stop when meet a wall)")]
    Collider2D bodyCollider;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsLadder;

    Animator animator;
    Rigidbody2D rb2D;
    float playerGravity;

    ///<param name="isMoving">Used in Player Animator Controller </param>
    ///<param name="isMovingBoolHash">Used to get isMoving variable's ID</param>
    private const string IS_RUNNING = "isRunning";
    private int isRunningBoolHash;

    ///<param name="isJumping">Used in Player Animator Controller </param>
    ///<param name="isJumpingBoolHash">Used to get isJumping variable's ID</param>
    private const string IS_JUMPING = "isJumping";
    private int isJumpingBoolHash;

    private const string IS_FALLING = "isFalling";
    private int isFallingBoolHash;

    private const string IS_CLIMBING = "isClimbing";
    private int isClimbingBoolHash;

    private const string CLIMBING_SPEED = "climbingAnimSpeed";
    private int climbingAnimSpeedFloatHash;

    private const string IS_ATTACKED = "isAttacked";
    private int isAttackedTrigger;

    private bool isFacingRight = true;
    private bool isControllable = true;

    public bool IsControllable
    {
        get
        {
            return isControllable;
        }

        set
        {
            isControllable = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        isRunningBoolHash = Animator.StringToHash(IS_RUNNING);
        isJumpingBoolHash = Animator.StringToHash(IS_JUMPING);
        isFallingBoolHash = Animator.StringToHash(IS_FALLING);
        isClimbingBoolHash = Animator.StringToHash(IS_CLIMBING);
        climbingAnimSpeedFloatHash = Animator.StringToHash(CLIMBING_SPEED);
        isAttackedTrigger = Animator.StringToHash(IS_ATTACKED);

        isFacingRight = transform.localScale.x > Mathf.Epsilon;

        playerGravity = rb2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsStandingStillInLadder();
        Run();
        
        Climb();
        CheckAnimationStates();
        Jump();
    }



    private void Run()
    {
        if (!IsControllable) { return; }
        float horizontalAxisValue = Input.GetAxisRaw("Horizontal") * runningSpeed;
        if (Mathf.Approximately(horizontalAxisValue, 0))
        {
            SetIsRunning(false);
            rb2D.velocity = new Vector2(0f, rb2D.velocity.y);
            return;
        }

        float horizontalVelocity = horizontalAxisValue;
        Vector2 targetVelocity = new Vector2(horizontalVelocity, rb2D.velocity.y);

        if (!animator.GetBool(isClimbingBoolHash))
        {
            SetIsRunning(true);
            rb2D.velocity = targetVelocity;
        }
        else
        {
            MovingInLadder(Mathf.Sign(horizontalAxisValue), false);
        }

        if (horizontalAxisValue < 0 && isFacingRight)
        {
            ChangeFacingDirection();
        }
        else if (horizontalAxisValue > 0 && !isFacingRight)
        {
            ChangeFacingDirection();
        }
    }

    private void Jump()
    {
        if (!IsControllable) { return; }
        if (animator.GetBool(isClimbingBoolHash)? Input.GetButtonDown("Jump"): Input.GetButton("Jump"))
        {
            if (!IsOnGround() && !animator.GetBool(isClimbingBoolHash)) { return; }
            SetIsJumping(true);
            if (animator.GetBool(isClimbingBoolHash))
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, climbingJumpPower);
            }
            else
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpingPower);
            }
            SetIsClimbing(false);
        }

    }

    private void Climb()
    {
        if (!IsControllable) { return; }
        float controlThrow = Input.GetAxisRaw("Vertical");

        if (!Mathf.Approximately(controlThrow, 0f))
        {
            if (!IsAtLadder()) { return; }

            SetIsClimbing(true);
            SetIsJumping(false);
            SetIsFalling(false);
            SetIsRunning(false);

            MovingInLadder(controlThrow, true);
        }
    }

    private void CheckIsStandingStillInLadder()
    {
        if (animator.GetBool(isClimbingBoolHash))
        {
            rb2D.velocity = new Vector2(0f, 0f);
            rb2D.gravityScale = 0f;
            animator.SetFloat(climbingAnimSpeedFloatHash, 0f);
        }
        else
        {
            rb2D.gravityScale = playerGravity;
        }
    }

    private void MovingInLadder(float controlInputValue, bool isMovingVertical)
    {
        if (!IsAtLadder()) { return; }
        //This function's used for moving when climbing ( vertical and horizontal)
        //Use in 2 functions - Move and Climb.
        animator.SetFloat(climbingAnimSpeedFloatHash, controlInputValue);
        if (!Mathf.Approximately(controlInputValue, 0f))
        {

            if (isMovingVertical)
            {
                rb2D.velocity += new Vector2(0f, controlInputValue * climbingSpeed);
            }
            else
            {
                rb2D.velocity += new Vector2(controlInputValue * climbingSpeed, 0f);
            }
        }
    }

    private void ChangeFacingDirection()
    {
        isFacingRight = !isFacingRight;

        Vector3 playerLocalScale = transform.localScale;
        playerLocalScale.x *= -1;
        transform.localScale = playerLocalScale;
    }

    private bool IsOnGround()
    {
        return groundCheckCollider.IsTouchingLayers(whatIsGround);
    }

    private bool IsAtLadder()
    {
        return bodyCollider.IsTouchingLayers(whatIsLadder);
    }

    private void CheckAnimationStates()
    {
        if (IsOnGround())
        {
            SetIsJumping(false);
            SetIsFalling(false);
            SetIsClimbing(false);
            return;
        }
        if (!IsAtLadder()) { SetIsClimbing(false); }
        if (rb2D.velocity.y < 0)
        {
            if (!animator.GetBool(isClimbingBoolHash))
            {
                SetIsFalling(true);
            }
        }
        else
        {
            //SetIsJumping(true);
            SetIsFalling(false);
        }
    }

    public void SetIsRunning(bool boolValue)
    {
        animator.SetBool(isRunningBoolHash, boolValue);
    }
    public void SetIsClimbing(bool boolValue)
    {
        animator.SetBool(isClimbingBoolHash, boolValue);
    }
    public void SetIsJumping(bool boolValue)
    {
        animator.SetBool(isJumpingBoolHash, boolValue);
    }
    public void SetIsFalling(bool boolValue)
    {
        animator.SetBool(isFallingBoolHash, boolValue);
    }
    public void SetIsAttacked()
    {
        animator.SetTrigger(isAttackedTrigger);
    }
}
