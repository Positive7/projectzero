using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private int      population;
    [SerializeField] private TMP_Text remainingPopulation, remainingTowns, fleeingText;

    public List<Settlement>       cityManagers      = new List<Settlement>();
    public List<CivilianMovement> civilianMovements = new List<CivilianMovement>();

    public delegate void PopulationChanged(int i);

    public static event PopulationChanged OnPopulationChanged;

    public int Population
    {
        get => population;
        set
        {
            if (population == value) return;
            population = value;
            OnPopulationChanged?.Invoke(population);
        }
    }

    private void Start()
    {
        OnPopulationChanged += OnOnPopulationChanged;
        int cnt = 0;
        foreach (var c in cityManagers)
        {
            if (c == null || c.population <= 0) continue;
            for (int j = 0; j < c.population; j++) { cnt++; }
        }

        Population = cnt;
    }

    private void OnOnPopulationChanged(int i)
    {
        remainingPopulation.text = $"Remaining population : {population}";
        remainingTowns.text      = $"Remaining towns : {cityManagers.Count}";
        fleeingText.text         = $"Fleeing civilians : {civilianMovements.Count}";
    }
}