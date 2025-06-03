namespace _Project.Scripts.PlayerScript.Interfaces
{
    public interface IHealth
    {
        void TakeDamage(int amount);
        bool IsDead { get; }
    }

}