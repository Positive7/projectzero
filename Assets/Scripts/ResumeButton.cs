using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeButton : DefaultButton, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1.0f;
        NewGame.Instance.GameState = GameState.NewGame;
    }
}