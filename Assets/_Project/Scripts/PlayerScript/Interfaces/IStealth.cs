namespace _Project.Scripts.PlayerScript.Interfaces
{
    public interface IStealth
    {
        bool IsStealthed { get; }
        void ToggleStealth();
        void TryStealthKill();
    }

}