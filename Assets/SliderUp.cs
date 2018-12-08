using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderUp : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData) => GetComponent<AudioSource>().Play();
}