using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private DialogConversation testDialog;
    public void OnProducePressed()
    {
        DialogManager.Instance.StartDialogSequence(testDialog, CompleteShopping);
    }

    public void OnMeatPressed()
    {
        DialogManager.Instance.StartDialogSequence(testDialog, CompleteShopping);
    }

    public void OnSpicePressed()
    {
        DialogManager.Instance.StartDialogSequence(testDialog, CompleteShopping);
    }

    public void OnCarbPressed()
    {
        DialogManager.Instance.StartDialogSequence(testDialog, CompleteShopping);
    }

    public void CompleteShopping()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
