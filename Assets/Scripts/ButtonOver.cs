using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup       = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
    }

    public void OnPointerEnter(PointerEventData eventData) => canvasGroup.alpha = 1.0f;

    public void OnPointerExit(PointerEventData eventData) => canvasGroup.alpha = 0.5f;
}