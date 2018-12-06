using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSettings : MonoBehaviour
{
    public        GameObject     planetSettings;
    public static PlanetSettings Instance;
    public        Slider         treesAmountSlider;
    public        TMP_Text       treesAmountText;

    public Slider   citiesAmountSlider;
    public TMP_Text citiesAmountText;


    public Slider   cityPopulationAmountSlider;
    public TMP_Text cityPopulationAmountText;
    public int      cityMaximumPopulation;

    public Slider   villagePopulationAmountSlider;
    public TMP_Text villagePopulationAmountText;
    public int      villageMaximumPopulation;

    private void Awake()
    {
        Instance = this;
    }

    private void SliderSetup(Slider slider, TMP_Text text, string str, Action<int> t)
    {
        text.text = str + (int) slider.value;
        slider.onValueChanged.AddListener(delegate(float value)
        {
            text.text = str + (int) value;
            t((int) value);
        });
    }


    private void Start()
    {
        SliderSetup(villagePopulationAmountSlider, villagePopulationAmountText, "Village population : ", x => villageMaximumPopulation                = x);
        SliderSetup(cityPopulationAmountSlider,    cityPopulationAmountText,    "Settlement population : ",    x => cityMaximumPopulation                   = x);
        SliderSetup(treesAmountSlider,             treesAmountText,             "Trees amount : ",       x => PlanetManager.Instance.maximumTrees  = x);
        SliderSetup(citiesAmountSlider,            citiesAmountText,            "Cities amount : ",      x => PlanetManager.Instance.maximumCities = x);
    }
}