using UnityEngine.EventSystems;

public class PlanetSettingsButton : DefaultButton, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PlanetSettings.Instance.planetSettings.SetActive(!PlanetSettings.Instance.planetSettings.activeSelf);
    }
}