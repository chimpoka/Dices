using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour {	
	


    // Задержка перед броском после создания кубика
    public float rollSpeed = 0.25F;

    // Определяет, движется ли кубик или остановился, используя rigidBody.velocity и rigidBody.angularVelocity
    public static bool rolling = true;

    // Переменная для подсчета задержки через Time.deltaTime
    private float rollTime = 0;
	
	// Массив кубиков, готовых к броску
    private static ArrayList rollQueue = new ArrayList();
	// Массив всех кубиков, созданных через Dice.Roll()
	private static ArrayList allDice = new ArrayList();
	// Массив кубиков, которые уже брошены, но еще не остановились
    private static ArrayList rollingDice = new ArrayList();


		
	// Создает кубик из префаба
	public static GameObject prefab(string name, Vector3 position, Vector3 rotation, Vector3 scale/*, string mat*/) 
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
	
	/// <summary>
	/// Roll one or more dice with a specific material from a spawnPoint and give it a specific force.
	/// format dice 			: 	({count}){die type}	, exmpl.  d6, 4d4, 12d8 , 1d20
	/// possible die types 	:	d4, d6, d8 , d10, d12, d20
	/// </summary>
	public static void Roll(string dice, /*string mat, */Vector3 spawnPoint, Vector3 force)
	{
        rolling = true;
		// sotring dice to lowercase for comparing purposes
		//dice = dice.ToLower();				
		//int count = 1;
		//string dieType = "d6";
		
		//// 'd' must be present for a valid 'dice' specification
		//int p = dice.IndexOf("d");
		//if (p>=0)
		//{
		//	// check if dice starts with d, if true a single die is rolled.
		//	// dice must have a count because dice does not start with 'd'
		//	if (p>0)
		//	{
		//		// extract count
		//		string[] a = dice.Split('d');
		//		count = System.Convert.ToInt32(a[0]);
		//		// get die type
		//		if (a.Length>1)
		//			dieType = "d"+a[1];
		//		else
		//			dieType = "d6";
		//	}
		//	else
		//		dieType = dice;
			
			// instantiate the dice
			//for (int d=0; d<count; d++)
			//{
				// randomize spawnPoint variation
				spawnPoint.x = spawnPoint.x - 1 + Random.value * 2;		
				spawnPoint.y = spawnPoint.y - 1 + Random.value * 2;
                spawnPoint.z = spawnPoint.z - 1 + Random.value * 2;
				// create the die prefab/gameObject
                GameObject die = prefab(dice, spawnPoint, Vector3.zero, Vector3.one/*, mat*/);
				// give it a random rotation
				die.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
				// inactivate this gameObject because activating it will be handeled using the rollQueue and at the apropriate time
				die.SetActive(false);
				// create RollingDie class that will hold things like spawnpoint and force, to be used when activating the die at a later stage
                RollingDie rDie = new RollingDie(die, dice, /*mat,*/ spawnPoint, force);
				// add RollingDie to allDices
				allDice.Add(rDie);               
				// add RollingDie to the rolling queue
                rollQueue.Add(rDie);
			//}
		//}
	}

	/// <summary>
	/// Get value of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
    public static int Value(string dieType)
    {
        int v = 0;
		// loop all dice
        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie) allDice[d];
			// check the type
            if (rDie.name == dieType || dieType == "")
                v += rDie.die.value;
        }
        return v;
    }

    public static bool IsDiceStopped(string dieType)
    {
        if (allDice.Count == 0)
            return false;

        bool dieExist = false;

        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie)allDice[d];
            if (rDie.name == dieType)
                dieExist = true;
        }

        if (dieExist == false)
            return false;

        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie)allDice[d];
            if (rDie.name == dieType && rDie.value == 0)
                return false;
            else if (dieType == "" && rDie.value == 0)
                return false;
        }
        return true;
    }

    //public static void Reroll(string dieType)
    //{
    //    for (int d = 0; d < allDice.Count; d++)
    //    {
    //        RollingDie rDie = (RollingDie)allDice[d];
    //        // check the type
    //        if (rDie.name == dieType)
    //        {
    //            allDice.Remove(rDie);
    //            rDie.ReRoll();
    //        }
               
    //        else if (dieType == "")
    //        {
    //            allDice.Remove(rDie);
    //            rDie.ReRoll();
    //        }
                
    //    }
    //}

    //public static bool IsInvalidValue(string dieType)
    //{
    //    for (int d = 0; d < allDice.Count; d++)
    //    {
    //        RollingDie rDie = (RollingDie)allDice[d];
    //        // check the type

    //        if (rDie.onGround && !rDie.rolling && rDie.value == 0)
    //        {
    //            if (rDie.name == dieType)
    //                return true;
    //            else if (dieType == "")
    //                return true;
    //        }
    //    }
    //    return false;
    //}

	/// <summary>
	/// Get number of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
    public static int Count(string dieType)
    {
        int v = 0;
		// loop all dice
        for (int d = 0; d < allDice.Count; d++)
        {
            RollingDie rDie = (RollingDie)allDice[d];
			// check the type
            if (rDie.name == dieType || dieType == "")
                v++;
        }
        return v;
    }

	/// <summary>
	/// Get rolling status of all ( dieType = "" ) dice or dieType specific dice.
	/// </summary>
    public static string AsString(string dieType)
    {
		// count the dice
        string v = ""+Count(dieType);
        if (dieType == "")
            v += " dice | ";
        else
            v += dieType + " : ";
		
        if (dieType == "")
        {
			// no dieType specified to cumulate values per dieType ( if they are available )
            if (Count("d6") > 0) v += AsString("d6") + " | ";
            if (Count("d10") > 0) v += AsString("d10") + " | ";
        }
        else
        {
			// assemble status of specific dieType
            bool hasValue = false;
            for (int d = 0; d < allDice.Count; d++)
            {
                RollingDie rDie = (RollingDie)allDice[d];
				// check type
                if (rDie.name == dieType || dieType == "")
                {
                    if (hasValue) v += " + ";
					// if the value of the die is 0 , no value could be determined
					// this could be because the die is rolling or is in a invalid position
                    v += "" + ((rDie.die.value == 0) ? "?" : "" + rDie.die.value);
                    hasValue = true;
                }
            }
            v += " = " + Value(dieType);
        }
        return v;
    }


	/// <summary>
	/// Clears all currently rolling dice
	/// </summary>
    public static void Clear()
	{
		for (int d=0; d<allDice.Count; d++)
			GameObject.Destroy(((RollingDie)allDice[d]).gameObject);

        allDice.Clear();
        rollingDice.Clear();
        rollQueue.Clear();

        rolling = false;
	}

    

	/// <summary>
	/// Update is called once per frame
	/// </summary>
    void Update()
    {
        if (rolling)
        {
			// there are dice rolling so increment rolling time
            rollTime += Time.deltaTime;
			// check rollTime against rollSpeed to determine if a die should be activated ( if one available in the rolling  queue )
            if (rollQueue.Count > 0 && rollTime > rollSpeed)
            {
				// get die from rolling queue
                RollingDie rDie = (RollingDie)rollQueue[0];
                GameObject die = rDie.gameObject;
				// activate the gameObject
				die.SetActive(true);
				// apply the force impuls
                die.GetComponent<Rigidbody>().AddForce((Vector3) rDie.force, ForceMode.Impulse);
				// apply a random torque
                die.GetComponent<Rigidbody>().AddTorque(new Vector3(-50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude), ForceMode.Impulse);
				// add die to rollingDice
                rollingDice.Add(rDie);
				// remove the die from the queue
                rollQueue.RemoveAt(0);
				// reset rollTime so we can check when the next die has to be rolled
                rollTime = 0;
            }
            else
                if (rollQueue.Count == 0)
                {
					// roll queue is empty so if no dice are rolling we can set the rolling attribute to false
                    if (!IsRolling())
                        rolling = false;
                }
        }
    }

	/// <summary>
	/// Check if there all dice have stopped rolling
	/// </summary>
    private bool IsRolling()
    {
        int d = 0;
		// loop rollingDice
        while (d < rollingDice.Count)
        {
			// if rolling die no longer rolling , remove it from rollingDice
            RollingDie rDie = (RollingDie)rollingDice[d];
            if (!rDie.rolling)
                rollingDice.Remove(rDie);
            else
                d++;
        }
		// return false if we have no rolling dice 
        return (rollingDice.Count > 0);
    }
}

