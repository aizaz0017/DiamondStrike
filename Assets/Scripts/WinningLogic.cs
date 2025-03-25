using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WinningLogic : MonoBehaviour
{
    public int[][] paylines = new int[][]
    {
        new int[] { 0, 1, 2, 3, 4 }, // Straight middle line
        new int[] { 5, 6, 7, 8, 9 }, // Straight top line
        new int[] { 10, 11, 12, 13, 14 }, // Straight bottom line
        new int[] { 0, 6, 12, 8, 4 }, // V Shape
        new int[] { 10, 6, 2, 8, 14 }, // Inverted V
        new int[] {5, 1, 7, 13, 9},
        new int[] {5, 1, 7, 3, 9},
        new int[] {5, 11 , 7, 3, 9},
        new int[] {5, 11 , 7, 13, 9},
        new int[] {0, 6 ,2, 8, 4},
        new int[] {10, 6 ,12 ,8, 14},
    };

    public int winProbability;
    public string wildSymbolName = "Wild"; // Set your wild symbol name
    public string scatterSymbolName = "Scatter"; // Set your scatter symbol name

    public (int paylineIndex, string symbolName, int matchCount) CheckForWins(Sprite[] symbols, string[] symbolNames)
    {
        Debug.Log("Check for wins called");
        if (symbols == null || symbols.Length != 15 || symbolNames == null || symbolNames.Length != 15)
        {
            Debug.Log($"Symbols - {symbols.Length} -- names {symbolNames.Length}");
            Debug.LogError("Invalid symbols or symbolNames array.");
            return (-1, null, 0);
        }


        for (int i = 0; i < paylines.Length; i++)
        {
            int[] payline = paylines[i];
            string firstSymbolName = symbolNames[payline[0]];
            int matchCount = 1;

            List<GameObject> winningSymbols = new List<GameObject>();
            winningSymbols.Add(symbols[payline[0]].GameObject());

            for (int j = 1; j < payline.Length; j++)
            {
                if (symbolNames[payline[j]] == firstSymbolName || symbolNames[payline[j]] == wildSymbolName)
                {
                    matchCount++;
                    winningSymbols.Add(symbols[payline[j]].GameObject());
                }
                else if (symbolNames[payline[j]] != firstSymbolName && symbolNames[payline[j]] != wildSymbolName && symbolNames[payline[j]] != symbolNames[payline[0]])
                {
                    break;
                }
            }

            if (matchCount > 2)
            {
                return (i, firstSymbolName, matchCount);
            }
        }

        return (-1, null, 0);
    }
    
    //public List<(int paylineIndex, string symbolName, int matchCount)> CheckForAllWins(Sprite[] symbols, string[] symbolNames)
    //{
    //    if (symbols == null || symbols.Length != 15 || symbolNames == null || symbolNames.Length != 15)
    //    {
    //        Debug.LogError("Invalid symbols or symbolNames array.");
    //        return new List<(int, string, int)>();
    //    }

    //    List<(int paylineIndex, string symbolName, int matchCount)> winningLines = new List<(int, string, int)>();

    //    for (int i = 0; i < paylines.Length; i++)
    //    {
    //        int[] payline = paylines[i];
    //        string firstSymbolName = symbolNames[payline[0]];
    //        int matchCount = 1;

    //        for (int j = 1; j < payline.Length; j++)
    //        {
    //            if (symbolNames[payline[j]] == firstSymbolName || symbolNames[payline[j]] == wildSymbolName)
    //            {
    //                matchCount++;
    //            }
    //            else if (symbolNames[payline[j]] != firstSymbolName && symbolNames[payline[j]] != wildSymbolName && symbolNames[payline[j]] != symbolNames[payline[0]])
    //            {
    //                break;
    //            }
    //        }

    //        if (matchCount > 1)
    //        {
    //            winningLines.Add((i, firstSymbolName, matchCount));
    //        }
    //    }

    //    return winningLines;
    //}

    public int CheckForScatterWins(string[] symbolNames)
    {
        if (symbolNames == null || symbolNames.Length != 15)
        {
            Debug.LogError("Invalid symbolNames array.");
            return 0;
        }

        int scatterCount = 0;
        for (int i = 0; i < symbolNames.Length; i++)
        {
            if (symbolNames[i] == scatterSymbolName)
            {
                scatterCount++;
            }
        }
        return scatterCount;
    }
}
