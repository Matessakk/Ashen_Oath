using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHealth : MonoBehaviour
{
    [Header("References (Auto-assigned at runtime)")]
    private PlayerHealth playerHealth;

    [Header("UI Hierarchy Configuration")]
    public Transform container;
    public GameObject hpIconPrefab;

    [Header("Sprites")]
    public Sprite fullHP;
    public Sprite emptyHP;

    List<Image> healthPoints = new List<Image>();
    bool _isInitialized = false;

    void Update()
    {
        // Keep searching for the spawned player instance until it successfully binds
        if (!_isInitialized && SpawnManager.Instance != null && SpawnManager.Instance.ActivePlayer != null)
        {
            playerHealth = SpawnManager.Instance.ActivePlayer.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.onHealthChanged += UpdateHealth;
                BuildIcons(playerHealth.maxHealth);
                UpdateHealth(playerHealth.currentHealth, playerHealth.maxHealth);
                _isInitialized = true;
            }
        }
    }

    void BuildIcons(int count)
    {
        foreach (var icon in healthPoints)
        {
            if (icon != null) Destroy(icon.gameObject);
        }
        healthPoints.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(hpIconPrefab, container);
            Image img = obj.GetComponent<Image>();
            healthPoints.Add(img);
        }
    }

    void UpdateHealth(int current, int max)
    {
        if (healthPoints.Count != max)
            BuildIcons(max);

        for (int i = 0; i < healthPoints.Count; i++)
        {
            if (healthPoints[i] != null)
            {
                healthPoints[i].sprite = i < current ? fullHP : emptyHP;
            }
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealth;
        }
    }
}