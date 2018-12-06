using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateNewPlanet : DefaultButton, IPointerClickHandler
{
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(PlanetManager.Instance.Create());
    }

    private void Update()
    {
        if (PlanetManager.Instance.generating)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha          = 0.3f;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha          = 1.0f;
        }
    }
}