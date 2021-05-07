using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager: MonoBehaviour
{
    [SerializeField] private float transitionTime;
    [SerializeField] private Animator transition;

    public IEnumerator LoadSceneWithAnim(string scene)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        // returns control back to the caller until it's complete
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}