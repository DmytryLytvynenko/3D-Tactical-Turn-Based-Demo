using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthControll healthControll;
    [SerializeField] private Image healthBarFilling;
    [SerializeField] private Gradient gradient;

    private Camera mainCamera;


    private void Awake()
    {
        healthControll.HealthChanged += OnHealthChanged;
        mainCamera = Camera.main;
        OnHealthChanged(1);
    }
    private void OnDestroy()
    {
        healthControll.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float valueAsPercentage)
    {
        healthBarFilling.fillAmount = valueAsPercentage;
        Color color = gradient.Evaluate(valueAsPercentage);
        color.a = 0.7f;
        healthBarFilling.color = color;
    }
    private void LateUpdate()
    {
        transform.LookAt(mainCamera.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
