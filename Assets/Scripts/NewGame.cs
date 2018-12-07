using UnityEngine;

public class NewGame : MonoBehaviour
{
    public static            NewGame   Instance;
    [SerializeField] private GameState gameState;

    public GameState GameState
    {
        get => gameState;
        set
        {
            if (!Equals(gameState, value))
            {
                gameState = value;
                CanvasManager.Instance.Initialize();
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameState = GameState.MainMenu;
        Time.timeScale = 1.0f;
    }
}