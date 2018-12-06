using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu,     mainGame, pauseMenu, score, planetCamera;
    [SerializeField] private Image      healthImage,  staminaImage;
    [SerializeField] private TMP_Text   healthAmount, staminaAmount;

    GameObject player => PlanetManager.Instance.player;
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
                Cursor.lockState = CursorLockMode.None;
                if (player != null) player.SetActive(false);
                planetCamera.SetActive(true);
                mainMenu.SetActive(true);
                pauseMenu.SetActive(false);
                score.SetActive(false);
                ScoreManager.Instance.totalKills                = 0;
                ScoreManager.Instance.totalSettlementsDestroyed = 0;
                ScoreManager.Instance.endTime                   = 0;
                ;
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
                    player.GetComponent<PlayerController>().Initialize();
                }

                planetCamera.SetActive(false);
                mainMenu.SetActive(false);
                mainGame.SetActive(true);
                pauseMenu.SetActive(false);
                score.SetActive(false);
                ;
                break;
            case GameState.PauseMenu:
                Cursor.lockState = CursorLockMode.None;
                planetCamera.SetActive(true);
                pauseMenu.SetActive(true);
                mainGame.SetActive(false);
                ;
                break;
            case GameState.End:
                Cursor.lockState = CursorLockMode.None;
                if (player != null) player.SetActive(false);
                planetCamera.SetActive(true);
                mainMenu.SetActive(false);
                mainGame.SetActive(false);
                score.SetActive(true);
                ;
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
                Time.timeScale             = 0;
            }
            else
            {
                NewGame.Instance.GameState = GameState.NewGame;
                Time.timeScale             = 1.0f;
            }
        }


    }
}