using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float maxHealth = 100;
    public float currentHealth;

    public healthBar healthBar;


    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }


    private void Update()
    {
        healthBar.SetHealth(currentHealth);

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10); 
        }

        currentHealth += Time.deltaTime;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeHeal(float heal)
    {
        currentHealth += heal;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            //disable all other player controls.
            //play death anim
            //game over screen.
        }
    }
}