/// <summary>
/// Supporting rolling die class to keep die information
/// </summary>
class RollingDie
{

    public GameObject gameObject;		// associated gameObject
    public Die die;								// associated Die (value calculation) script

    public string name = "";				// dieType
    //public string mat;						// die material (asString)
    public Vector3 spawnPoint;			// die spawnPoiunt
    public Vector3 force;					// die initial force impuls

	// rolling attribute specifies if this die is still rolling
    public bool rolling
    {
        get
        {
            return die.rolling;
        }
    }

    public int value
    {
        get
        {
            return die.value;
        }
    }

    public bool onGround
    {
        get
        {
            return die.onGround;
        }
    }

	// constructor
    public RollingDie(GameObject gameObject, string name,/* string mat,*/ Vector3 spawnPoint, Vector3 force)
    {
        this.gameObject = gameObject;
        this.name = name;
        //this.mat = mat;
        this.spawnPoint = spawnPoint;
        this.force = force;
		// get Die script of current gameObject
        die = (Die)gameObject.GetComponent(typeof(Die));
    }

	// ReRoll this specific die
    public void ReRoll()
    {
        if (name != "")
        {
            GameObject.Destroy(gameObject);
            Dice.Roll(name,/* mat,*/ spawnPoint, force);
        }
    }
}

