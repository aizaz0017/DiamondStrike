using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    public List<ReelController> reels;
    public WinAnimation winAnimation;
    public UiHandler uiHandler;
    public WinningLogic winningLogic;

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

        foreach (var reel in reels) reel.StartSpin();

        resultDisplay.text = "Spinning...";

        // Wait until all reels stop spinning
        yield return new WaitUntil(() => reels.All(r => !r.IsSpinning()));

        yield return new WaitForSeconds(0.5f);
        spinButton.interactable = true;
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
        Sprite[] symbols = new Sprite[reels.Count * 3]; // 5 reels x 3 symbols

        // Extract symbols in horizontal order.
        for (int row = 0; row < 3; row++) // Top, Middle, Bottom rows
        {
            for (int reelIndex = 0; reelIndex < reels.Count; reelIndex++)
            {
                symbols[row * reels.Count + reelIndex] = reels[reelIndex].GetAllSymbols()[row];
            }
        }

        // Extract symbol names.
        string[] symbolNames = new string[symbols.Length];
        for (int i = 0; i < symbols.Length; i++)
        {
            symbolNames[i] = symbols[i].name;
        }

        //int winnings = 0;
        //int scatterCount = 0;
        //string winReason = "";

        int winnings = 0;
        string winReason = "";
        int scatterCount = 0;
        int scatterPayout = 10; // Example scatter win multiplier
        int wildMultiplier = 2; // Wild symbol payout boost

        bool[] isWild = { symbolNames[0].Contains("Wild"), symbolNames[1].Contains("Wild"), symbolNames[2].Contains("Wild") };
        bool[] isScatter = { symbolNames[0].Contains("Scatter"), symbolNames[1].Contains("Scatter"), symbolNames[2].Contains("Scatter") };

        (int winningPayline, string winningSymbol, int matchCount) winResult = winningLogic.CheckForWins(symbols, symbolNames);
       int scatterWinsCount =  winningLogic.CheckForScatterWins(symbolNames);
        winnings = 3 * winResult.matchCount;
        //// Count scatter symbols
        //for (int i = 0; i < isScatter.Length; i++)
        //{
        //    if (isScatter[i]) scatterCount++;
        //}

        //// Scatter Wins
        //if (scatterCount == 2)
        //{
        //    winnings += betAmount * 2;
        //    winReason = "Dual scatter!";
        //}
        //else if (scatterCount == 3)
        //{
        //    winnings += betAmount * 5;
        //    winReason = "Triple scatter! Free spin !";
        //    StartCoroutine(FreeSpin());
        //}

        //// Regular Line Wins
        //if (symbolNames[0] == symbolNames[1] && symbolNames[1] == symbolNames[2])
        //{
        //    winnings += betAmount * 5;
        //    winReason = $"Three matching : {symbolNames[0]}";
        //}
        //else if (isWild[0] && symbolNames[1] == symbolNames[2] ||
        //         isWild[1] && symbolNames[0] == symbolNames[2] ||
        //         isWild[2] && symbolNames[0] == symbolNames[1])
        //{
        //    winnings += betAmount * 5;
        //    winReason = $"Wild + two matching!";
        //}
        //// Partial Wins
        //else if (symbolNames[0] == symbolNames[1] || symbolNames[1] == symbolNames[2])
        //{
        //    winnings += betAmount * 2;
        //    winReason = $"Partial win with {symbolNames[1]}!";
        //}
        //// Double Wild Bonus
        //else if ((isWild[0] && isWild[1]) || (isWild[1] && isWild[2]))
        //{
        //    winnings += betAmount * 3;
        //    winReason = "Double wild boost!";
        //}

        //// Handle winnings
        //if (winnings > 0)
        //{
        //    GameEvents.PlayerWins(symbols[0], symbols[1], symbols[2]);
        //    Delay(3.5f, nameof(EnableSpinButton));
        //}
        //else
        //{
        //    spinButton.interactable = true;
        //}
        
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
