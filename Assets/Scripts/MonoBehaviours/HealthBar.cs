using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HitPoints hitPoints;
    [HideInInspector]
    public Player character;
    public Image meterImage;
    public Text hpText;
    float maxHitpoints;
    // Start is called before the first frame update
    void Start()
    {
        maxHitpoints = character.maxHitPoints;   
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            meterImage.fillAmount = hitPoints.value / maxHitpoints;
            hpText.text = "HP: " + (meterImage.fillAmount * 100);
        }
    }
}
