using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider hpSlider;
    public TMP_Text bossNameText;

    [Header("Nastavení")]
    public string bossName = "Boss";

    int _maxHealth;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Init(int maxHealth)
    {
        _maxHealth = maxHealth;
        hpSlider.value = 1f;
        bossNameText.text = bossName;
        gameObject.SetActive(true);
    }

    public void UpdateBar(int currentHealth)
    {
        if (_maxHealth <= 0) return;
        hpSlider.value = (float)currentHealth / _maxHealth;

        if (currentHealth <= 0)
            gameObject.SetActive(false);
    }
}