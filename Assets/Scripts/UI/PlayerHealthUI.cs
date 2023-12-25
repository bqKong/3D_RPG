using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private TextMeshProUGUI levelText;

    private Image healthSlider;

    private Image expSlider;

    private void Awake()
    {
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        levelText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playStats.characterData.currentLevel.ToString("00");
        updateHealth();
        updateExp();

    }

    void updateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playStats.CurrentHealth / GameManager.Instance.playStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void updateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playStats.characterData.currentExp / GameManager.Instance.playStats.characterData.baseExp ;
        healthSlider.fillAmount = sliderPercent;
    }


}
