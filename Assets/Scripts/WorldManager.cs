using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private DialogConversation[] openingDialog;
    [SerializeField] private DialogConversation[] butcherDialog;
    [SerializeField] private DialogConversation[] grocerDialog;
    [SerializeField] private DialogConversation[] bakerDialog;
    [SerializeField] private DialogConversation[] spiceDialog;



    public void Start()
    {
        if (openingDialog[Save.day] != null)
        {
            DialogManager.Instance.StartDialogSequence(openingDialog[Save.day]);
        }
    }

    public void OnProducePressed()
    {
        DialogManager.Instance.StartDialogSequence(grocerDialog[Save.day], CompleteShopping);
    }

    public void OnMeatPressed()
    {
        DialogManager.Instance.StartDialogSequence(butcherDialog[Save.day], CompleteShopping);
    }

    public void OnSpicePressed()
    {
        DialogManager.Instance.StartDialogSequence(spiceDialog[Save.day], CompleteShopping);
    }

    public void OnCarbPressed()
    {
        DialogManager.Instance.StartDialogSequence(bakerDialog[Save.day], CompleteShopping);
    }

    public void CompleteShopping()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
