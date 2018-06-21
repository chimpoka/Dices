using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player// : MonoBehaviour
{
    // Ссылка на префаб кубика
    [SerializeField]
    private GameObject dicePrefab;
    // Ссылка на точку появления кубиков
    [SerializeField]
    private Transform spawnTransform;
    // Ссылка на вектор силы, с которой нужно бросить кубики
    public Vector3 force;

    // Позиция точки появления кубиков, получаемая из spawnTransform
    public Vector3 spawnPosition { get { return spawnTransform.position; } }
    // Имя префаба кубика
    public string diceName { get { return dicePrefab.name; } }
    // Сумма значений кубиков, брошенных игроком
    public int value { get { return Dice.Value(diceName); } }


    // Бросить diceCount кубиков
    public void Roll(int diceCount)
    {
        for (int i = 0; i < diceCount; i++)
            Dice.Roll(diceName, spawnPosition, force);
    }

    // Завершил ли игрок свой ход
    public bool IsEndTurn()
    {
        return (Dice.IsStopped(diceName));
    }
   
}
