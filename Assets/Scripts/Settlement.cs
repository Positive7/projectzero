using UnityEngine;

public class Settlement : MonoBehaviour
{
    [Header("Population Setup")] public int maxPopulation;
    public                              int population;
    public                              int currentGuards;
    public                              int currentCivilian;

    protected virtual void Start()
    {
        population      = maxPopulation;
        currentGuards   = population / 3;
        currentCivilian = population - currentGuards;
    }
}