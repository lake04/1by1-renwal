using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CircleEffect : MonoBehaviour
{
    public static CircleEffect Instance;

    public GameObject circle;

    private Coroutine routine;

    [SerializeField] private bool isStage = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(!isStage)
            Effect(0, 1, 0.7f);
    }

    public void Effect(float start, float end, float time, Action OnEnd = default)
    {
        if (routine != null) StopCoroutine(routine);

        routine = StartCoroutine(Cor());

        IEnumerator Cor()
        {
            float re = 1f / time;
            for (float t = 0; t < 1; t += Time.fixedDeltaTime * re)
            {
                circle.transform.localScale = Vector3.one * Mathf.Lerp(start, end, t);
                yield return Waits.fixedWait;
            }

            circle.transform.localScale = Vector3.one * end;
            OnEnd?.Invoke();
            if (Player.Instance != null)
            {
                Player.Instance.enabled = true;
                Player.Instance.canvas.SetActive(true);
            }
        }
    }

    public void LoadScene(string name)
    {
        Effect(1, 0, 0.5f, () =>
        {
            LoadingManager.Load(name);
            Effect(0, 1, 0.5f);

        });
    }
}