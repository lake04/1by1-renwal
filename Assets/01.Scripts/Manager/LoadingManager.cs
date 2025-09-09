using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private static readonly string LoadScene = "Loding";
    private static string targetScene = "Title";


    private IEnumerator Start()
    {
        yield return Waits.GetWait(0.5f);
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

    
        yield return Waits.GetWait(1.5f);
        op.allowSceneActivation = true;
    }

    public static void Load(string name)
    {
        targetScene = name;
        SceneManager.LoadScene(LoadScene);
    }


}
