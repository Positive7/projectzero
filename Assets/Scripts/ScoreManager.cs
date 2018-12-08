using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int   totalKills;
    public int   totalSettlementsDestroyed;
    public float endTime;

    public int score;

    [SerializeField] private TMP_Text summary;
    [SerializeField] private TMP_Text kills;
    [SerializeField] private TMP_Text destroyed;
    [SerializeField] private TMP_Text end;
    [SerializeField] private TMP_Text finalScore;
    [SerializeField] private TMP_Text inGameScore;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy() => StopAllCoroutines();

    public void Summary(string value = "You Win!")
    {
        summary.text = value;
    }

    public void ScoreAdd(int amount)
    {
        if (NewGame.Instance.GameState != GameState.NewGame) return;
        StartCoroutine(AddScore(amount));
    }

    private IEnumerator AddScore(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            score++;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Update()
    {
        if (NewGame.Instance.GameState == GameState.NewGame)
        {
            endTime          += Time.deltaTime;
            inGameScore.text =  $" Score : {score}";
        }

        if (NewGame.Instance.GameState == GameState.End)
        {
            kills.text      = $"Total Kills : {totalKills}";
            destroyed.text  = $"Total Settlements Destroyed : {totalSettlementsDestroyed}";
            end.text        = $"Total Time : {endTime:n2}";
            finalScore.text = $"Final Score : {score * totalKills * totalSettlementsDestroyed / (endTime / totalKills):n0}";
        }
    }
}