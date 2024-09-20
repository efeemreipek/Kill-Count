using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public event Action OnHealthZero;
    public static event Action OnDamageTaken;
    public static event Action OnGameOver;

    private void Start()
    {
        SetToMax();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (this.tag == "Player")
        {
            OnDamageTaken?.Invoke();
        }

        if (currentHealth <= 0)
        {
            OnHealthZero?.Invoke();
            if (this.tag == "Player")
            {
                OnGameOver?.Invoke();
            }
        }
        print("Health is: " + currentHealth);
    }
    public void SetToMax()
    {
        currentHealth = maxHealth;
    }
}
