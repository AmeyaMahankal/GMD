using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonFC : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public AudioClip highlightSound;
    public AudioClip submitSound;
    public GameObject border;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GameObject.Find("ButtonSoundManager")?.GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isSelected = EventSystem.current.currentSelectedGameObject == gameObject;
        border.SetActive(isSelected);
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (highlightSound != null)
            audioSource?.PlayOneShot(highlightSound);
    }
    
    public void OnSubmit(BaseEventData eventData)
    {
        if (submitSound != null)
            audioSource?.PlayOneShot(submitSound);
    }
}