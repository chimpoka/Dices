using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player// : MonoBehaviour
{
    public Vector3 force;

    [SerializeField]
    private GameObject dicePrefab;

    [SerializeField]
    private Transform spawnTransform;

   // [HideInInspector]
    public Vector3 spawnPosition { get { return spawnTransform.position; } }
    //[HideInInspector]
    public string diceName { get { return dicePrefab.name; } }

    public int value;


   
}
