using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IRecover
{
    public float maxHealth = 100.0f;
    public SpriteRenderer spriteRenderer;
    public float hitFlashTime = 0.05f;

    public DropOnDeath drop;

    private float currentHealth;
    private float hitFlashTimer;

    private Color originalColor;

    private WaveDirector waveDirector;

    private void Awake()
    {
        currentHealth = maxHealth;

        if(spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        waveDirector = GameObject.FindAnyObjectByType<WaveDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(hitFlashTimer > 0.0f)
        if(hitFlashTimer < hitFlashTime)
        {
            //hitFlashTimer -= Time.deltaTime;
            hitFlashTimer += Time.deltaTime;

            //if(hitFlashTimer <= 0.0f)
            if(hitFlashTimer >= hitFlashTime)
            {
                RestoreColor();
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;

        PlayHitFlash();

        if(currentHealth <= 0.0f)
        {
            Die();
        }
    }

    void Die()
    {
        if(drop != null)
        {
            drop.SpawnDrops();
        }

        if (waveDirector != null)
        {
            waveDirector.ReportKill();
        }

        Destroy(gameObject);
    }

    void PlayHitFlash()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            //hitFlashTimer = hitFlashTime;
            hitFlashTimer = 0.0f;
        }
    }

    void RestoreColor()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(float value)
    {
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
    }
}
