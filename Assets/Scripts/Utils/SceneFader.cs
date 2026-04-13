using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    private static SceneFader instance;

    public Image fadeImage;
    public float fadeDuration = 1.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        gameObject.SetActive(true);
        EnsureOverlayCanvas();
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0); 
            fadeImage.raycastTarget = false;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public IEnumerator FadeAndLoad(string sceneName)
    {
        yield return FadeScene(sceneName);
    }
    
    public IEnumerator FadeAndReload(string sceneName)
    {
        yield return FadeScene(sceneName);
    }

    private IEnumerator FadeScene(string sceneName)
    {
        if (fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        yield return FadeToBlack();
        SceneManager.LoadScene(sceneName);
        yield return FadeFromBlack();
    }
    
    private IEnumerator FadeToBlack()
    {
        float t = 0f;
        Color black = Color.black; 
        
        fadeImage.raycastTarget = true;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            // Lerp from 0 alpha to 1 alpha
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = new Color(black.r, black.g, black.b, alpha);
            yield return null;
        }
        
        fadeImage.color = new Color(black.r, black.g, black.b, 1);
    }
    
    private IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color black = Color.black; 
        
        yield return new WaitForSeconds(0.1f);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            // Lerp from 1 alpha to 0 alpha
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = new Color(black.r, black.g, black.b, alpha);
            yield return null;
        }
        
        fadeImage.color = new Color(black.r, black.g, black.b, 0);
        fadeImage.raycastTarget = false;
    }

    private void EnsureOverlayCanvas()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        if (GetComponent<CanvasScaler>() == null)
            gameObject.AddComponent<CanvasScaler>();

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();
    }
}
