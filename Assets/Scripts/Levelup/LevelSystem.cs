using System.Runtime.InteropServices;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField]
    private float baseExpToLevel = 5.0f;

    [SerializeField]
    private float growthPerLevel = 2.5f;

    private float currentExp;
    private int currentLevel = 1;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentExp = 0.0f;
    }

    public void AddExperience(float amount)
    {
        if(gameManager != null)
        {
            if(gameManager.IsPlaying() == false)
            {
                return;
            }
        }

        if(amount <= 0.0f)
        {
            return;
        }

        currentExp += amount;

        int safety = 32;
        while(safety > 0)
        {
            float need = GetRequiredExpForNextLevel();
            if(currentExp >= need)
            {
                currentExp -= need;
                ++currentLevel;
                OnLevelUp();
            }
            else
            {
                break;
            }

            --safety;
        }
    }

    public float GetRequiredExpForNextLevel()
    {
        float need = baseExpToLevel + growthPerLevel * (currentLevel - 1);
        return Mathf.Max(1.0f, need);
    }

    void OnLevelUp()
    {
        Debug.Log("·¹º§ ¾÷!!! : " + currentLevel);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public float GetCurrentExp()
    {
        return currentExp;
    }
}
