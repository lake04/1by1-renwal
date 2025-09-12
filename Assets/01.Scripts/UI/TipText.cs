using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipText : MonoBehaviour
{
    [SerializeField] private string[] tips;
    private TextMeshPro m_TextMeshPro;


    void OnEnable()
    {
       
    }


    void Start()
    {
        m_TextMeshPro = GetComponent<TextMeshPro>();
        SelectTip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SelectTip()
    {
        int num = Random.Range(-1,tips.Length);

        m_TextMeshPro.text = tips[num];

    }
}
