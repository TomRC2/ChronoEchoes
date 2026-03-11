using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
            playButton.onClick.AddListener(AudioManager.Instance.PlayButtonClick);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void PlayGame()
    {
        LevelLoader.Instance.LoadNextLevel("Level1");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

       #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
       #endif
    }
}