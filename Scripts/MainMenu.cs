using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image fillingBar;
    [SerializeField] private int sceneID;

    private void Awake()
    {
        Invoke("LoadScene", 0.1f);
    }

    public void LoadScene()
    {
        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {
            fillingBar.fillAmount = operation.progress;
            yield return null;
        }
    }

}
