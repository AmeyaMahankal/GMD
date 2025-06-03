using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    private float movementX;
    private float movementY;
    private bool isCrouched = false;
    private bool isStealthed = false;
    private Animator animator;

    [SerializeField] private float speed = 5;
    [SerializeField] public TextMeshProUGUI inputIndicator;
    [SerializeField] public TextMeshProUGUI crouchedIndicator;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = 9.81f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;

    [Header("Attack Settings")]
    public bool isAttacking = false;
    [SerializeField] private SwordCollision swordCollision;

    [Header("Block Settings")]
    public bool isBlocking = false;

    [Header("Player Health")]
    [SerializeField] private int playerHealth = 100;

    [Header("Stealth Kill Settings")]
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float killAngle = 45f;

    [Header("Audio")]
    [SerializeField] private AudioClip swordSwingSFX;
    private AudioSource audioSource;

    public bool IsStealthed => isStealthed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (swordCollision == null)
            swordCollision = GetComponentInChildren<SwordCollision>();
    }

    private void Start()
    {
        animator.SetBool("isGrounded", true);
        animator.SetBool("isJumping", false);
    }

    private void Update()
    {
        Vector3 movement = new Vector3(movementX, 0f, movementY);
        animator.SetFloat("speed", movement.magnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movement),
                0.15f
            );
        }

        transform.Translate(movement * Time.deltaTime * speed, Space.World);

        if (isJumping)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);

            if (transform.position.y <= 0f)
            {
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;

                verticalVelocity = 0f;
                isJumping = false;
                animator.SetBool("isJumping", false);
                animator.SetBool("isGrounded", true);
            }
        }
    }

    public void OnMovement(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void OnA()
    {
        UpdateInputIndicator("A");

        if (!isJumping)
        {
            isJumping = true;
            animator.SetTrigger("JumpTrigger");
            animator.SetBool("isGrounded", true);
            verticalVelocity = jumpHeight;
        }
    }

    public void OnB()
    {
        isCrouched = !isCrouched;
        isStealthed = isCrouched;
        UpdateInputIndicator("B");
        UpdateCrouchIndicator();

        Debug.Log(isStealthed ? "Player entered STEALTH mode!" : "Player exited STEALTH mode!");
    }

    public void OnX()
    {
        UpdateInputIndicator("X");
    }

    public void OnY() => UpdateInputIndicator("Y");

    public void OnLeftTrigger()
    {
        UpdateInputIndicator("L Trigger");
        isBlocking = !isBlocking;

        Debug.Log(isBlocking ? "Player started blocking!" : "Player stopped blocking!");
    }

    public void OnRightTrigger()
    {
        UpdateInputIndicator("R Trigger");

        if (isStealthed)
        {
            TryStealthKill();
        }
        else
        {
            PerformAttack();
        }
    }

    public void OnStart() => UpdateInputIndicator("Start");

    private void UpdateCrouchIndicator()
    {
        crouchedIndicator.text = isCrouched ? "Crouched" : "Not Crouched.";
        crouchedIndicator.color = isCrouched ? Color.red : Color.green;
    }

    private void UpdateInputIndicator(string input) => inputIndicator.text = input;

    private void PerformAttack()
    {
        animator.SetTrigger("attack");
    }

    // Animation Event: Call this during the swing animation
    public void PlaySwordSwingSound()
    {
        if (swordSwingSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSwingSFX);
        }
    }

    public void StartAttack()
    {
        isAttacking = true;
        Debug.Log("Attack started — can now deal damage.");
    }

    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Attack ended — damage disabled.");
    }

    public void EnableSwordHitbox()
    {
        swordCollision?.EnableSwordCollider();
    }

    public void DisableSwordHitbox()
    {
        swordCollision?.DisableSwordCollider();
    }

    public void ResetSwordHit()
    {
        swordCollision?.ResetHit();
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log($"Player took {damage} damage. Remaining Health = {playerHealth}");

        if (playerHealth <= 0)
            Debug.Log("Player is dead!");
    }

    private void TryStealthKill()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= killDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        if (closestEnemy == null) return;

        Vector3 toPlayer = (transform.position - closestEnemy.transform.position).normalized;
        float dotProduct = Vector3.Dot(closestEnemy.transform.forward, toPlayer);
        float requiredDot = Mathf.Cos(killAngle * Mathf.Deg2Rad);

        Debug.Log($"dotProduct: {dotProduct}, requiredDot: {requiredDot}");

        if (dotProduct > requiredDot)
        {
            Debug.Log("STEALTH KILL TRIGGERED");

            Animator enemyAnimator = closestEnemy.GetComponentInChildren<Animator>();
            if (enemyAnimator)
            {
                enemyAnimator.SetTrigger("Die");
            }

            DummyHealth health = closestEnemy.GetComponent<DummyHealth>();
            if (health != null)
            {
                Debug.Log("Applying 999 damage to enemy...");
                health.TakeDamage(999);
            }
            else
            {
                Debug.Log("DummyHealth not found, destroying object directly.");
                Destroy(closestEnemy);
            }
        }
        else
        {
            Debug.Log("Stealth kill failed — player not behind the enemy.");
        }
    }
}
