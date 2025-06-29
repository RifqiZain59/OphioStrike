using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;
    public float bounceForce = 8f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private float mobileInputX = 0f;

    private Vector2 moveInput;
    private bool isJumping = false;
    private bool isAttacking = false;

    private enum MovementState { idle, walk, jump, fall, run, attack }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    public TextMeshProUGUI healthText;
    public Image healthBarImage;

    [Header("Knockback Settings")]
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private float knockBackThrust = 10f;

    private bool isKnockedBack = false;

    [Header("Coin System")]
    public int currentCoins = 0;
    public TextMeshProUGUI coinText;

    [Header("Enemy Defeated System")]
    public int enemiesDefeated = 0;
    public TextMeshProUGUI enemiesDefeatedText;

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("Coin saat ini: " + currentCoins);

        // Hanya tampilkan angka koin
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        playerController = new PlayerController();

        currentHealth = maxHealth;

        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = 1f;
        }
        UpdateHealthUI();

        UpdateEnemiesDefeatedUI();
    }

    private void OnEnable()
    {
        playerController.Enable();

        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        playerController.Movement.Jump.performed += ctx => Jump();

        playerController.Movement.Attack.performed += ctx => Attack();
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
        }
        else
        {
            moveInput = playerController.Movement.Move.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack || isAttacking) return;

        Vector2 targetVelocity = new Vector2((moveInput.x + mobileInputX) * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        UpdateAnimation();

        if (isGrounded() && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            isJumping = false;
        }
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (isAttacking)
        {
            state = MovementState.attack;
        }
        else
        {
            float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

            if (horizontal > 0f)
            {
                state = MovementState.walk;
                sprite.flipX = false;
            }
            else if (horizontal < 0f)
            {
                state = MovementState.walk;
                sprite.flipX = true;
            }
            else
            {
                state = MovementState.idle;
            }

            if (rb.velocity.y > 0.1f)
            {
                state = MovementState.jump;
            }
            else if (rb.velocity.y < -0.1f)
            {
                state = MovementState.fall;
            }
        }

        anim.SetInteger("state", (int)state);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void Jump()
    {
        if (isGrounded() && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    public void MoveRight(bool isPressed)
    {
        if (isPressed)
            mobileInputX = 1f;
        else if (mobileInputX == 1f)
            mobileInputX = 0f;
    }

    public void MoveLeft(bool isPressed)
    {
        if (isPressed)
            mobileInputX = -1f;
        else if (mobileInputX == -1f)
            mobileInputX = 0f;
    }

    public void MobileJump()
    {
        if (isGrounded() && !isAttacking)
        {
            Jump();
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.SetTrigger("Attack");

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    public void TakeDamage(int damage, Vector2 direction)
    {
        if (isKnockedBack) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player Mati");
            gameObject.SetActive(false);
        }

        StartCoroutine(HandleKnockback(direction.normalized));
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Hanya tampilkan angka health, jika ingin menghilangkan teks "Health:"
        if (healthText != null)
            healthText.text = currentHealth.ToString(); // Diubah dari "Health: " + currentHealth;
        
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private IEnumerator HandleKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;

        Vector2 force = direction * knockBackThrust * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    public void UpdateEnemiesDefeatedUI()
    {
        // Hanya tampilkan angka musuh yang dikalahkan
        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = enemiesDefeated.ToString();
        }
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log("Total Musuh Dikalahkan: " + enemiesDefeated);
        UpdateEnemiesDefeatedUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rb.velocity.y < 0 && transform.position.y > collision.transform.position.y + collision.collider.bounds.extents.y * 0.8f)
            {
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(100);
                    Debug.Log("Musuh terbunuh (diinjak)!");
                    rb.velocity = new Vector2(rb.velocity.x, bounceForce);

                    EnemyDefeated();
                }
            }
            else
            {
                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                TakeDamage(10, knockbackDirection);
            }
        }
    }
}