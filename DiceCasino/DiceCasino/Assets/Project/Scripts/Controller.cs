using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {Start, Bets, Game, Result}
public enum Winner { Player1, Player2, Draw } 

public class Controller : MonoBehaviour
{
    public List<Player> players;
    public static GameMode mode;
    public static Winner winner;
    public int diceNumberPerOneRoll;

    private float betsTimer;
    private float resultTimer; 
    private int rollCount = 0;

    private static Controller instance = null;

    public static Controller Instance
    {
        get
        {
            if (!instance) instance = new Controller();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (mode == GameMode.Game)
        {
            if (rollCount == 0)
            {
                players[0].Roll(diceNumberPerOneRoll);
                rollCount++;
            }
            else if (players[0].IsDiceStopped() && rollCount == 1)
            {
                players[1].Roll(diceNumberPerOneRoll);
                rollCount++;
            }
            else if (players[1].IsDiceStopped() && rollCount == 2)
            {
                ShowResult();                
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

    public void ShowResult()
    {
        HUD.Instance.AddNewResult(players[0].value, players[1].value);
        HUD.Instance.ShowResultWindow();
    }

    
}
