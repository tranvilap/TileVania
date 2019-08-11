using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] float health = 100;
    [SerializeField] int lives = 3;

    [Header("Get Attacked Effects")]
    [SerializeField] float invincibleTime = 1.5f;
    [SerializeField][Tooltip("Time between 2 blinking action in second")] float timeBetweenTwoBlink = 0.1f;

    [SerializeField] [Tooltip("Sprite renderer of player for render blinking action when got hit")]
    SpriteRenderer bodySpriteRenderer;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Transparency of blinking effect when player get attacked")]
    float transparencyWhenAttacked = 0.2f;

    [SerializeField]
    [Tooltip("Player can't be control in this time")]
    float knockBackTime = 0.1f;

    [SerializeField] Vector2 knockBackForce = new Vector2(0.3f, 0f);

    private bool isInvincible = false;
    private bool isBlinking = false;
    private PlayerMovement playerMovement;
    

	// Use this for initialization
	void Start () {
        playerMovement = GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void GetAttacked(float damage)
    {
        if (isInvincible || isBlinking || lives <= 0) { return; }
        
        health -= damage;
        Debug.Log("Get hit, HP left " + health);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            ActiveInvincible();
            ActiveBlinkingEffect();
            playerMovement.SetIsAttacked();
            KnockBack();
        }
    }
    
    public void Die()
    {
        isInvincible = true;
        playerMovement.IsControllable = false;
        health = 100;
        lives -= 1;
        Debug.Log("Die");
        Respawn();
    }

    public void Respawn()
    {
        playerMovement.IsControllable = true;
        isInvincible = false;
    }

    IEnumerator ImmuneDamage()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    IEnumerator KnockBackAction()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(knockBackForce.x*Mathf.Sign(transform.localScale.x)*-1, knockBackForce.y);
        playerMovement.SetIsClimbing(false);
        playerMovement.IsControllable = false;
        yield return new WaitForSeconds(knockBackTime);
        playerMovement.IsControllable = true;
    }

    void KnockBack()
    {
        StartCoroutine(KnockBackAction());
    }

    IEnumerator BlinkRender()
    {
        isBlinking = true;
        Color currentColor = bodySpriteRenderer.color;
        while (isInvincible)
        {
            //bodySpriteRenderer.enabled = !bodySpriteRenderer.enabled;
            if(Mathf.Approximately(bodySpriteRenderer.color.a, currentColor.a))
            {
                bodySpriteRenderer.color = new Color(currentColor.r, currentColor.b, currentColor.g, transparencyWhenAttacked);
            }
            else
            {
                bodySpriteRenderer.color = currentColor;
            }
            yield return new WaitForSeconds(timeBetweenTwoBlink);
        }
        //bodySpriteRenderer.enabled = true;
        bodySpriteRenderer.color = currentColor;
        isBlinking = false;
    }

    private void ActiveBlinkingEffect()
    {
        StartCoroutine(BlinkRender());
    }

    private void ActiveInvincible()
    {
        StartCoroutine(ImmuneDamage());
    }

}
