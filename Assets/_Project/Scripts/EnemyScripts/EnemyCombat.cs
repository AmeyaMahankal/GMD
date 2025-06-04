using UnityEngine;
using _Project.Scripts.PlayerScript.Interfaces;

[RequireComponent(typeof(AudioSource))]
public class EnemyCombat : MonoBehaviour, ICombatant
{
    [SerializeField] private AudioClip swordSwingSFX;
    [SerializeField] private int baseDamageToPlayer = 5;
    [SerializeField] private float stopDistance = 2f;

    private AudioSource audioSource;
    private Transform player;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    public void PlaySwordSwingSound()
    {
        if (swordSwingSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSwingSFX);
        }
    }

    public void PerformAttack()
    {
        throw new System.NotImplementedException();
    }

    public void StartAttack()
    {
        Debug.Log("Enemy attack started — attempting to deal damage.");

        if (player != null && Vector3.Distance(transform.position, player.position) <= stopDistance + 0.25f)
        {
            PlayerScript target = player.GetComponent<PlayerScript>();
            if (target != null)
            {
                target.TakeDamage(baseDamageToPlayer);
                Debug.Log($"Enemy dealt {baseDamageToPlayer} damage to player.");
            }
            else
            {
                Debug.LogWarning("PlayerScript not found on target.");
            }
        }
    }

    public void EndAttack()
    {
        Debug.Log("Enemy attack ended.");
    }

    public void EnableSwordHitbox() { }
    public void DisableSwordHitbox() { }
    public void ResetSwordHit() { }
}