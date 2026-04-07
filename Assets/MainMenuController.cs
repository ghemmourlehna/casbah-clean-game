using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("IntroScene"); // nom EXACT de ta scène d'intro
    }
}