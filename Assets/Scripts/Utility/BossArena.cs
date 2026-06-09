using Unity.VisualScripting;
using UnityEngine;

public class BossArena : MonoBehaviour
{
    [Header("Gates")]
    public ArenaGate[] gates;

    [Header("Boss")]
    public BossHealth bossHealth;

    bool _arenaActive = false;
    private bool _bossDefeated = false;

    void Start()
    {
        // Gates start open
        OpenGates();
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // If boss already dead, keep gates open
        if (_bossDefeated) return;

        // If boss is gone but we never caught the death event
        if (bossHealth == null)
        {
            _bossDefeated = true;
            return;
        }

        if (_arenaActive) return;

        _arenaActive = true;
        CloseGates();

        bossHealth.onDeath += OnBossDied;
    }

    void OnBossDied()
    {
        _bossDefeated = true;
        _arenaActive = false;
        OpenGates();

        if (bossHealth != null)
            bossHealth.onDeath -= OnBossDied;
    }

    void CloseGates()
    {
        foreach (ArenaGate gate in gates)
            gate.Close();
    }

    void OpenGates()
    {
        foreach (ArenaGate gate in gates)
            gate.Open();
    }

    void OnDestroy()
    {
        // Clean up subscription if arena gets destroyed
        if (bossHealth != null)
            bossHealth.onDeath -= OnBossDied;
    }
}