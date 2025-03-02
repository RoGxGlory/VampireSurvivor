using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerUISystem : MonoBehaviour
{
    public RectTransform panel; // The UI panel
    public Button toggleButton; // The button that opens/closes the panel

    private bool isOpen;
    private readonly float slideDuration = 0.3f; // Speed of sliding

    private void Start()
    {
        toggleButton.onClick.AddListener(TogglePanel);
    }

    public void TogglePanel()
    {
        StopAllCoroutines();
        StartCoroutine(SlidePanel(isOpen ? 250f : 0f)); // Move the panel in/out
        isOpen = !isOpen;
    }

    private IEnumerator SlidePanel(float targetX)
    {
        var startX = panel.anchoredPosition.x;
        var elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            var newX = Mathf.Lerp(startX, targetX, elapsedTime / slideDuration);
            panel.anchoredPosition = new Vector2(newX, panel.anchoredPosition.y);
            yield return null;
        }

        panel.anchoredPosition = new Vector2(targetX, panel.anchoredPosition.y);
    }
}