using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour
{	
    // �������� ����� ������� ����� �������� ������
    public float rollSpeed = 0.25F;

    // ����������, �������� �� ����� ��� �����������, ��������� rigidBody.velocity � rigidBody.angularVelocity
    public static bool rolling = true;

    // ���������� ��� �������� �������� ����� ������� ����� Time.deltaTime
    private float rollTime = 0;
	
	// ������ �������, ������� � ������
    private static ArrayList rollQueue = new ArrayList();
	// ������ ���� �������, ��������� ����� Dice.Roll()
	private static ArrayList allDice = new ArrayList();
	// ������ �������, ������� ��� �������, �� ��� �� ������������
    private static ArrayList rollingDice = new ArrayList();


		
	// ������� ����� �� �������
	public static GameObject prefab(string name, Vector3 position, Vector3 rotation, Vector3 scale) 
	{		
		// �������� ������� �� ��������
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

    // ������� ����� �� ����� spawnPoint c ����� force
    public static void Roll(string dice, Vector3 spawnPoint, Vector3 force)
	{
        rolling = true;

        // ��������� ����� �������� ������, +-1 ���� �� spawnPoint
        spawnPoint.x = spawnPoint.x - 1 + Random.value * 2;		
		spawnPoint.y = spawnPoint.y - 1 + Random.value * 2;
        spawnPoint.z = spawnPoint.z - 1 + Random.value * 2;
		// ������� ����� �� �������
        GameObject die = prefab(dice, spawnPoint, Vector3.zero, Vector3.one/*, mat*/);
		// ��������� ������ ��������
		die.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
        // ������������ ������. �� ����� ����������� ����� ��������, ����� �������� ��� �������
		die.SetActive(false);
        // ������� ��������� ���������������� ������, ������� �������� ��� ���������� � ������ ������
        RollingDie rDie = new RollingDie(die, dice, spawnPoint, force);
		// ��������� ��� � allDice
		allDice.Add(rDie);
        // � � ������� ����� ������� rollQueue
        rollQueue.Add(rDie);
	}

    // ����� �������� ���� ����������� ������� (dieType = "") ��� ����� �������� ������� ������������� ���� (dieType)
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

    // ���������� ���� ����������� ������� ��� ���������� ����������� ������� ������������� ����
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

	// �������� ���� �������
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
            // ������� �������� ����� �������
            rollTime += Time.deltaTime;
            // ���� � ������� ���� ������ � �������� ����� ������� ������
            if (rollQueue.Count > 0 && rollTime > rollSpeed)
            {
				// ����� ������ ����� �� �������
                RollingDie rDie = (RollingDie)rollQueue[0];
                GameObject die = rDie.gameObject;
				// ���������� ���
				die.SetActive(true);
				// ��������� ����
                die.GetComponent<Rigidbody>().AddForce((Vector3) rDie.force, ForceMode.Impulse);
				// ��������� ��������� �������� ������
                die.GetComponent<Rigidbody>().AddTorque(new Vector3(-50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude, -50 * Random.value * die.transform.localScale.magnitude), ForceMode.Impulse);
                // ��������� ����� � rollingDice
                rollingDice.Add(rDie);
				// ������� ����� �� �������
                rollQueue.RemoveAt(0);
                // �������� ������� �������� ��� ���������� ������
                rollTime = 0;
            }
            else
                if (rollQueue.Count == 0)
                {
                // ���� ������� ����� � ���������� ������� ���, �� rolling = false
                if (!IsRolling(""))
                        rolling = false;
                }
        }
    }

    // ���������� ��� ���� ������� ��� ������� ������������� ����, ������������ �� ��� � ����� �� ���������� ��������
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

    // ���������� ��� ���� ������� ��� ������� ������������� ����, �������� �� ���
    public static bool IsRolling(string dieType)
    {
        int d = 0;
        while (d < rollingDice.Count)
        {
            // ���� ����� �� ��������, �� ������ ��� �� rollingDice
            RollingDie rDie = (RollingDie)rollingDice[d];
            if (!rDie.rolling)
                rollingDice.Remove(rDie);
            else if (rDie.name == dieType || dieType == "")
                d++;
        }

        return (rollingDice.Count > 0);
    }
}


// ��������������� ����� ��� �������� ���������� � ������
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

	// �����������
    public RollingDie(GameObject gameObject, string name, Vector3 spawnPoint, Vector3 force)
    {
        this.gameObject = gameObject;
        this.name = name;
        this.spawnPoint = spawnPoint;
        this.force = force;
        die = (Die)gameObject.GetComponent(typeof(Die));
    }

	// �������� ������
    public void ReRoll()
    {
        if (name != "")
        {
            GameObject.Destroy(gameObject);
            Dice.Roll(name, spawnPoint, force);
        }
    }
}

