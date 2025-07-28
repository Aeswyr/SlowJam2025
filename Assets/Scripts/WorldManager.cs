using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private DialogConversation[] openingDialog;
    [SerializeField] private DialogConversation[] butcherDialog;
    [SerializeField] private DialogConversation[] grocerDialog;
    [SerializeField] private DialogConversation[] bakerDialog;
    [SerializeField] private DialogConversation[] spiceDialog;
    [SerializeField] private DialogConversation flavorDialog;
    [SerializeField] private DialogConversation tutorialDialog;
    [SerializeField] private DialogConversation endingDialog;


    public void Start()
    {
        if (openingDialog[Save.day] != null)
        {
            DialogManager.Instance.StartDialogSequence(openingDialog[Save.day], Save.day == 0 ? RunTutorial : AnnounceFlavors);
        }
        else
        {
            AnnounceFlavors();
        }
    }

    private void RunTutorial()
    {
        DialogManager.Instance.StartDialogSequence(tutorialDialog, AnnounceFlavors);
    }

    private void AnnounceFlavors()
    {
        if (Save.day == 0)
        {
            Save.majorFlavor = Flavor.UMAMI;
            Save.minorFlavor = Flavor.SOUR;
        }
        else
        {
            Save.majorFlavor = (Flavor)Random.Range(0, (int)Flavor.MAX);
            do
            {
                Save.minorFlavor = (Flavor)Random.Range(0, (int)Flavor.MAX);
            } while (Save.minorFlavor == Save.majorFlavor);
        }
        DialogManager.Instance.StartDialogSequence(flavorDialog);
    }

    public void OnProducePressed()
    {
        if (grocerDialog[Save.day] != null)
            DialogManager.Instance.StartDialogSequence(grocerDialog[Save.day], CompleteShopping);
        else
            CompleteShopping();
    }

    public void OnMeatPressed()
    {
        if (butcherDialog[Save.day] != null)
            DialogManager.Instance.StartDialogSequence(butcherDialog[Save.day], CompleteShopping);
        else
            CompleteShopping();
    }

    public void OnSpicePressed()
    {
        if (spiceDialog[Save.day] != null)
            DialogManager.Instance.StartDialogSequence(spiceDialog[Save.day], CompleteShopping);
        else
            CompleteShopping();
    }

    public void OnCarbPressed()
    {
        if (bakerDialog[Save.day] != null)
            DialogManager.Instance.StartDialogSequence(bakerDialog[Save.day], CompleteShopping);
        else
            CompleteShopping();
    }

    public void CompleteShopping()
    {
        DialogManager.Instance.StartDialogSequence(endingDialog, () =>
        {
            SceneManager.LoadScene("BattleScene");
        });
    }
}
