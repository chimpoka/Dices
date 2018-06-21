using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player// : MonoBehaviour
{
    [SerializeField]
    private GameObject dicePrefab;

    [SerializeField]
    private Transform spawnTransform;

    public Vector3 force;

    public Vector3 spawnPosition { get { return spawnTransform.position; } }

    public string diceName { get { return dicePrefab.name; } }

    public int value
    {
        get
        {
            return Dice.Value(diceName);
        }
    }


    //------------------------------------------------------------------------------------------------------------------------------
    // public methods
    //------------------------------------------------------------------------------------------------------------------------------	

    public void Roll(int diceCount)
    {
        for (int i = 0; i < diceCount; i++)
            Dice.Roll(diceName, spawnPosition, force);
    }

    public bool IsDiceStopped()
    {
        return Dice.IsDiceStopped(diceName);
    }
   
}
