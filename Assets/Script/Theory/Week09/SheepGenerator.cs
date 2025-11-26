using System;
using UnityEngine;

public class SheepGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sheep watame = new Sheep(1);
        //Sheep bob = new Sheep(2);
        Sheep mike = new Sheep(3);
        Sheep foxtrot = new Sheep(4);

        Debug.Log("watame is number " + watame.AskNumber());
        Debug.Log("mike is number " + mike.sheepNumber);
        Debug.Log("foxtrot is number " + foxtrot.sheepNumber);
        Debug.Log("total Sheep Method" + Sheep.GetAllSheep());
        Debug.Log("total Sheep Field" + Sheep.totalSheepCount);
        Debug.Log("total Sheep Property" + Sheep.maxSheepCount);

        foxtrot.SetNumber(3);
        Debug.Log("foxtrot is number " + foxtrot.AskNumber());
        Sheep.RemoveSheep(1);

        int wool = FarmUtils.CalculateWoolCapacity(Sheep.totalSheepCount);
        Debug.Log("My farm woolcapacity is " + wool);
        Debug.Log("This Month day time is " + FarmUtils.DayTime);
    }
}
