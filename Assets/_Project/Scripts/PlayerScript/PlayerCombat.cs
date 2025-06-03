using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICombatant
{
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] private SwordCollisionScript SwordCollisionScript;
    [SerializeField] private AudioClip swordSwingSFX;

    public bool IsAttacking { get; private set; }
    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (SwordCollisionScript == null)
            SwordCollisionScript = GetComponentInChildren<SwordCollisionScript>();
    }

    public void PerformAttack()
    {
        animator.SetTrigger("attack");
    }

    public void StartAttack()
    {
        IsAttacking = true;
        Debug.Log("Attack started — can now deal damage.");
    }

    public void EndAttack()
    {
        IsAttacking = false;
        Debug.Log("Attack ended — damage disabled.");
    }

    public void ToggleBlock()
    {
        IsBlocking = !IsBlocking;
        Debug.Log(IsBlocking ? "Player started blocking!" : "Player stopped blocking!");
    }

    public void EnableSwordHitbox()
    {
        SwordCollisionScript?.EnableSwordCollider();
    }

    public void DisableSwordHitbox()
    {
        SwordCollisionScript?.DisableSwordCollider();
    }

    public void ResetSwordHit()
    {
        SwordCollisionScript?.ResetHit();
    }

    public void PlaySwordSwingSound()
    {
        if (swordSwingSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSwingSFX);
        }
    }

    public void TakeDamage(int amount)
    {
        GetComponent<PlayerScript>().TakeDamage(amount);
    }
}