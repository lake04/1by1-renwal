using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageText : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        StartCoroutine(TextEffect());
    }

    private IEnumerator TextEffect()
    {
        Color color = text.color;
        color.a = 0.0f;
        text.color = color;

        while (color.a < 1.0f)
        {
            color.a += Time.deltaTime;
            text.color = color;

            yield return null;
        }

        color.a = 1.0f;
        text.color = color;
        yield return new WaitForSeconds(0.2f);

        text.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            text.color = color;

            yield return null;
        }
    }
}