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
    void Start()
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
                image.SetNativeSize();
            }
            foreach (var name in speakerNames)
                    name.text = line.SpeakerName;

            speakers[0].SetActive(!line.IsRight);
            speakers[1].SetActive(line.IsRight);

            yield return PrintTextBox(line.TextLine);

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
                next = false;
                break;
            }

            textBox.text = text.Substring(0, i);

            yield return new WaitForSeconds(0.05f);

            
        }

        textBox.text = text;
    }

    public void OnNext()
    {
        next = true;
    }

}
