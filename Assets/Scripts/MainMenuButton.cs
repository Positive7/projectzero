using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : DefaultButton, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale             = 1.0f;
        NewGame.Instance.GameState = GameState.MainMenu;
        PlanetManager.Instance.OnCreate();
    }
}