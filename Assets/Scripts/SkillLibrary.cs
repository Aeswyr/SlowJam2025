using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SkillLibrary", menuName = "Scriptable Objects/SkillLibrary")]
public class SkillLibrary : ScriptableObject
{
    public Skill[] skills;
}

[Serializable]
public struct Skill
{
    public string Name;
    public Sprite Sprite;
    public bool IsFood;
    public FlavorProfile flavors;
}

[Serializable]
public struct FlavorProfile
{
    public int Umami, Sweet, Salty, Sour, Bitter;
}