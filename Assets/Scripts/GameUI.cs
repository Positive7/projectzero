using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using Fleeing = System.Collections.ObjectModel.ObservableCollection<CivilianMovement>;
using Cities = System.Collections.ObjectModel.ObservableCollection<City>;
using Population = System.Collections.ObjectModel.ObservableCollection<int>;

public class GameUI : MonoBehaviour
{
    public Fleeing    civilianMovements = new Fleeing();
    public Cities     cities            = new Cities();
    public Population population        = new Population();

    [SerializeField] private TMP_Text fleeingText, remainingTowns, remainingPopulation;

    [SerializeField] private int    maxPopulation;
    public static            GameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        civilianMovements = new Fleeing();
        population        = new Population();
        if (cities != null && cities.Count > 0)
        {
            foreach (var city in cities)
            {
                for (int i = 0; i < city.population; i++) { population.Add(i); }
            }

            maxPopulation            = population.Count;
            remainingPopulation.text = $"Remaining population : {maxPopulation} / {population.Count}";
            remainingTowns.text      = $"Remaining Towns : {cities.Count}";
        }

        fleeingText.text = $"Fleeing civilians : {civilianMovements.Count}";
    }

    private void Start()
    {
        cities.CollectionChanged            += OnCitiesChanged;
        civilianMovements.CollectionChanged += OnCivilianMovementsChanged;
        population.CollectionChanged        += OnPopulationChanged;
    }

    private void OnPopulationChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        remainingPopulation.text = $"Remaining population : {maxPopulation} / {population.Count}";
    }

    private void OnCitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        remainingTowns.text = $"Remaining Towns : {cities.Count}";
    }

    private void OnCivilianMovementsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        fleeingText.text = $"Fleeing civilians : {civilianMovements.Count}";
    }
}