using UnityEngine.EventSystems;

public enum GameState
{
    Null, MainMenu, NewGame, PauseMenu, End
}

public class NewGameButton : DefaultButton, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        NewGame.Instance.GameState = GameState.NewGame;
    }
}