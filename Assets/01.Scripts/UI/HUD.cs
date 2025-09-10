using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Skill, Level, Kill, Time, Health}
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
            case InfoType.Skill:
                // 현재 들고 있는 총
                Gun currentGun = Player.Instance.guns[Player.Instance.currentGun];

                // Player 스크립트의 쿨타임 딕셔너리에서 정보 가져오기
                if (Player.Instance.gunCooldowns.ContainsKey(currentGun))
                {
                    // 쿨타임 진행 중
                    float curTime = Player.Instance.gunCooldowns[currentGun];
                    float maxTime = currentGun.cooldownTime;
                    mySlider.value = 1 - (curTime / maxTime); // 쿨타임이 0이 될 때 슬라이더가 1이 되도록
                }
                else
                {
                    // 쿨타임이 아닐 때
                    mySlider.value = 1;
                }
                break;
     
        }
    }

    void Update()
    {

    }
}
