using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static PlayerSpawn Instance;

    [SerializeField] private GameObject spawnPos;

    public bool isSpawn = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawn)
        {
            Player.Instance.gameObject.transform.position =  spawnPos.transform.position;
            isSpawn = false;


            if (Player.Instance != null)
            {
                Player.Instance.enabled = true;
                Player.Instance.gameObject.SetActive(true);
                Player.Instance.canvas.SetActive(true);
                Debug.Log("활성화중중");
            }
        }

    }
}
