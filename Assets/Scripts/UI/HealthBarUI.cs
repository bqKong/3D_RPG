using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;

    public Transform barPoint;

    [Header("是否长久可见")]
    public bool alwaysVisible;

    [Header("可视化时间")]
    public float visibleTime;

    private float timeLeft;

    private Image healthSlider;
    private Transform UIbar;
    private Transform cam;

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.updateHealBarOnAttack += UpdataHealthBar;
    }


    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvs in FindObjectsOfType<Canvas>())
        {
            if (canvs.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvs.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }

        }

    }

    private void UpdataHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(healthUIPrefab.gameObject);

        UIbar.gameObject.SetActive(true);

        timeLeft = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;

            //摄像机永远对着UIbar，在这里是反向的
            UIbar.forward = -cam.forward;

            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }


        }

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
