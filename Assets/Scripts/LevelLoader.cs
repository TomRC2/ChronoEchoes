using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.ResetTrigger("Start");
            transitionAnimator.SetTrigger("End");
            Debug.Log("Abriendo círculo en: " + scene.name);
        }
    }

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    IEnumerator LoadLevelRoutine(string sceneName)
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.ResetTrigger("End");
            transitionAnimator.SetTrigger("Start");
            Debug.Log("Cerrando círculo para cargar " + sceneName);
        }

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
