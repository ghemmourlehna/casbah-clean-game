using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SlideStep
{
    public Sprite topSprite;     // sprite à afficher en haut (SlotTop) - peut être null
    public Sprite bottomSprite;  // sprite à afficher en bas (SlotBottom) - peut être null
    public bool clearTop;        // si true, SlotTop sera caché
    public bool clearBottom;     // si true, SlotBottom sera caché
}

public class StepSlides : MonoBehaviour
{
    [Header("UI")]
    public Image slotTop;
    public Image slotBottom;
    public Button nextButton;
    public GameObject level1Button;

    [Header("Steps")]
    public SlideStep[] steps;

    [Header("Transition")]
    public float fadeDuration = 0.4f;

    int currentIndex = 0;
    bool isTransitioning = false;

    void Start()
    {
        if (level1Button != null)
            level1Button.SetActive(false);

        ApplyStep(0);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);
    }

    void ApplyStep(int index)
    {
        if (steps == null || steps.Length == 0) return;
        currentIndex = Mathf.Clamp(index, 0, steps.Length - 1);
        SlideStep step = steps[currentIndex];

        // Top slot
        if (slotTop != null)
        {
            if (step.clearTop || step.topSprite == null)
            {
                slotTop.sprite = null;
                slotTop.color = new Color(1, 1, 1, 0); // complètement transparent
            }
            else
            {
                slotTop.sprite = step.topSprite;
                StartCoroutine(FadeIn(slotTop));
            }
        }

        // Bottom slot
        if (slotBottom != null)
        {
            if (step.clearBottom || step.bottomSprite == null)
            {
                slotBottom.sprite = null;
                slotBottom.color = new Color(1, 1, 1, 0);
            }
            else
            {
                slotBottom.sprite = step.bottomSprite;
                StartCoroutine(FadeIn(slotBottom));
            }
        }
    }

    public void OnNextClicked()
    {
        if (isTransitioning) return;

        currentIndex++;

        if (currentIndex >= steps.Length)
        {
            // fin : cacher les slots + bouton suivant, montrer Level1
            if (slotTop != null) slotTop.gameObject.SetActive(false);
            if (slotBottom != null) slotBottom.gameObject.SetActive(false);
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            if (level1Button != null) level1Button.SetActive(true);
            return;
        }

        StartCoroutine(TransitionToStep(currentIndex));
    }

    System.Collections.IEnumerator TransitionToStep(int targetIndex)
    {
        isTransitioning = true;

        // fade out actuel
        if (slotTop != null && slotTop.color.a > 0f)
            yield return StartCoroutine(FadeOut(slotTop));
        if (slotBottom != null && slotBottom.color.a > 0f)
            yield return StartCoroutine(FadeOut(slotBottom));

        ApplyStep(targetIndex);
        isTransitioning = false;
    }

    System.Collections.IEnumerator FadeIn(Image img)
    {
        if (img == null) yield break;
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            img.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 1f);
    }

    System.Collections.IEnumerator FadeOut(Image img)
    {
        if (img == null) yield break;
        Color c = img.color;
        float startAlpha = c.a;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            img.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 0f);
    }

    // appelé par le bouton Level1
    public void OnLevel1Clicked()
    {
        SceneManager.LoadScene("Level1"); // nom exact de ta scène
    }
}