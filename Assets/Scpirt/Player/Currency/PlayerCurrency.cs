using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance;

    public int currentNova = 0;

    [Header("UI")]
    public List<TextMeshProUGUI> novaTexts;  // List ของ Text ทุกอันที่จะแสดงจำนวนเงิน

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateUI();
    }

    public void AddNova(int amount)
    {
        currentNova += amount;
        UpdateUI();
    }

    public bool SpendNova(int amount)
    {
        if (currentNova >= amount)
        {
            currentNova -= amount;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("Not enough Nova!");
            return false;
        }
    }

    private void UpdateUI()
    {
        foreach (var text in novaTexts)
        {
            if (text != null)
            {
                text.text = currentNova.ToString() + " Nova";
            }
        }
    }
}
