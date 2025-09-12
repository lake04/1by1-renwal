using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loding : MonoBehaviour
{
    private bool isFirest = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
            PlayerDeble();
    }

    private void PlayerDeble()
    {
        if (Player.Instance != null)
        {
            Player.Instance.enabled = false;
            Player.Instance.gameObject.SetActive(false);
            Player.Instance.canvas.SetActive(false);
            isFirest = true;
            Debug.Log("비활성화 중중ㅈ우");
        }
    }
}
