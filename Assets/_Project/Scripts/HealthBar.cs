using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void setHealth(int current, int max)
    {
        Debug.Log(current);
        fillImage.fillAmount = Mathf.Clamp01((float)current / max);
    }
}