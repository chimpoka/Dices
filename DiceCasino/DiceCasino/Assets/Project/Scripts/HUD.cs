using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD instance = null;

    public static HUD Instance
    {
        get
        {
            if (!instance) instance = new HUD();
            return instance;
        }
    }

    public int maxResultsInTable = 10;

    public Text textValuePlayer1;
    public Text textValuePlayer2;

    public GameObject startWindow;
    public GameObject betsWindow;
    public GameObject resultWindow;
    public GameObject mainWindow;

    public InputField betsStartTimer;
    public InputField resultStartTimer;
   // public Text betsStartTimer;
   //  public Text resultStartTimer;
    public Text betsTime;
    public Text resultTime;

    public Transform resultTable;

    private Animator startWindowAnimator;
    private Animator betsWindowAnimator;
    private Animator resultWindowAnimator;
    private Animator mainWindowAnimator;

    private float betsTimerFloat;
    private float resultTimerFloat;

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

    void Start ()
    {
        startWindowAnimator = startWindow.GetComponent<Animator>();
        betsWindowAnimator = betsWindow.GetComponent<Animator>();
        resultWindowAnimator = resultWindow.GetComponent<Animator>();
        mainWindowAnimator = mainWindow.GetComponent<Animator>();

        startWindowAnimator.SetBool("Enable", true);
        betsWindowAnimator.SetBool("Enable", false);
        resultWindowAnimator.SetBool("Enable", false);
        mainWindowAnimator.SetBool("Enable", false);

        Controller.mode = GameMode.Start;
    }

	void Update ()
    {
        textValuePlayer1.text = Controller.Instance.players[0].value.ToString();
        textValuePlayer2.text = Controller.Instance.players[1].value.ToString();

        if (Controller.mode == GameMode.Bets)
        {
            betsTimerFloat -= Time.deltaTime;
            betsTime.text = Mathf.Floor(betsTimerFloat).ToString();
            if (betsTimerFloat < 1)
            {               
                ShowGameWindow();
            }
        }
        else if (Controller.mode == GameMode.Game)
        {
            
        }
        else if (Controller.mode == GameMode.Result)
        {
            resultTimerFloat -= Time.deltaTime;
            resultTime.text = Mathf.Floor(resultTimerFloat).ToString();
            if (resultTimerFloat< 1)
            {
                ShowBetsWindow();
            }
        }
    }

    public void onReadyClick()
    {
        if (betsStartTimer.text != "" && resultStartTimer.text != "")
        {
            startWindowAnimator.SetBool("Enable", false);
            mainWindowAnimator.SetBool("Enable", true);
            ShowBetsWindow();
        }              
    }

    public void onEndEditTimer(InputField timer)
    {
        int value;
        if (int.TryParse(timer.text, out value) == false || value < 0)
        {           
            timer.text = "";
        }
            
    }

    public void ShowBetsWindow()
    {
        Dice.Clear();
        betsWindowAnimator.SetBool("Enable", true);
        resultWindowAnimator.SetBool("Enable", false);
        Controller.mode = GameMode.Bets;
        betsTimerFloat = float.Parse(betsStartTimer.text);
    }

    public void ShowGameWindow()
    {
        betsWindowAnimator.SetBool("Enable", false);
        resultWindowAnimator.SetBool("Enable", false);
        Controller.mode = GameMode.Game;
    }

    public void ShowResultWindow()
    {
        betsWindowAnimator.SetBool("Enable", false);
        resultWindowAnimator.SetBool("Enable", true);
        Controller.mode = GameMode.Result;
        resultTimerFloat = float.Parse(resultStartTimer.text);
    }


    public void AddNewResult(int valuePlayer1, int valuePlayer2)
    {
        Transform[] elements = resultTable.GetComponentsInChildren<Transform>();
        foreach (Transform element in elements)
        {
            if (element.GetSiblingIndex() == maxResultsInTable - 1)
                Destroy(element.gameObject);
        }


        Controller.FindWinner(valuePlayer1, valuePlayer2);

        GameObject result = Instantiate(Resources.Load("Prefabs/Result") as GameObject, resultTable);

        Transform[] objects = result.GetComponentsInChildren<Transform>();
        foreach (Transform obj in objects)
        {
            if (obj.name == "ValuePlayer1")
                obj.GetComponent<Text>().text = valuePlayer1.ToString();
            else if (obj.name == "ValuePlayer2")
                obj.GetComponent<Text>().text = valuePlayer2.ToString();
            else if (obj.name == "IconWinPlayer1")
            {
                if (Controller.winner == Winner.Player1)
                    obj.GetComponent<Image>().enabled = true;
                else
                    obj.GetComponent<Image>().enabled = false;
            }
            else if (obj.name == "IconWinPlayer2")
            {
                if (Controller.winner == Winner.Player2)
                    obj.GetComponent<Image>().enabled = true;
                else
                    obj.GetComponent<Image>().enabled = false;
            }
        }

      //  result.transform.parent = resultTable;
        result.transform.SetAsFirstSibling();
        
    }



}
