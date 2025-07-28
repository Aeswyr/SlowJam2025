using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogManager : Singleton<DialogManager>
{
    [SerializeField] private GameObject dialogParent;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private GameObject[] speakers;
    [SerializeField] private Image[] speakerSprites;
    [SerializeField] private TextMeshProUGUI[] speakerNames;
    private bool next;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dialogParent.SetActive(false);
    }

    public void StartDialogSequence(DialogConversation conversation, UnityAction callback = null)
    {
        StartCoroutine(RunDialogSequence(conversation, callback));
    }


    private IEnumerator RunDialogSequence(DialogConversation conversation, UnityAction callback)
    {
        dialogParent.SetActive(true);


        foreach (var line in conversation.conversation)
        {
            foreach (var image in speakerSprites)
            {
                image.sprite = line.Sprite;
                if (line.Sprite == null) {
                    image.color = Color.clear;
                } else {
                    image.color = Color.white;
                    image.SetNativeSize();
                }
            }
            foreach (var name in speakerNames)
            {
                name.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(line.SpeakerName));
                name.text = line.SpeakerName;
                Canvas.ForceUpdateCanvases();
            }

            speakers[0].SetActive(!line.IsRight);
            speakers[1].SetActive(line.IsRight);

            string text = line.TextLine;
            text = text.Replace("{flavor1}", Save.majorFlavor.ToString());
            text = text.Replace("{flavor2}", Save.minorFlavor.ToString());
            yield return PrintTextBox(text);

            yield return new WaitUntil(() => next);
            next = false;
        }
        

        dialogParent.SetActive(false);

        callback?.Invoke();
    }

    private IEnumerator PrintTextBox(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (next)
            {
                AudioManager.Instance.PlaySound("text_blip");
                next = false;
                break;
            }

            if (i % 4 == 0)
                AudioManager.Instance.PlaySound("text_blip");

            textBox.text = text.Substring(0, i);

            yield return new WaitForNextFrameUnit();

            
        }

        textBox.text = text;
    }

    public void OnNext()
    {
        next = true;
    }

}
