using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Sprite[] loadBackgrounds;
    [SerializeField] private Image loadBackground;
    private static SceneTransition instance;
    private static bool  shouldPlayOpeningAnimation = true;

    private Animator animator;
    private AsyncOperation loadingOperation;
    private string sceneName;

    private void Start()
    {
        instance = this;

        animator = GetComponent<Animator>();

        if (shouldPlayOpeningAnimation) animator.SetTrigger("SceneOpening");
    }
    public static void SwitchToScene(string _sceneName)
    {
        instance.sceneName = _sceneName;
        //instance.loadBackground.sprite = instance.loadBackgrounds[UnityEngine.Random.Range(0, instance.loadBackgrounds.Length)];
        instance.animator.SetTrigger("SceneClosing");
    }
    public void LoadScene()
    {
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        Time.timeScale = 1.0f;
        loadingOperation.allowSceneActivation = false;
    }
    public void OnAnimationOver()
    {
        shouldPlayOpeningAnimation = true;
        loadingOperation.allowSceneActivation = true;
    }
}
