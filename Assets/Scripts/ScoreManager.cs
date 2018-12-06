using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int   totalKills;
    public int   totalSettlementsDestroyed;
    public float endTime;

    [SerializeField] private TMP_Text kills;
    [SerializeField] private TMP_Text destroyed;
    [SerializeField] private TMP_Text end;

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        if (NewGame.Instance.GameState == GameState.NewGame)
            endTime += Time.deltaTime;
        if (NewGame.Instance.GameState == GameState.End)
        {
            kills.text     = $"Total Kills : {totalKills}";
            destroyed.text = $"Total Settlements Destroyed : {totalSettlementsDestroyed}";
            end    .text       = $"Total Time : {endTime:n2}";
        }
    }
}