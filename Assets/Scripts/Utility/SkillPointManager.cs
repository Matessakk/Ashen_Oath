using UnityEngine;


public class SkillPointManager : MonoBehaviour
{
    public static SkillPointManager Instance { get; private set; }

    public int skillPoints { get; private set; }

    public System.Action<int> onPointsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddPoint(int amount = 1)
    {
        skillPoints += amount;
        onPointsChanged?.Invoke(skillPoints);
    }

    public bool SpendPoint()
    {
        if (skillPoints <= 0) return false;
        skillPoints--;
        onPointsChanged?.Invoke(skillPoints);
        return true;
    }
}
