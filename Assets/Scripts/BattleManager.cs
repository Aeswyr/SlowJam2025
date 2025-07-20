using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<int> skillQueue = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            startPositions.Add(characters[i].transform.localPosition);
        }
        selecting = true;
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
        characters[activeChar].transform.DOLocalMoveX(startPositions[activeChar].x + 1.25f, 0.5f);

        foreach (var old in skillButtons)
            Destroy(old);
        skillButtons.Clear();

        if (selecting)
        {
            // add skill buttons
            var button = Instantiate(skillButtonPrefab, skills.transform);
            button.GetComponent<Button>().onClick.AddListener(AddAction);
            skillButtons.Add(button);

            // add undo button
            var undo = Instantiate(skillButtonPrefab, skills.transform);
            undo.GetComponent<Button>().onClick.AddListener(UndoAction);
            skillButtons.Add(undo);
        }
    }

    public void NextChar()
    {
        ChangeActiveChar((activeChar + 1) % characters.Length);
    }

    public void ResetChars()
    {
        activeChar = -1;
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].transform.DOLocalMoveX(startPositions[i].x, 0.5f);
        }
    }

    public void UndoAction()
    {
        skillQueue.RemoveAt(skillQueue.Count - 1);
        int index = activeChar - 1;
        if (index < 0)
            index = characters.Length - 1;
        ChangeActiveChar(index);
    }

    public void AddAction()
    {
        skillQueue.Add(0);
        if (skillQueue.Count < 5)
        {
            NextChar();
        }
        else
        {
            selecting = false;
            StartCoroutine(ExecuteTurn());
        }
    }

    private IEnumerator ExecuteTurn()
    {
        ResetChars();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < characters.Length; i++)
        {
            NextChar();
            yield return new WaitForSeconds(1f);
        }

        skillQueue.Clear();

        selecting = true;
        ResetChars();
        yield return new WaitForSeconds(0.5f);
        NextChar();
    }

}
