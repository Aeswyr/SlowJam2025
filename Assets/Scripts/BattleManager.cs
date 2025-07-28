using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> characters;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI roundCount;
    [Header("Enemies")]
    [SerializeField] List<EnemyTeam> enemies;

    [Header("Dish")]
    [SerializeField] private Transform allyDish;
    [SerializeField] private Transform enemyDish;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Transform allyDefense;
    [SerializeField] private Transform enemyDefense;

    [Header("Dialog")]
    [SerializeField] private DialogConversation battleTutorial;
    [SerializeField] private DialogConversation[] openingDialog;
    [SerializeField] private DialogConversation closingDialog;
    [SerializeField] private DialogConversation[] victoryDialog;
    [SerializeField] private DialogConversation[] lossDialog;

    [Header("Skills")]
    [SerializeField] private SkillLibrary skillLibrary;
    [SerializeField] private GameObject skills;
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private GameObject skillName;
    [SerializeField] private Transform skillTarget;
    [SerializeField] private TextMeshProUGUI skillText;

    [SerializeField] private string[] playerSkills;
    [SerializeField] private string[] enemySkills;

    [Header("Judgement")]
    [SerializeField] private GameObject judgementParent;
    [SerializeField] private SpriteRenderer[] allyStars;
    [SerializeField] private SpriteRenderer[] enemyStars;
    [SerializeField] private Sprite star;
    [SerializeField] private Sprite[] judgementAnimation;
    [SerializeField] private Image judgementComic;

    private List<GameObject> skillButtons = new();
    private bool selecting;
    private int activeChar = -1;
    private List<Vector3> startPositions = new();
    private List<int> skillQueue = new();
    private List<int> allyMeal = new();
    private List<int> enemyMeal = new();
    private int allyActions, enemyActions;
    private Vector3 skillStartingPos;
    private int allyGuards, enemyGuards;

    private int round = 1;
    private bool locked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        judgementParent.SetActive(false);
        gameOver.SetActive(false);
        judgementComic.gameObject.SetActive(false);

        DrawDefense();

        var enemyTeam = enemies[Save.day].sprites;
        for (int i = 0; i < 5; i++) {
            if (i < enemyTeam.Length)
            {
                characters[5 + i].GetComponent<SpriteRenderer>().sprite = enemyTeam[i];
            }
            else
            {
                Destroy(characters[characters.Count - 1]);
                characters.RemoveAt(characters.Count - 1);
            }
        }


        skillStartingPos = skillName.transform.position;
        for (int i = 0; i < characters.Count; i++)
        {
            startPositions.Add(characters[i].transform.localPosition);
        }

        DialogManager.Instance.StartDialogSequence(openingDialog[Save.day], BeginCombat);

        roundCount.text = round.ToString();
    }

    void BeginCombat()
    {
        selecting = true;
        NextChar();

        if (Save.day == 0)
        {
            DialogManager.Instance.StartDialogSequence(battleTutorial);
        }
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
            var skillOptions = playerSkills[activeChar].Split(',');
            for (int i = 0; i < skillOptions.Length; i++)
            {
                var button = Instantiate(skillButtonPrefab, skills.transform);
                int skillId = int.Parse(skillOptions[i]);
                button.GetComponent<Button>().onClick.AddListener(() => AddAction(skillId));
                var image = button.transform.GetChild(0).GetComponent<Image>();
                image.sprite = skillLibrary.skills[skillId].Sprite;
                skillButtons.Add(button);
            }

            // add undo button
            if (activeChar != 0)
            {
                var undo = Instantiate(skillButtonPrefab, skills.transform);
                undo.GetComponent<Button>().onClick.AddListener(UndoAction);
                skillButtons.Add(undo);
            }

            locked = false;
        }
    }

    public void NextChar()
    {
        ChangeActiveChar((activeChar + 1) % characters.Count);
    }

    public void ResetChars()
    {
        activeChar = -1;
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].transform.DOLocalMoveX(startPositions[i].x, 0.5f);
        }
    }

    public void UndoAction()
    {
        if (locked)
            return;
        locked = true;

        int skill = skillQueue[skillQueue.Count - 1];
        skillQueue.RemoveAt(skillQueue.Count - 1);

        if (skill == 2)
        {
            allyGuards--;
            DrawDefense();
        }

        int index = activeChar - 1;
        if (index < 0)
            index = characters.Count - 1;

        ChangeActiveChar(index);
    }

    public void AddAction(int actionId)
    {
        if (locked)
            return;
        locked = true;

        skillQueue.Add(actionId);


        if (actionId == 2)
        {
            allyGuards++;
            DrawDefense();
        }
            

        if (skillQueue.Count < 5)
        {
            NextChar();
        }
        else
        {
            SetupCombatRound();
        }
    }

    private void SetupCombatRound()
    {
        var options = enemySkills[Save.day].Split(',');
        for (int i = 0; i < characters.Count - 5; i++)
        {
            int skill = int.Parse(options[UnityEngine.Random.Range(0, options.Length)]);
            skillQueue.Add(skill);
            if (skill == 2)
                enemyGuards++;
        }
        selecting = false;
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        ResetChars();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < characters.Count; i++)
        {
            NextChar();
            yield return new WaitForSeconds(0.5f);
            yield return ExecuteSkill();
            yield return new WaitForSeconds(0.5f);
        }

        skillQueue.Clear();

        selecting = true;
        ResetChars();
        yield return new WaitForSeconds(0.5f);
        if (round == 3)
        {
            DialogManager.Instance.StartDialogSequence(closingDialog, StartJudgement);
        }
        else
        {
            allyGuards = 0;
            enemyGuards = 0;
            DrawDefense();

            round++;
            roundCount.text = round.ToString();
            NextChar();
        }
    }

    private int RateDish(List<int> dish)
    {
        int[] flavors = new int[(int)Flavor.MAX];
        foreach (int id in dish)
        {
            var skill = skillLibrary.skills[id];
            flavors[(int)Flavor.BITTER] += skill.flavors.Bitter;
            flavors[(int)Flavor.SALT] += skill.flavors.Salty;
            flavors[(int)Flavor.SOUR] += skill.flavors.Sour;
            flavors[(int)Flavor.SWEET] += skill.flavors.Sweet;
            flavors[(int)Flavor.UMAMI] += skill.flavors.Umami;
        }

        int score = 0;
        if (flavors[(int)Save.majorFlavor] > 0)
        {
            score++;
        }
        if (flavors[(int)Save.majorFlavor] > 5)
        {
            score++;
        }
        if (flavors[(int)Save.minorFlavor] > 0)
        {
            score++;
        }
        if (flavors[(int)Save.majorFlavor] > flavors[(int)Save.minorFlavor])
        {
            score++;
        }
        bool majorFlavorLargest = true;
        for (int i = 0; i < flavors.Length; i++)
        {
            if (i == (int)Save.majorFlavor)
                continue;

            if (flavors[i] >= flavors[(int)Save.majorFlavor])
            {
                majorFlavorLargest = false;
                break;
            }
        }
        if (majorFlavorLargest)
        {
            score++;
        }
        return score;
    }

    private void StartJudgement()
    {
        foreach (var chara in characters)
            chara.SetActive(false);
        judgementParent.SetActive(true);

        enemyGuards = 0;
        allyGuards = 0;
        DrawDefense();

        StartCoroutine(JudgementSequence());

        IEnumerator JudgementSequence()
        {
            int allyRating = RateDish(allyMeal);
            int enemyRating = RateDish(enemyMeal);

            judgementComic.gameObject.SetActive(true);
            foreach (var img in judgementAnimation)
            {
                judgementComic.sprite = img;
                yield return new WaitForSeconds(1.5f);
            }
            judgementComic.gameObject.SetActive(false);


            yield return new WaitForSeconds(1f);

            for (int i = 1; i <= 5; i++)
            {
                if (allyRating >= i)
                {
                    allyStars[i - 1].sprite = star;
                }
                if (enemyRating >= i)
                {
                    enemyStars[i - 1].sprite = star;
                }

                yield return new WaitForSeconds(0.5f + 0.2f * i);
            }

            if (allyRating >= enemyRating)
            {
                DialogManager.Instance.StartDialogSequence(victoryDialog[Save.day], () =>
                {
                    Save.day = (Save.day + 1) % 4;
                    SceneManager.LoadScene("WorldScene");
                });
            }
            else
            {
                DialogManager.Instance.StartDialogSequence(lossDialog[Save.day], () =>
                {
                    gameOver.SetActive(true);
                });
            }
        }
    }

    private IEnumerator ExecuteSkill()
    {
        int skillId = skillQueue[0];
        skillQueue.RemoveAt(0);
        var skill = skillLibrary.skills[skillId];

        skillText.text = skill.Name;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)skillName.transform);
        skillName.transform.DOMove(skillTarget.position, 0.5f);
        yield return new WaitForSeconds(0.5f);

        if (skill.IsFood)
        {
            var food = Instantiate(foodPrefab, activeChar < 5 ? allyDish : enemyDish);
            food.transform.localPosition = ((activeChar < 5 ? allyMeal.Count : enemyMeal.Count) * 0.25f + 0.5f) * Vector3.up;
            var sprite = food.GetComponent<SpriteRenderer>();
            sprite.sprite = skill.Sprite;
            sprite.sortingOrder = (activeChar < 5 ? allyActions : enemyActions) + 1;
            food.SetActive(true);

            AudioManager.Instance.PlaySound("splat");

            if (activeChar < 5)
                allyMeal.Add(skillId);
            else
                enemyMeal.Add(skillId);
        }
        else
        {
            switch (skillId)
            {
                case 0: // kick
                    List<int> meal = null;
                    Transform dish = null;

                    if (activeChar < 5)
                    {
                        if (enemyGuards > 0 || enemyMeal.Count == 0)
                        {
                            AudioManager.Instance.PlaySound("block");
                            enemyGuards--;
                            DrawDefense();
                            enemyDish.transform.DOShakePosition(0.25f, vibrato: 50);
                        }
                        else
                        {
                            meal = enemyMeal;
                            dish = enemyDish;
                        }
                    }
                    else if (activeChar >= 5)
                    {
                        if (allyGuards > 0 || allyMeal.Count == 0)
                        {
                            AudioManager.Instance.PlaySound("block");
                            allyGuards--;
                            DrawDefense();
                            allyDish.transform.DOShakePosition(0.25f, vibrato: 50);
                        }
                        else
                        {
                            meal = allyMeal;
                            dish = allyDish;
                        }
                    }

                    if (meal != null && dish != null)
                    {
                        int target = UnityEngine.Random.Range(0, meal.Count);
                        if (meal.Count > 0)
                        {
                            AudioManager.Instance.PlaySound("kick");
                            meal.RemoveAt(target);
                            var obj = dish.GetChild(target);
                            dish.DOShakePosition(0.25f, strength: 0.25f, vibrato: 20);
                            var seq = DOTween.Sequence();
                            seq.Append(obj.DOBlendableLocalRotateBy(180 * Vector3.forward, 0.5f));
                            seq.Join(obj.DOBlendableLocalMoveBy(10 * UnityEngine.Random.insideUnitCircle.normalized, 0.5f));
                            seq.Play();
                            yield return new WaitForSeconds(0.5f);
                            Destroy(obj.gameObject);
                        }

                        yield return new WaitForNextFrameUnit();
                        for (int i = 0; i < meal.Count; i++)
                            dish.GetChild(i).localPosition = (i * 0.25f + 0.5f) * Vector3.up;

                    }
                    break;
                case 1: // throw
                    if (activeChar < 5 && enemyGuards > 0)
                    {
                        AudioManager.Instance.PlaySound("block");
                        enemyGuards--;
                        DrawDefense();
                        enemyDish.transform.DOShakePosition(0.25f, vibrato: 50);
                    }
                    else if (activeChar >= 5 && allyGuards > 0)
                    {
                        AudioManager.Instance.PlaySound("block");
                        allyGuards--;
                        DrawDefense();
                        allyDish.transform.DOShakePosition(0.25f, vibrato: 50);
                    }
                    else
                    {
                        var food = Instantiate(foodPrefab, activeChar >= 5 ? allyDish : enemyDish);
                        food.transform.localPosition = ((activeChar >= 5 ? allyMeal.Count : enemyMeal.Count) * 0.25f + 0.5f) * Vector3.up;
                        var sprite = food.GetComponent<SpriteRenderer>();
                        sprite.sprite = skillLibrary.skills[25].Sprite;
                        sprite.sortingOrder = (activeChar >= 5 ? allyActions : enemyActions) + 1;
                        food.SetActive(true);

                        if (activeChar >= 5)
                            allyMeal.Add(25);
                        else
                            enemyMeal.Add(25);

                        Vector3 endPos = food.transform.position;
                        Vector3 startPos = characters[activeChar].transform.position;

                        food.transform.position = startPos;
                        food.transform.rotation = Quaternion.Euler(0, 0, 180);
                        
                        var seq = DOTween.Sequence();
                        
                        seq.Append(food.transform.DOMoveX(endPos.x, 1f).SetEase(Ease.Linear));
                        seq.Join(food.transform.DOBlendableRotateBy(180 * Vector3.forward, 1f));
                        seq.Join(food.transform.DOMoveY(startPos.y + 5, 0.5f).SetEase(Ease.OutQuad));
                        seq.Insert(0.5f, food.transform.DOMoveY(endPos.y, 0.5f).SetEase(Ease.InQuad));
                        seq.Play();

                        yield return new WaitForSeconds(1f);
                    }
                    break;
                case 2: // guard
                    AudioManager.Instance.PlaySound("defend");
                    break;
            }
        }

        if (activeChar < 5)
            allyActions++;
        else
            enemyActions++;


        yield return new WaitForSeconds(0.5f);

        skillName.transform.DOMove(skillStartingPos, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    private void DrawDefense()
    {
        for (int i = 0; i < 5; i++)
        {
            allyDefense.GetChild(i).gameObject.SetActive(allyGuards > i);
            enemyDefense.GetChild(i).gameObject.SetActive(enemyGuards > i);
        }
    }

    public void GameOverRetry()
    {
        SceneManager.LoadScene("WorldScene");
    }
    public void GameOverRestart()
    {
        Save.day = 0;
        SceneManager.LoadScene("WorldScene");
    }
    public void GameOverQuit()
    {
        Application.Quit();
    }

}

[Serializable]
public struct EnemyTeam
{
    public Sprite[] sprites;
}
