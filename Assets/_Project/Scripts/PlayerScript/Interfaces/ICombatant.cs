public interface ICombatant
{
    void PerformAttack();
    void StartAttack();
    void EndAttack();
    void EnableSwordHitbox();
    void DisableSwordHitbox();
    void ResetSwordHit();
    void PlaySwordSwingSound();
}