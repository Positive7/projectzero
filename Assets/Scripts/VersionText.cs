using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<TMP_Text>().text = $"Version : {Application.version}";
    }
}