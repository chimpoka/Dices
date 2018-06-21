using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Максимальное число результатов в таблице
    public int maxResultsInTable = 10;
    // Ссылки на текстовые объекты, показывающие сумму значений выпавших кубиков
    public Text textValuePlayer1;
    public Text textValuePlayer2;
    //// Ссылки на окна для каждого из 4 режимов игры
    //public GameObject startWindow;
    //public GameObject betsWindow;
    //public GameObject resultWindow;
    //public GameObject mainWindow;
    // Ссылки на считываемые значения таймеров для ставок и результатов
    public InputField betsStartTimer;
    public InputField resultStartTimer;
    // Ссылки на устанавливаемые значения таймеров для ставок и результатов
    public Text betsTime;
    public Text resultTime;
    // Ссылка на таблицу результатов
    public Transform resultTable;
    // Аниматоры окон для каждого из 4 режимов игры
    public Animator startWindowAnimator;
    public Animator betsWindowAnimator;
    public Animator resultWindowAnimator;
    public Animator mainWindowAnimator;
    // Переменные для манипуляций с таймерами  
    private float betsTimerFloat;
    private float resultTimerFloat;

    // Создание синглтона
    private static HUD instance = null;

    public static HUD Instance
    {
        get
        {
            if (!instance) instance = new HUD();
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

    void Start ()
    {
        //startWindowAnimator = startWindow.GetComponent<Animator>();
        //betsWindowAnimator = betsWindow.GetComponent<Animator>();
        //resultWindowAnimator = resultWindow.GetComponent<Animator>();
        //mainWindowAnimator = mainWindow.GetComponent<Animator>();
        // Установка аниматоров окон на появление или исчезновение
        startWindowAnimator.SetBool("Enable", true);
        betsWindowAnimator.SetBool("Enable", false);
        resultWindowAnimator.SetBool("Enable", false);
        mainWindowAnimator.SetBool("Enable", false);

        Controller.mode = GameMode.Start;
    }

	void Update ()
    {
        // Вывод значений выпавших кубиков для двух игроков
        textValuePlayer1.text = Controller.Instance.players[0].value.ToString();
        textValuePlayer2.text = Controller.Instance.players[1].value.ToString();

        if (Controller.mode == GameMode.Bets)
        {
            // Если сейчас режим ставок, декрементируем значение таймера для ставок 
            betsTimerFloat -= Time.deltaTime;
            betsTime.text = Mathf.Floor(betsTimerFloat).ToString();
            if (betsTimerFloat < 1)
            {               
                // Если таймер вышел, начинаем игру
                ShowGameWindow();
            }
        }
        else if (Controller.mode == GameMode.Result)
        {
            // Если сейчас режим показа результатов, декрементируем значение таймера для показа результатов 
            resultTimerFloat -= Time.deltaTime;
            resultTime.text = Mathf.Floor(resultTimerFloat).ToString();
            if (resultTimerFloat< 1)
            {
                // Если таймер вышел,запускаем режим ставок
                ShowBetsWindow();
            }
        }
    }

    // Действие по нажатию кнопки Готово в стартовом меню
    public void onReadyClick()
    {
        // Если поля для ввода значений не пустые
        if (betsStartTimer.text != "" && resultStartTimer.text != "")
        {
            // Деактивируем стартовое окно и активируем окно игры (делается 1 раз при старте)
            startWindowAnimator.SetBool("Enable", false);
            mainWindowAnimator.SetBool("Enable", true);
            // Запускаем режим ставок
            ShowBetsWindow();
        }              
    }

    // Действие по окончании заполнения значения любого из таймеров в стартовом меню
    public void onEndEditTimer(InputField timer)
    {
        int value;
        // Если это не число или отрицательное число, то удаляем текст
        if (int.TryParse(timer.text, out value) == false || value < 0)
        {           
            timer.text = "";
        }
            
    }

    // Запуск режима ставок
    public void ShowBetsWindow()
    {
        // Удаление всех кубиков
        Dice.Clear();
        // Замена окна результатов на окно ставок
        betsWindowAnimator.SetBool("Enable", true);
        resultWindowAnimator.SetBool("Enable", false);
        // Установка режима ставок
        Controller.mode = GameMode.Bets;
        // Присваиваем таймеру стартовое значение
        betsTimerFloat = float.Parse(betsStartTimer.text);
    }

    // Запуск режима игры
    public void ShowGameWindow()
    {
        // Удаление окна ставок
        betsWindowAnimator.SetBool("Enable", false);
        //resultWindowAnimator.SetBool("Enable", false);
        // Установка режима игры
        Controller.mode = GameMode.Game;
    }

    // Запуск режима показа результатов
    public void ShowResultWindow()
    {
        // Показ окна результатов
        //betsWindowAnimator.SetBool("Enable", false);
        resultWindowAnimator.SetBool("Enable", true);
        // Установка режима игры
        Controller.mode = GameMode.Result;
        // Присваиваем таймеру стартовое значени
        resultTimerFloat = float.Parse(resultStartTimer.text);
    }

    // Добавление нового результата в таблицу результатов
    public void AddNewResult(int valuePlayer1, int valuePlayer2)
    {
        // Получаем объекты из таблицы результатов
        Transform[] elements = resultTable.GetComponentsInChildren<Transform>();
        
        foreach (Transform element in elements)
        {
            // Если в таблице уже maxResultsInTable элементов, то удаляем последний
            if (element.tag == "ResultElement" && element.GetSiblingIndex() == maxResultsInTable - 1)
                Destroy(element.gameObject);
        }

        // Находим победителя
        Controller.FindWinner(valuePlayer1, valuePlayer2);
        // Создаем элемент таблицы результатов из префаба и наследуем его от таблицы результатов
        GameObject result = Instantiate(Resources.Load("Prefabs/Result") as GameObject, resultTable);
        // Получаем объекты внутри элемента таблицы (значение игрока 1, значение игрока 2, иконка 1, иконка 2)
        Transform[] objects = result.GetComponentsInChildren<Transform>();
        foreach (Transform obj in objects)
        {
            // Записываем значения выпавших кубиков для первого и второго игрока в элемент таблицы
            if (obj.name == "ValuePlayer1")
                obj.GetComponent<Text>().text = valuePlayer1.ToString();
            else if (obj.name == "ValuePlayer2")
                obj.GetComponent<Text>().text = valuePlayer2.ToString();
            // Рисуем иконку победителя в зависимости от результатов раунда
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
        // Перемещаем элемент вверх на панели иерархии
        result.transform.SetAsFirstSibling();
    }

    public void onExitClick()
    {
        Application.Quit();
    }

}
