using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public Animator animator;
    public GameObject text;
    private bool isKey = false;

    private void Start()
    {
        Invoke("waiting", 1);
    }
    public void OnClickPlay()
    {
        Invoke("Play",1);
    }
    private void Update()
    {
        if(Input.anyKey && isKey == true)
        {
            text.SetActive(false);
            animator.SetTrigger("Play");
        }
    }

    private void Play()
    {
        CircleEffect.Instance.LoadScene("Stage1");
    }

    public void ClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
    private void waiting()
    {
        isKey = true;
    }

}
