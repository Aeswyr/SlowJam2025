using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject skills;
    [SerializeField] private GameObject skillButtonPrefab;

    private List<GameObject> skillButtons = new();
    private bool selecting;
    private int activeMember;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void ChangeActiveChar(int index)
    {
        
    }
}
