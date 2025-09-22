using SmoothShakeFree;
using System;
using UnityEngine;

public class HealthControll : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private GameObject healVFX;
    [field: SerializeField] public float RegenPoints { get; set; } = 0;
    private float currentHelth;
    private Character Character;

    public event Action<float> HealthChanged;
    public event Action<Character> DamageTaken;
    public event Action Healed;
    public event Action CharacterDied;

    private void OnEnable()
    {
        CharacterManager.EnemiesTurnEnded += OnEnemiesTurnEnded;
    }
    private void OnDisable()
    {
        CharacterManager.EnemiesTurnEnded -= OnEnemiesTurnEnded;
    }
    public void Start()
    {
        currentHelth = maxHealth;
        Character = GetComponent<Character>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeHealth(-10);
        }
    }
    public bool NotFull { get { return currentHelth != maxHealth; } }
    public bool Empty { get { return currentHelth <= 0; } }
    public bool Dead { get; set; } = false;
    public void ChangeHealth(float value, Character attacker = null)
    {
        currentHelth += value;
        if (value >= 0) //heal
        {
            if (currentHelth - value >= maxHealth)
            {
                currentHelth = maxHealth;
                return;
            }
            currentHelth = Mathf.Clamp(currentHelth, currentHelth - value, maxHealth);
            Healed?.Invoke();
            Instantiate(healVFX, Character.characterBottom.position, Quaternion.identity);

        }
        else //damage
        {
            DamageTaken?.Invoke(attacker);
            Character.animationController.SetTrigger(CharacterAnimParameters.Hit);
            if (Character.IsPlayer)
            {
                CameraShaker.Shake(ShakeType.Hit);
                CanvasController.Shake(ShakeType.CanvasHit);
            }
            if (currentHelth <= 0)
            {
                CharacterDied?.Invoke();
            }
        }

        float currentHealthAsPercentage = (float)currentHelth / maxHealth;
        HealthChanged?.Invoke(currentHealthAsPercentage);
    }
    public void RestoreHealth()
    {
        ChangeHealth(maxHealth - currentHelth);
    }
    public void ChangeMaxHealth(float increment)
    {
        maxHealth += increment;
        RestoreHealth();
    }
    public void Kill()
    {
        ChangeHealth(-maxHealth);
        Character.Die();
    }
    private void OnEnemiesTurnEnded()
    {
        if (RegenPoints > 0)
        {
            ChangeHealth(RegenPoints);
        }
    }
}
