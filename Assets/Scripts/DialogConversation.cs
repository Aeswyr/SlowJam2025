using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogConversation", menuName = "Scriptable Objects/DialogConversation")]
public class DialogConversation : ScriptableObject
{
    public ConversationPart[] conversation;
}


[Serializable]
public struct ConversationPart
{
    public Sprite Sprite;
    public bool IsRight;
    public string SpeakerName;
    [TextArea(3,20)]
    public string TextLine;
}
