using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHealth : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Transform container;
    public GameObject hpIconPrefab;

    [Header("Sprites")]
    public Sprite fullHP;
    public Sprite emptyHP;

    List<Image> healthPoints = new List<Image>();

    void Start()
    {
        playerHealth.onHealthChanged += UpdateHealth;
        BuildIcons(playerHealth.maxHealth);
        UpdateHealth(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    void BuildIcons(int count)
    {
        foreach (var icon in healthPoints)
            Destroy(icon.gameObject);
        healthPoints.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(hpIconPrefab, container);
            Image img = obj.GetComponent<Image>();
            Debug.Log($"Created icon {i}, Image component: {img}");
            healthPoints.Add(img);
        }
    }

    void UpdateHealth(int current, int max)
    {
        if (healthPoints.Count != max)
            BuildIcons(max);

        for (int i = 0; i < healthPoints.Count; i++)
        {
            Debug.Log($"Icon {i}: {healthPoints[i]}, fullHP: {fullHP}, emptyHP: {emptyHP}");
            healthPoints[i].sprite = i < current ? fullHP : emptyHP;
        }
    }
}

