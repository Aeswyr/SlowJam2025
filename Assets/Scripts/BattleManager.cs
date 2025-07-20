using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject skills;
    [SerializeField] private GameObject skillButtonPrefab;

    private List<GameObject> skillButtons = new();
    private bool selecting;
    private int activeChar = -1;
    private List<Vector3> startPositions = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            startPositions.Add(characters[i].transform.position);
        }
        NextChar();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void ChangeActiveChar(int index)
    {
        if (activeChar != -1)
        {
            characters[activeChar].transform.DOLocalMoveX(startPositions[activeChar].x, 0.5f);
        }
        activeChar = index;
        characters[activeChar].transform.DOLocalMoveX(startPositions[activeChar].x + (activeChar < players.Length ? 1.25f : -1.25f), 0.5f);

        foreach (var old in skillButtons)
            Destroy(old);
        skillButtons.Clear();

        // add skill buttons
        var button = Instantiate(skillButtonPrefab, skills.transform);
        button.GetComponent<Button>().onClick.AddListener(NextChar);
        skillButtons.Add(button);

        // add undo button
        var undo = Instantiate(skillButtonPrefab, skills.transform);
        undo.GetComponent<Button>().onClick.AddListener(UndoAction);
        skillButtons.Add(undo);
    }

    public void NextChar()
    {
        ChangeActiveChar((activeChar + 1) % characters.Length);
    }

    public void UndoAction()
    {
        int index = activeChar - 1;
        if (index < 0)
            index = characters.Length - 1;
        ChangeActiveChar(index);
    }

}
