using UnityEngine;
using UnityEngine.UI;

public class ShutterChargeMeterUI : MonoBehaviour
{
    [SerializeField] private ShutterController _shutterController;
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private Image _boltIcon;
    [SerializeField] private Color _boltChargingColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color _boltReadyColor = Color.white;

    private void Update()
    {
        if (_shutterController != null)
        {
            float progress = _shutterController.ChargeProgress;

            if (_chargeSlider != null)
            {
                _chargeSlider.value = progress;
            }

            if (_boltIcon != null)
            {
                _boltIcon.color = progress >= 1f ? _boltReadyColor : _boltChargingColor;
            }
        }
    }
}
