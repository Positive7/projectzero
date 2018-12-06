using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject isOver;

    protected virtual void Awake()
    {
        isOver.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        isOver.SetActive(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isOver.SetActive(false);
    }
}