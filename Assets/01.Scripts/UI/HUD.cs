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
                // ���� ��� �ִ� ��
                Gun currentGun = Player.Instance.guns[Player.Instance.currentGun];

                // Player ��ũ��Ʈ�� ��Ÿ�� ��ųʸ����� ���� ��������
                if (Player.Instance.gunCooldowns.ContainsKey(currentGun))
                {
                    // ��Ÿ�� ���� ��
                    float curTime = Player.Instance.gunCooldowns[currentGun];
                    float maxTime = currentGun.cooldownTime;
                    mySlider.value = 1 - (curTime / maxTime); // ��Ÿ���� 0�� �� �� �����̴��� 1�� �ǵ���
                }
                else
                {
                    // ��Ÿ���� �ƴ� ��
                    mySlider.value = 1;
                }
                break;
     
        }
    }

    void Update()
    {

    }
}
