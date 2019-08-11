using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour {

    [SerializeField] float movingSpeed = 4f;

    [SerializeField] Collider2D footCollider;
    [SerializeField] LayerMask whatIsGround;

    Rigidbody2D rb2D;
    Animator animator;

    private const string IS_MOVING = "isMoving";
    private int isMovingBoolHash;

    private const string IS_ON_GROUND = "isOnGround";
    private int isOnGroundBoolHash;

	// Use this for initialization
	void Start () {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        isMovingBoolHash = Animator.StringToHash(IS_MOVING);
        isOnGroundBoolHash = Animator.StringToHash(IS_ON_GROUND);
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        CheckAnimationStates();
	}


    private void Move()
    {
        if (!IsOnGround()) { return; }
        if(!animator.GetBool(isMovingBoolHash))
        {
            SetIsMoving(true);
        }
        rb2D.velocity = new Vector2(movingSpeed*Mathf.Sign(transform.localScale.x), 0f);
    }

    public void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, 
                                            transform.localScale.y, 
                                            transform.localScale.z);
    }



    private void CheckAnimationStates()
    {
        if (IsOnGround()) { SetIsOnGround(true); }
        else { SetIsOnGround(false); }
    }


    private bool IsOnGround()
    {
        return footCollider.IsTouchingLayers(whatIsGround);
    }

    private void SetIsMoving(bool boolValue)
    {
        animator.SetBool(isMovingBoolHash, boolValue);
    }
    private void SetIsOnGround(bool boolValue)
    {
        animator.SetBool(isOnGroundBoolHash, boolValue);
    }

}
