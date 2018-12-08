using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu,     mainGame, pauseMenu, score, planetCamera;
    [SerializeField] private Image      healthImage,  staminaImage;
    [SerializeField] private TMP_Text   healthAmount, staminaAmount;

    GameObject                  player => PlanetManager.Instance.player;
    public static CanvasManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        switch (NewGame.Instance.GameState)
        {
            case GameState.MainMenu:
                Time.timeScale   = 1.0f;
                Cursor.lockState = CursorLockMode.None;
                if (player != null) player.SetActive(false);
                //planetCamera.SetActive(true);
                mainMenu.SetActive(true);
                pauseMenu.SetActive(false);
                score.SetActive(false);
                ScoreManager.Instance.totalKills                = 0;
                ScoreManager.Instance.totalSettlementsDestroyed = 0;
                ScoreManager.Instance.endTime                   = 0;
                ScoreManager.Instance.score                     = 0;
                planetCamera.GetComponent<SmoothFollow>().MoveToOriginalPosition();
                break;
            case GameState.NewGame:
                Cursor.lockState = CursorLockMode.Locked;
                if (player != null)
                {
                    player.SetActive(true);
                    player.GetComponent<PlayerController>().healthImage   = healthImage;
                    player.GetComponent<PlayerController>().healthAmount  = healthAmount;
                    player.GetComponent<PlayerController>().staminaAmount = staminaAmount;
                    player.GetComponent<PlayerController>().staminaImage  = staminaImage;
                    if (NewGame.Instance.previousState != GameState.NewGame) player.GetComponent<PlayerController>().Initialize();
                    planetCamera.GetComponent<SmoothFollow>().target = player.transform;
                }

                mainMenu.SetActive(false);
                mainGame.SetActive(true);
                pauseMenu.SetActive(false);
                score.SetActive(false);

                break;
            case GameState.PauseMenu:
                Cursor.lockState = CursorLockMode.None;
                if (player != null) player.SetActive(false);
                pauseMenu.SetActive(true);
                mainGame.SetActive(false);
                planetCamera.GetComponent<SmoothFollow>().MoveToOriginalPosition();
                break;
            case GameState.End:
                Cursor.lockState = CursorLockMode.None;
                if (player != null) player.SetActive(false);
                mainMenu.SetActive(false);
                mainGame.SetActive(false);
                score.SetActive(true);
                planetCamera.GetComponent<SmoothFollow>().MoveToOriginalPosition();
                break;
            case GameState.Null:
                Time.timeScale = 1.0f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (NewGame.Instance.GameState == GameState.NewGame)
            {
                NewGame.Instance.GameState = GameState.PauseMenu;
                Time.timeScale             = 0.0f;
            }
            else
            {
                NewGame.Instance.GameState = GameState.NewGame;
                Time.timeScale             = 1.0f;
            }
        }
    }
}