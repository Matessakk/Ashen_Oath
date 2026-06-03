using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ElementWheelUI : MonoBehaviour
{
    [Header("Reference (Auto-assigned at runtime)")]
    private WeaponCharge weaponCharge;

    [Header("Animace")]
    public float spinDuration = 0.3f;
    static readonly float[] ElementAngles = { 0f, 90f, 180f, 270f };

    RectTransform _rect;
    WeaponCharge.Element _lastElement;

    bool _isSpinning;
    float _spinTimer;
    float _fromAngle;
    float _toAngle;
    bool _isInitialized = false;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Safely hook up to the player's weapon layout at runtime
        if (!_isInitialized)
        {
            if (SpawnManager.Instance != null && SpawnManager.Instance.ActivePlayer != null)
            {
                weaponCharge = SpawnManager.Instance.ActivePlayer.GetComponent<WeaponCharge>();
                if (weaponCharge != null)
                {
                    _lastElement = weaponCharge.currentElement;
                    SetAngle(ElementAngles[(int)_lastElement]);
                    _isInitialized = true;
                }
            }
            return; // Skip evaluation until initialization completes
        }

        if (weaponCharge == null) return;

        if (!_isSpinning && weaponCharge.currentElement != _lastElement)
        {
            _lastElement = weaponCharge.currentElement;

            _fromAngle = _rect.localEulerAngles.z;

            if (_fromAngle > 180f) _fromAngle -= 360f;

            _toAngle = ElementAngles[(int)_lastElement];

            _spinTimer = 0f;
            _isSpinning = true;
        }

        if (_isSpinning)
            UpdateSpin();
    }

    void UpdateSpin()
    {
        _spinTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_spinTimer / spinDuration);
        float eased = EaseOutBack(t);

        SetAngle(Mathf.Lerp(_fromAngle, _toAngle, eased));

        if (t >= 1f)
        {
            SetAngle(_toAngle);
            _isSpinning = false;
        }
    }

    void SetAngle(float angle)
    {
        _rect.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + c1 * u * u;
    }
}