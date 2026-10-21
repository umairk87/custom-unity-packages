
using UnityEngine;
using UnityEngine.UI;

namespace UKFramework.Game.UI
{
[RequireComponent(typeof(Image))]
public class AdaptiveScreen : MonoBehaviour
{
    [SerializeField] private Sprite portraitSprite;
    [SerializeField] private Sprite landscapeSprite;

    private Image background;
    private RectTransform rectTransform;
    private bool? lastPortrait;

    private void Awake()
    {
        background = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Fill the parent
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        background.preserveAspect = false;
        background.raycastTarget = false;
    }

    private void OnEnable()
    {
        UpdateBackground();
    }

    private void Update()
    {
        bool portrait = Screen.height >= Screen.width;

        if (lastPortrait != portrait)
            UpdateBackground();
    }

    private void UpdateBackground()
    {
        bool portrait = Screen.height >= Screen.width;
        lastPortrait = portrait;

        background.sprite = portrait
            ? portraitSprite
            : landscapeSprite;
    }
}
}
