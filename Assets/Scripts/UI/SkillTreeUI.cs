using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SkillTreeUI : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public TMP_Text pointsText;

    [Header("Buttons")]
    public Button fireButton;
    public Button waterButton;
    public Button earthButton;
    public Button airButton;
    public Button hpButton;

    [Header("Unlock texty")]
    public string unlockedSuffix = " X";

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        SetVisible(false);

        fireButton.onClick.AddListener(OnFire);
        waterButton.onClick.AddListener(OnWater);
        earthButton.onClick.AddListener(OnEarth);
        airButton.onClick.AddListener(OnAir);
        hpButton.onClick.AddListener(OnHP);

        SkillPointManager.Instance?.onPointsChanged.Equals(null);
    }

    void OnEnable()
    {
        if (SkillPointManager.Instance != null)
            SkillPointManager.Instance.onPointsChanged += RefreshPoints;
    }

    void OnDisable()
    {
        if (SkillPointManager.Instance != null)
            SkillPointManager.Instance.onPointsChanged -= RefreshPoints;
    }

    public void Show()
    {
        Debug.Log("SkillTreeUI Show zavolano");
        SetVisible(true);
        RefreshAll();
    }

    public void Hide()
    {
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    void RefreshAll()
    {
        RefreshPoints(SkillPointManager.Instance?.skillPoints ?? 0);
        RefreshButtons();
    }

    void RefreshPoints(int points)
    {
        if (pointsText != null)
            pointsText.text = $"Skill Points: {points}";
    }

    void RefreshButtons()
    {
        var st = SkillTree.Instance;
        if (st == null) return;

        SetButtonState(fireButton, st.fireUnlocked, "Fire: +Damage");
        SetButtonState(waterButton, st.waterUnlocked, "Water: +Slow");
        SetButtonState(earthButton, st.earthUnlocked, "Earth: +Stun");
        SetButtonState(airButton, st.airUnlocked, "Air: +Knockback");
        SetButtonState(hpButton, st.hpUnlocked, "+1 Max HP");
    }

    void SetButtonState(Button btn, bool unlocked, string label)
    {
        btn.interactable = !unlocked;
        var txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.text = unlocked ? label + unlockedSuffix : label;
    }

    void OnFire() { if (SkillTree.Instance.UnlockFire()) RefreshAll(); }
    void OnWater() { if (SkillTree.Instance.UnlockWater()) RefreshAll(); }
    void OnEarth() { if (SkillTree.Instance.UnlockEarth()) RefreshAll(); }
    void OnAir() { if (SkillTree.Instance.UnlockAir()) RefreshAll(); }
    void OnHP() { if (SkillTree.Instance.UnlockHP()) RefreshAll(); }
}
