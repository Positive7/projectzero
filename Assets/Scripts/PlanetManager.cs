using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using Fleeing = System.Collections.ObjectModel.ObservableCollection<CivilianMovement>;
using Cities = System.Collections.ObjectModel.ObservableCollection<Settlement>;
using Population = System.Collections.ObjectModel.ObservableCollection<int>;

public class PlanetManager : MonoBehaviour
{
    public Fleeing    civilianMovements = new Fleeing();
    public Cities     cityManagers      = new Cities();
    public Population population        = new Population();

    [SerializeField] private TMP_Text fleeingText, remainingTowns, remainingPopulation;

    public static                                PlanetManager Instance;
    [Header("Prefabs")] [SerializeField] private GameObject    planetPrefab;
    [SerializeField]                     private GameObject    playerPrefab;
    [SerializeField]                     private GameObject[]  trees;
    [SerializeField]                     private GameObject[]  cities;

    [Header("Setup")] public int maximumTrees;
    public                   int maximumCities;

    [HideInInspector] public GameObject  planet;
    [HideInInspector] public GameObject  player;
    public                   bool        generating;
    private                  IEnumerator spawnTrees, spawnCities, routineTree, routineCities;

    private void Awake() => Instance = this;

    public void OnCityDestroyed(int value)
    {
        for (int i = 0; i < value; i++)
            if (population.Count > 0)
            {
                population.Remove(population.LastOrDefault());
                ScoreManager.Instance.totalKills++;
            }
    }

    private void Initialize()
    {
        civilianMovements = new Fleeing();

        void OnCityManagersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            remainingTowns.text = cityManagers.Count == 1 || cityManagers.Count == 0
                                          ? $"Remaining Town : {cityManagers.Count}"
                                          : $"Remaining Towns : {cityManagers.Count}";
        }

        cityManagers.CollectionChanged += OnCityManagersOnCollectionChanged;

        void OnCivilianMovementsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) =>
                fleeingText.text = civilianMovements.Count == 1 || civilianMovements.Count == 0
                                           ? $"Fleeing civilian : {civilianMovements.Count}"
                                           : $"Fleeing civilians : {civilianMovements.Count}";

        civilianMovements.CollectionChanged += OnCivilianMovementsOnCollectionChanged;

        if (cityManagers != null && cityManagers.Count > 0)
        {
            population = new Population();

            void OnPopulationOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) =>
                    remainingPopulation.text = $"Remaining population : {population.Count}";

            population.CollectionChanged += OnPopulationOnCollectionChanged;
            for (int i = 0; i < cityManagers.Count; i++)
            {
                if (cityManagers[i]   != null && cityManagers[i].population > 0)
                    for (int j = 0; j < cityManagers[i].population; j++) { population.Add(i); }
            }

            remainingTowns.text = cityManagers.Count == 1 || cityManagers.Count == 0
                                          ? $"Remaining Town : {cityManagers.Count}"
                                          : $"Remaining Towns : {cityManagers.Count}";
            remainingPopulation.text = $"Remaining population : {population.Count}";
        }
        else { NewGame.Instance.GameState = GameState.End; }

        fleeingText.text = civilianMovements.Count == 1 || civilianMovements.Count == 0
                                   ? $"Fleeing civilian : {civilianMovements.Count}"
                                   : $"Fleeing civilians : {civilianMovements.Count}";
    }

    private void OnEnable()
    {
        OnCreate();
    }

    public void OnCreate()
    {
        StartCoroutine(Create());
    }

    public IEnumerator Create()
    {
        if (planet != null)
        {
            if (planet.transform.GetChild(0).childCount > 0)
            {
                for (int i = 0; i < planet.transform.GetChild(0).childCount; i++) { Destroy(planet.transform.GetChild(0).GetChild(i).gameObject); }
            }

            if (planet.transform.GetChild(1).childCount > 0)
            {
                for (int i = 0; i < planet.transform.GetChild(1).childCount; i++) { Destroy(planet.transform.GetChild(1).GetChild(i).gameObject); }
            }
        }

        if (planet == null) { yield return StartCoroutine(SpawnPlanet()); }

        if (spawnTrees  != null) StopCoroutine(spawnTrees);
        if (routineTree != null) StopCoroutine(routineTree);
        spawnTrees = SpawnTrees();
        yield return StartCoroutine(spawnTrees);
        if (spawnCities   != null) StopCoroutine(spawnCities);
        if (routineCities != null) StopCoroutine(routineCities);
        spawnCities = SpawnCities();
        yield return StartCoroutine(spawnCities);
        if (player == null) player = Instantiate(playerPrefab, Random.onUnitSphere * 26.5f, Quaternion.identity);
    }

    private IEnumerator SpawnPlanet()
    {
        planet = Instantiate(planetPrefab, transform.position, Quaternion.identity);
        StartCoroutine(Extensions.BoingLikeInterpolation(x => planet.transform.localScale = x, Vector3.one / 10, Vector3.one * 25.0f, 1.0f));
        yield return null;
    }

    private IEnumerator SpawnTrees()
    {
        for (int i = 0; i < maximumTrees; i++)
        {
            var pos  = Random.onUnitSphere * 25f;
            var tree = Instantiate(trees[Random.Range(0, trees.Length)], planet.transform.GetChild(0));
            tree.transform.localScale /= 25;
            tree.transform.up         =  pos - planet.transform.position;
            var time = Random.Range(0.5f, 1.0f);
            routineTree = Extensions.BoingLikeInterpolation(x =>
            {
                if (tree != null) tree.transform.position = x;
            }, pos * 5, pos, time);
            StartCoroutine(routineTree);
        }

        yield return null;
    }

    private IEnumerator SpawnCities()
    {
        cityManagers = new Cities();
        for (int i = 0; i < maximumCities; i++)
        {
            var pos  = Random.onUnitSphere * 25f;
            var city = Instantiate(cities[Random.Range(0, cities.Length)], planet.transform.GetChild(1));
            city.transform.localScale /= 25;
            city.transform.up         =  pos - planet.transform.position;
            var time = Random.Range(0.5f, 1.0f);
            routineCities = Extensions.BoingLikeInterpolation(x =>
            {
                if (city != null) city.transform.position = x;
            }, pos * 5, pos, time);
            StartCoroutine(routineCities);
            if (city.GetComponent<Village>()) city.GetComponent<Village>().maxPopulation = PlanetSettings.Instance.villageMaximumPopulation;
            if (city.GetComponent<City>()) city.GetComponent<City>().maxPopulation       = PlanetSettings.Instance.cityMaximumPopulation;
            cityManagers.Add(city.GetComponent<Settlement>());
        }

        yield return null;
        Initialize();
        yield return null;
    }

    private void Update()
    {
        if (NewGame.Instance.GameState != GameState.NewGame) return;
        if (population.Count <= 0 && cityManagers.Count <= 0 && civilianMovements.Count <= 0)
            NewGame.Instance.GameState = GameState.End;
    }
}