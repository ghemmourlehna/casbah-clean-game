using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class DialogueStep
{
    public Sprite topImage;            // image spéciale en haut (peut être null)
    public Sprite portraitLeft;        // sprite de la femme (null = cachée)
    public Sprite bubbleLeft;          // bulle de la femme (null = cachée)
    public Sprite portraitRight;       // sprite du jeune
    public Sprite bubbleRight;         // bulle du jeune
}

public class DialogueLayout : MonoBehaviour
{
    [Header("UI Slots")]
    public Image topImageSlot;
    public Image portraitLeftSlot;
    public Image bubbleLeftSlot;
    public Image portraitRightSlot;
    public Image bubbleRightSlot;

    [Header("Controls")]
    public Button nextButton;
    public GameObject level1Button;    // Garde ton GameObject
    public float fadeDuration = 0.4f;

    [Header("Steps")]
    public DialogueStep[] steps;

    int currentIndex = 0;
    bool isTransitioning = false;

    void Start()
    {
        // Cache le bouton level1 au début
        if (level1Button != null)
            level1Button.SetActive(false);

        // Affiche la première étape
        ApplyStep(0);

        // Connecte le bouton suivant
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextClicked);

        // ===== CORRECTION : Connecte le bouton Level 1 =====
        ConnectLevel1Button();
        // ===================================================
    }

    // NOUVELLE MÉTHODE : Connecte automatiquement le bouton Level 1
    void ConnectLevel1Button()
    {
        if (level1Button != null)
        {
            Button btn = level1Button.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnLevel1Clicked);
                Debug.Log("✅ Bouton Level 1 connecté automatiquement !");
            }
            else
            {
                Debug.LogError("❌ Le GameObject level1Button n'a pas de composant Button !");
            }
        }
    }

    void ApplyStep(int index)
    {
        if (steps == null || steps.Length == 0) return;
        currentIndex = Mathf.Clamp(index, 0, steps.Length - 1);
        DialogueStep step = steps[currentIndex];

        // Top image
        SetImage(topImageSlot, step.topImage);

        // Left side (femme)
        SetImage(portraitLeftSlot, step.portraitLeft);
        SetImage(bubbleLeftSlot, step.bubbleLeft);

        // Right side (jeune)
        SetImage(portraitRightSlot, step.portraitRight);
        SetImage(bubbleRightSlot, step.bubbleRight);
    }

    void SetImage(Image slot, Sprite sprite)
    {
        if (slot == null) return;

        if (sprite == null)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0);
        }
        else
        {
            slot.sprite = sprite;
            StartCoroutine(FadeIn(slot));
        }
    }

    public void OnNextClicked()
    {
        if (isTransitioning) return;

        currentIndex++;

        if (currentIndex >= steps.Length)
        {
            // Fin : on laisse la dernière image à l'écran
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            if (level1Button != null)
                level1Button.SetActive(true);

            return;
        }

        StartCoroutine(TransitionToStep(currentIndex));
    }

    void HideAllSlots()
    {
        HideImage(topImageSlot);
        HideImage(portraitLeftSlot);
        HideImage(bubbleLeftSlot);
        HideImage(portraitRightSlot);
        HideImage(bubbleRightSlot);
    }

    void HideImage(Image img)
    {
        if (img == null) return;
        img.color = new Color(1, 1, 1, 0);
        img.sprite = null;
    }

    System.Collections.IEnumerator TransitionToStep(int targetIndex)
    {
        isTransitioning = true;

        // fade out actuel
        yield return StartCoroutine(FadeOut(topImageSlot));
        yield return StartCoroutine(FadeOut(portraitLeftSlot));
        yield return StartCoroutine(FadeOut(bubbleLeftSlot));
        yield return StartCoroutine(FadeOut(portraitRightSlot));
        yield return StartCoroutine(FadeOut(bubbleRightSlot));

        ApplyStep(targetIndex);
        isTransitioning = false;
    }

    System.Collections.IEnumerator FadeIn(Image img)
    {
        if (img == null || img.sprite == null) yield break;

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

    // MÉTHODE QUI CHARGE LA SCÈNE LEVEL1
    public void OnLevel1Clicked()
    {
        Debug.Log("🚀 Chargement de la scène Level1...");
        SceneManager.LoadScene("Level1");
    }
}