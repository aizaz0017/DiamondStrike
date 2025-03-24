using SlotMachine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    public ReelController reel1, reel2, reel3;
    public WinAnimation winAnimation;
    public UiHandler uiHandler;

    public TMP_Text balanceDisplay, resultDisplay, betAmountDisplay;
    public Button spinButton;

    [SerializeField] Button betIncreaseButton;
    [SerializeField] Button betDecreaseButton;
    [SerializeField] int betModifier;

    private int balance = 1000;
    private int betAmount = 10;

    private int freeSpinCounter = 0;
    private const int maxFreeSpins = 3;

    private Coroutine spinCoroutine;

    private void OnEnable()
    {
        UpdateBalance();
        uiHandler.UpdateBalance(balance);
        uiHandler.UpdateBet(betAmount);
        spinButton.onClick.AddListener(() => Spin(false));
        betIncreaseButton.onClick.AddListener(() => AdjustBet(betModifier));  // Increase bet
        betDecreaseButton.onClick.AddListener(() => AdjustBet(-betModifier)); // Decrease bet
    }

    private void OnDisable()
    {
        spinButton.onClick.RemoveListener(() => Spin(false));
        betIncreaseButton.onClick.RemoveListener(() => AdjustBet(betModifier));  // Increase bet
        betDecreaseButton.onClick.RemoveListener(() => AdjustBet(-betModifier)); // Decrease bet
    }

    public void Spin(bool freeSpin = false)
    {
        if (balance < betAmount)
        {
            resultDisplay.text = "Not enough balance!";
            return;
        }

        if(!freeSpin)
            balance -= betAmount;
        UpdateBalance();

        stopCoroutine(spinCoroutine);
        spinCoroutine = StartCoroutine(SpinReels());
    }

    private IEnumerator SpinReels()
    {
        spinButton.interactable = false;

        ReelController[] reels = { reel1, reel2, reel3 };
        foreach (var reel in reels) reel.StartSpin();

        resultDisplay.text = "Spinning...";

        // Wait until all reels stop spinning
        yield return new WaitUntil(() => reels.All(r => !r.IsSpinning()));

        yield return new WaitForSeconds(0.5f);

        CheckWin();
    }

    void stopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            if (spinCoroutine != null)
                StopCoroutine(coroutine);
        }
    }
    private void CheckWin()
    {
        // Retrieve symbols and sprites
        Sprite[] symbols = { reel1.GetMiddleSymbol(), reel2.GetMiddleSymbol(), reel3.GetMiddleSymbol() };
        string[] symbolNames = { symbols[0].name, symbols[1].name, symbols[2].name };

        int winnings = 0;
        int scatterCount = 0;
        string winReason = "";

        bool[] isWild = { symbolNames[0].Contains("Wild"), symbolNames[1].Contains("Wild"), symbolNames[2].Contains("Wild") };
        bool[] isScatter = { symbolNames[0].Contains("Scatter"), symbolNames[1].Contains("Scatter"), symbolNames[2].Contains("Scatter") };

        // Count scatter symbols
        for (int i = 0; i < isScatter.Length; i++)
        {
            if (isScatter[i]) scatterCount++;
        }

        // Scatter Wins
        if (scatterCount == 2)
        {
            winnings += betAmount * 2;
            winReason = "Dual scatter!";
        }
        else if (scatterCount == 3)
        {
            winnings += betAmount * 5;
            winReason = "Triple scatter! Free spin !";
            StartCoroutine(FreeSpin());
        }

        // Regular Line Wins
        if (symbolNames[0] == symbolNames[1] && symbolNames[1] == symbolNames[2])
        {
            winnings += betAmount * 5;
            winReason = $"Three matching : {symbolNames[0]}";
        }
        else if (isWild[0] && symbolNames[1] == symbolNames[2] ||
                 isWild[1] && symbolNames[0] == symbolNames[2] ||
                 isWild[2] && symbolNames[0] == symbolNames[1])
        {
            winnings += betAmount * 5;
            winReason = $"Wild + two matching!";
        }
        // Partial Wins
        else if (symbolNames[0] == symbolNames[1] || symbolNames[1] == symbolNames[2])
        {
            winnings += betAmount * 2;
            winReason = $"Partial win with {symbolNames[1]}!";
        }
        // Double Wild Bonus
        else if ((isWild[0] && isWild[1]) || (isWild[1] && isWild[2]))
        {
            winnings += betAmount * 3;
            winReason = "Double wild boost!";
        }

        // Handle winnings
        if (winnings > 0)
        {
            GameEvents.PlayerWins(symbols[0], symbols[1], symbols[2]);
            Delay(3.5f, nameof(EnableSpinButton));
        }
        else
        {
            spinButton.interactable = true;
        }

        balance += winnings;
        UpdateBalance();
        resultDisplay.text = winnings > 0 ? $"You won: ${winnings}! {winReason}" : "Try again!";
    }

    private void Delay(float delay, string methodName) 
    { 
        Invoke(methodName, delay); 
    }

    private void EnableSpinButton()
    {
        spinButton.interactable = true;
    }

    private IEnumerator FreeSpin()
    {
        if (freeSpinCounter >= maxFreeSpins)
        {
            resultDisplay.text = "No more free spins!";
            yield break;
        }

        freeSpinCounter++;
        resultDisplay.text = "Free Spin Awarded!";
        yield return new WaitForSeconds(1.5f);
        Spin(true);
    }
    public void AdjustBet(int amount)
    {
        int newBet = betAmount + amount;
        if (newBet >= 10)  // Ensure minimum bet is 10
        {
            betAmount = newBet;
            uiHandler.UpdateBet(betAmount);
        }
    }

    private void UpdateBalance()
    {
        uiHandler.UpdateBalance(balance);
    }

}
