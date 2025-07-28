using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] private string clickAudio;
    [SerializeField] private string hoverAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void PlayClick() {
        if (!string.IsNullOrEmpty(clickAudio))
            AudioManager.Instance.PlaySound(clickAudio);
    }

    public void PlayHover() {
        if (!string.IsNullOrEmpty(hoverAudio))
            AudioManager.Instance.PlaySound(hoverAudio);            
    }
}
