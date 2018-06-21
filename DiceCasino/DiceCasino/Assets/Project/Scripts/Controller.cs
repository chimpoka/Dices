using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {Start, Bets, Game, Result}
public enum Winner { Player1, Player2, Draw } 

public class Controller : MonoBehaviour
{
    // Список игроков (необходимо в редакторе заполнить параметры: Dice Prefab, Spawn Transform, Force)
    public List<Player> players;
    // Стадия игры (Стартовое окно, таймер ставок, сама игра, таймер результатов)
    public static GameMode mode;
    // Победитель (игрок 1, игрок 2, ничья)
    public static Winner winner;
    // Количество бросаемых кубиков за 1 бросок
    public int diceNumberPerOneRoll = 2;

    // Количество сделанных бросков кубиков за раунд
    private int rollCount = 0;

    // Создание синглтона
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
                // Если еще не был совершен бросок, то первый игрок бросает diceNumberPerOneRoll кубиков
                players[0].Roll(diceNumberPerOneRoll);
                rollCount++;
            }
            else if (players[0].IsEndTurn() && rollCount == 1)
            {
                // Если совершен 1 бросок, то второй игрок бросает diceNumberPerOneRoll кубиков
                players[1].Roll(diceNumberPerOneRoll);
                rollCount++;
            }
            else if (players[1].IsEndTurn() && rollCount == 2)
            {
                // Если совершено 2 броска, то показываем результаты
                ShowResult();                
                rollCount = 0;
            }           
        }
    }

    // Определение побелителя
    public static void FindWinner(int valuePlayer1, int valuePlayer2)
    {       
        if (valuePlayer1 > valuePlayer2)
            winner = Winner.Player1;
        else if (valuePlayer1 < valuePlayer2)
            winner = Winner.Player2;
        else if (valuePlayer1 == valuePlayer2)
            winner = Winner.Draw;
    }

    // Показ результатов
    public void ShowResult()
    {
        // Добавление результатов в таблицу
        HUD.Instance.AddNewResult(players[0].value, players[1].value);
        // Показ таймера после игры
        HUD.Instance.ShowResultWindow();
    }

    
}
