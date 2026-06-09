using UnityEngine;

public class ArenaGate : MonoBehaviour
{
    private Collider2D _collider;
    private Animator _animator;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    public void Close()
    {
        _collider.enabled = true;
        _animator.SetBool("IsOpen", false);
        Debug.Log($"[ArenaGate] {gameObject.name} closing. IsOpen set to false.");
    }

    public void Open()
    {
        _animator.SetBool("IsOpen", true);
        // Disable collider after animation finishes
        Invoke(nameof(DisableCollider), GetOpenAnimationLength());
    }

    void DisableCollider()
    {
        _collider.enabled = false;
    }

    float GetOpenAnimationLength()
    {
        // Match this to however long your open animation is
        AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0) return clips[0].clip.length;
        return 0.5f;
    }
}