using UnityEngine;
using TMPro;

using Assets.Scripts.Core;


public class OutputHubRequisites : MonoBehaviour
{
    [Header("Анимация появившихся реквизитов")]
    [SerializeField] private Animator _uiAnimator;
    [SerializeField] private string _popupPayeeTriggerName = "Payee";

    // Заменить структурой.
    [Header("Ссылки на тексты")]
    public TextMeshProUGUI TextBIC;
    public TextMeshProUGUI TextCorrespAcc;
    public TextMeshProUGUI TextPayeeAcc;
    public TextMeshProUGUI TextPayeeInn;
    public TextMeshProUGUI TextPayeeKpp;
    public TextMeshProUGUI TextPayeeBankName;
    public TextMeshProUGUI TextPayeePayeeName;

    void Start()
    {
        if (TextBIC == null || TextCorrespAcc == null || TextPayeeAcc == null || TextPayeeInn == null || 
            TextPayeeKpp == null || TextPayeeBankName == null || TextPayeePayeeName == null)
        {
            Debug.LogError("Text requisites dont initialized.");
        }

        // Cached version output
    }

    public void UpdateHub(PayeeDetails payeeDetails)
    {
        if (payeeDetails != null)
        {
            TextBIC.text = payeeDetails.payeeDetails.bankBic;
            TextCorrespAcc.text = payeeDetails.payeeDetails.correspAcc;
            TextPayeeAcc.text = payeeDetails.payeeDetails.payeeAcc;
            TextPayeeInn.text = payeeDetails.payeeDetails.payeeInn;
            TextPayeeKpp.text = payeeDetails.payeeDetails.payeeKpp;
            TextPayeeBankName.text = payeeDetails.payeeDetails.bankName;
            TextPayeePayeeName.text = payeeDetails.payeeDetails.payeeName;
        }
        else
        {
            TextBIC.text = "Ошибка.";
            TextCorrespAcc.text = "Ошибка.";
            TextPayeeAcc.text = "Ошибка.";
            TextPayeeInn.text = "Ошибка.";
            TextPayeeKpp.text = "Ошибка.";
            TextPayeeBankName.text = "Ошибка.";
            TextPayeePayeeName.text = "Ошибка.";
        }

        _uiAnimator.SetTrigger(_popupPayeeTriggerName);
    }

    public void ClearHub()
    {
        TextBIC.text = "";
        TextCorrespAcc.text = "";
        TextPayeeAcc.text = "";
        TextPayeeInn.text = "";
        TextPayeeKpp.text = "";
        TextPayeeBankName.text = "";
        TextPayeePayeeName.text = "";
    }
}
