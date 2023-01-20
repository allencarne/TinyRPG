using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    Enemy enemy;

    [SerializeField] Image healthBarFront;
    [SerializeField] Image healthBarBack;
    [SerializeField] Canvas EnemyUI;
    [HideInInspector] public float chipSpeed = 2f;
    [HideInInspector] public float lerpTimer;

    public void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Start()
    {
        enemy.enemyHealth = enemy.enemyMaxHealth;
    }

    public void Update()
    {
        enemy.enemyHealth = Mathf.Clamp(enemy.enemyHealth, 0, enemy.enemyMaxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        float fillFront = healthBarFront.fillAmount;
        float fillBack = healthBarBack.fillAmount;
        float healthFraction = enemy.enemyHealth / enemy.enemyMaxHealth;

        if (fillBack > healthFraction)
        {
            healthBarFront.fillAmount = healthFraction;
            healthBarBack.color = Color.white;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            healthBarBack.fillAmount = Mathf.Lerp(fillBack, healthFraction, percentComplete);
        }

        if (fillFront < healthFraction)
        {
            healthBarBack.color = Color.green;
            healthBarBack.fillAmount = healthFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            healthBarFront.fillAmount = Mathf.Lerp(fillFront, healthBarBack.fillAmount, percentComplete);
        }
    }
}
