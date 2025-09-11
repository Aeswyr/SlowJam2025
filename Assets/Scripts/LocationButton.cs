using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationButton : MonoBehaviour
{
    [SerializeField] private List<Sprite> daySprites;
    [SerializeField] private Image icon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        icon.sprite = daySprites[Save.day];
        icon.SetNativeSize();
    }
}
