using UnityEngine;
using UnityEngine.Events;
public class GameEvents : MonoBehaviour
{
    public static UnityAction SpinButtonClicked;
    public static UnityAction<Sprite, Sprite, Sprite> PlayerWins;
}
