using TMPro;
using UnityEngine;

public class UiHandler : MonoBehaviour
{
    [SerializeField] TMP_Text balanceDisplay, resultDisplay, betAmountDisplay;


    public void UpdateBet(int betAmount)
    {
        betAmountDisplay.text = $"${betAmount}";
    }
    public void UpdateBalance(float balance)
    {
        balanceDisplay.text = $"${balance}";
    }
}
