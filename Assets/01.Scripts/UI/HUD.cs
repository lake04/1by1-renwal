using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Health:
                float curHp = Player.Instance.CurHp;
                float maxHp = Player.Instance.MaxHp;
                mySlider.value = curHp / maxHp;
                break;
            case InfoType.Kill:
                //myText.text = string.Format("{0:F0}", GameManager.Instance.kill);
                break;
        }
    }

    void Update()
    {

    }
}
