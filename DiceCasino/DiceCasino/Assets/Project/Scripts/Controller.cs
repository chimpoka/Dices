using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {Start, Bets, Game, Result}
public enum Winner { Player1, Player2, Draw } 

public class Controller : MonoBehaviour
{
    public static List<Player> players;

    //public Transform spawnPoint1;
    //public Transform spawnPoint2;
    //public Vector3 force1;
   // public Vector3 force2;
    
    //public GameObject dicePrefab1;
    //public GameObject dicePrefab2;
    // public int rollTime;
    //public static int ValuePlayer1;
    //public static int ValuePlayer2;

    public static GameMode mode;
    public static Winner winner;

    private float betsTimer;
    private float resultTimer; 
    //private float delay = 0;
    //private int intDelay = 0;
    //private Vector3 spawnPosition1;
    //private Vector3 spawnPosition2;

    private int rollCount = 0;
    //private string diceName1;
    //private string diceName2;

    private void Start()
    {
        spawnPosition1 = players[0].spawnPosition;
        spawnPosition2 = spawnPoint2.position;

        diceName1 = dicePrefab1.name;
        diceName2 = dicePrefab2.name;
    }

    void Update()
    {

        //delay += Time.deltaTime;
        //intDelay = (int)Mathf.Floor(delay);
        if (mode == GameMode.Bets)
        {
            foreach (Player player in players)
                player.value = 0;
            
            ValuePlayer1 = 0;
            ValuePlayer2 = 0;
        }
        else if (mode == GameMode.Game)
        {
            ValuePlayer1 = Dice.Value(diceName1);
            ValuePlayer2 = Dice.Value(diceName2);

            if (rollCount == 0)
            {
                Dice.Roll(diceName1, spawnPosition1, force1);
                Dice.Roll(diceName1, spawnPosition1, force1);
                rollCount++;
            }
            else if (Dice.IsEndOfTurn(diceName1) && rollCount == 1)
            {
                Dice.Roll(diceName2, spawnPosition2, force2);
                Dice.Roll(diceName2, spawnPosition2, force2);
                rollCount++;
            }
            else if (Dice.IsEndOfTurn(diceName2) && rollCount == 2)
            {
                EndGame();                
                rollCount = 0;
            }           
        }
    }

    public static void FindWinner(int valuePlayer1, int valuePlayer2)
    {       
        if (valuePlayer1 > valuePlayer2)
            winner = Winner.Player1;
        else if (valuePlayer1 < valuePlayer2)
            winner = Winner.Player2;
        else if (valuePlayer1 == valuePlayer2)
            winner = Winner.Draw;
    }

    public void EndGame()
    {
        HUD.Instance.AddNewResult(ValuePlayer1, ValuePlayer2);
        HUD.Instance.ShowResultWindow();
    }

    
}
