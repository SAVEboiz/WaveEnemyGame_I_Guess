using System;
using UnityEngine;

public class Sheep 
{
    public int sheepNumber;
    public static int totalSheepCount;

    public static readonly int _initialPopulation = 5;
    static Sheep()
    {
        totalSheepCount = _initialPopulation;
    }

    public static int maxSheepCount 
    { 
        get { return totalSheepCount; } 
    }
    public Sheep(int i)
    {
        this.sheepNumber = i;
        totalSheepCount++;
        Debug.Log("Sheep Number " + i + " has been created");
    }
    public void SetNumber(int i)
    {
        sheepNumber = i;
    }

    public int AskNumber()
    {
        return sheepNumber;
    }
    public static int GetAllSheep()
    {
        return totalSheepCount;
    }
    public static void RemoveSheep(int count)
    {
        totalSheepCount -= count;
        Debug.Log("RemoveSheep totalSheep is " + totalSheepCount);
    }
    public void Jump()
    {
        Debug.Log("Sheep jump Sheep got gravity" + FarmUtils.Gravity);

    }

}
