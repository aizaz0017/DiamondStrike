using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting.Dependencies.Sqlite;

namespace SlotMachine
{ 
    public class GameEvents : MonoBehaviour
    {
        public static UnityAction SpinButtonClicked;
        public static UnityAction<Sprite , Sprite , Sprite > PlayerWins;
    }
}
