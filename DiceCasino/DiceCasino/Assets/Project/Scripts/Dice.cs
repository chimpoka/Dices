using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour
{	
    // Задержка перед броском после создания кубика
    public float rollSpeed = 0.25F;

    // Определяет, движется ли кубик или остановился, используя rigidBody.velocity и rigidBody.angularVelocity
    public static bool rolling = true;

    // Переменная для подсчета задержки перед броском через Time.deltaTime
    private float rollTime = 0;
	
	// Массив кубиков, готовых к броску
    private static ArrayList rollQueue = new ArrayList();
	// Массив всех кубиков, созданных через Dice.Roll()
	private static ArrayList allDice = new ArrayList();
	// Массив кубиков, которые уже брошены, но еще не остановились
    private static ArrayList rollingDice = new ArrayList();


		
	// Создает кубик из префаба
	public static GameObject prefab(string name, Vector3 position, Vector3 rotation, Vector3 scale) 
	{		
		// Загрузка префаба из ресурсов
        Object pf = Resources.Load("Prefabs/" + name);
		if (pf!=null)
		{
			GameObject inst = (GameObject) GameObject.Instantiate( pf , Vector3.zero, Quaternion.identity);
			if (inst!=null)
			{
				inst.transform.position = position;
				inst.transform.Rotate(rotation);
				inst.transform.localScale = scale;

				return inst;
			}
		}
		else
			Debug.Log("Prefab "+name+" not found!");
		return null;		
	}

    // Бросает кубик из точки spawnPoint c силой force
    public static void Roll(string dice, Vector3 spawnPoint, Vector3 force)
	{
        rolling = true;

        // Случайная точка создания кубика, +-1 юнит от spawnPoint
        spawnPoint.x = spawnPoint.x - 1 + Random.value * 2;		
		spawnPoint.y = spawnPoint.y - 1 + Random.value * 2;
        spawnPoint.z = spawnPoint.z - 1 + Random.value * 2;
		// Создаем кубик из префаба
        GameObject die = prefab(dice, spawnPoint, Vector3.zero, Vector3.one/*, mat*/);
		// Случайный вектор вращения
		die.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
        // Деактивируем объект. Но будет активорован после задержки, когда наступит его очередь
		die.SetActive(false);
        // Создаем экземпляр вспомогательного класса, который содержит всю информацию о данном кубике
        RollingDie rDie = new RollingDie(die, dice, spawnPoint, force);
		// Добавляем его в allDice
		allDice.Add(rDie);
        // И в очередь перед броском rollQueue
        rollQueue.Add(rDie);
	}

    // Сумма значений всех выброшенных кубиков (dieType = "") или сумма значений кубиков определенного типа (dieType)
    public static int Value(string dieType)
    {
        int v = 0;
        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie) allDice[d];
            if (rDie.name == dieType || dieType == "")
                v += rDie.die.value;
        }
        return v;
    }

    // Количество всех выброшенных кубиков или количество выброшенных кубиков определенного типа
    public static int Count(string dieType)
    {
        int v = 0;
        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie)allDice[d];
            if (rDie.name == dieType || dieType == "")
                v++;
        }
        return v;
    }

	// Удаление всех кубиков
    public static void Clear()
	{
		for (int d=0; d<allDice.Count; d++)
			GameObject.Destroy(((RollingDie)allDice[d]).gameObject);

        allDice.Clear();
        rollingDice.Clear();
        rollQueue.Clear();

        rolling = false;
	}

    

    void Update()
    {
        if (rolling)
        {
            // Считаем задержку перед броском
            rollTime += Time.deltaTime;
            // Если в очереди есть кубики и задержка перед броском прошла
            if (rollQueue.Count > 0 && rollTime > rollSpeed)
            {
				// Берем первый кубик из очереди
                RollingDie rDie = (RollingDie)rollQueue[0];
                GameObject die = rDie.gameObject;
				// Активируем его
				die.SetActive(true);
				// Применяем силу
                die.GetComponent<Rigidbody>().AddForce((Vector3) rDie.force, ForceMode.Impulse);
				// Применяем случайный крутящий момент
                die.GetComponent<Rigidbody>().AddTorque(new Vector3(-50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude), ForceMode.Impulse);
                // Добавляем кубик в rollingDice
                rollingDice.Add(rDie);
				// Удаляем кубик из очереди
                rollQueue.RemoveAt(0);
                // Обнуляем счетчик задержки для следующего кубика
                rollTime = 0;
            }
            else
                if (rollQueue.Count == 0)
                {
                // Если очередь пуста и движущихся кубиков нет, то rolling = false
                if (!IsRolling(""))
                        rolling = false;
                }
        }
    }

    // Определяет для всех кубиков или кубиков определенного типа, остановились ли они и имеют ли допустимое значение
    public static bool IsStopped(string dieType)
    {
        if (allDice.Count == 0)
            return false;

        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie)allDice[d];
            if ((!rDie.onGround || rDie.value == 0 || rDie.rolling) && (rDie.name == dieType || dieType == ""))
                return false;               
        }
        return true;
    }

    // Определяет для всех кубиков или кубиков определенного типа, движутся ли они
    public static bool IsRolling(string dieType)
    {
        int d = 0;
        while (d < rollingDice.Count)
        {
            // Если кубик не движется, то удалим его из rollingDice
            RollingDie rDie = (RollingDie)rollingDice[d];
            if (!rDie.rolling)
                rollingDice.Remove(rDie);
            else if (rDie.name == dieType || dieType == "")
                d++;
        }

        return (rollingDice.Count > 0);
    }
}


// Вспомогательный класс для хранения информации о кубике
class RollingDie
{

    public GameObject gameObject;
    public Die die;

    // dieType
    public string name = "";
   
    public Vector3 spawnPoint;
    public Vector3 force;

    public bool rolling { get { return die.rolling; } }

    public int value { get { return die.value; } }

    public bool onGround { get { return die.onGround; } }

	// Конструктор
    public RollingDie(GameObject gameObject, string name, Vector3 spawnPoint, Vector3 force)
    {
        this.gameObject = gameObject;
        this.name = name;
        this.spawnPoint = spawnPoint;
        this.force = force;
        die = (Die)gameObject.GetComponent(typeof(Die));
    }

	// Переброс кубика
    public void ReRoll()
    {
        if (name != "")
        {
            GameObject.Destroy(gameObject);
            Dice.Roll(name, spawnPoint, force);
        }
    }
}

