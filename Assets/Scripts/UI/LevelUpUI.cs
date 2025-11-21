using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panelRoot;

    [SerializeField]
    private Button[] cardButton;

    [SerializeField]
    private Text[] cardText;

    [SerializeField]
    private Text titleText;

    [SerializeField]
    private UpgradeManager upgradeManager;

    private List<UpgradeData> currentChoices = new List<UpgradeData>();

    private void Awake()
    {
        if(panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        for(int i=0; i<cardButton.Length; ++i)
        {
            cardButton[i].onClick.AddListener(() => { OnSelect(i); });
        }
    }

    public void Open()
    {
        currentChoices = upgradeManager.DrawThree();

        for(int i=0; i<cardText.Length; ++i)
        {
            BindCardText(cardText[i], i);
        }

        if(panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        Time.timeScale = 0.0f;
    }

    public void OnSelect(int index)
    {
        if(index >= 0 && index < currentChoices.Count)
        {
            UpgradeData chosen = currentChoices[index];
            upgradeManager.ApplyUpgrade(chosen);
            Close();
        }
    }

    public void Close()
    {
        if(panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        Time.timeScale = 1.0f;
    }

    void BindCardText(Text target, int index)
    {
        if(target == null)
        {
            return;
        }

        if(index < currentChoices.Count)
        {
            UpgradeData data = currentChoices[index];
            if(data != null)
            {
                target.text = $"{data.displayName}\n{data.description}";
            }
            else
            {
                target.text = "N/A";
            }
        }
        else
        {
            target.text = "N/A";
        }
    }
}
