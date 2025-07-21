using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance;

    public int currentNova = 0;

    [Header("UI")]
    public List<TextMeshProUGUI> novaTexts = new List<TextMeshProUGUI>();  // List ของ Text ทุกอันที่จะแสดงจำนวนเงิน

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
                text.text = currentNova.ToString();
            }
        }
    }
    /// <summary>
    /// เรียกเมื่อ NovaTextRegister.OnEnable()  
    /// </summary>
    public void RegisterNovaText(TextMeshProUGUI txt)
    {
        if (!novaTexts.Contains(txt))
        {
            novaTexts.Add(txt);
            // ตั้งค่าทันทีให้ตรงกับยอดปัจจุบัน
            txt.text = currentNova.ToString();
        }
    }

    /// <summary>
    /// เรียกเมื่อ NovaTextRegister.OnDisable()
    /// </summary>
    public void UnregisterNovaText(TextMeshProUGUI txt)
    {
        novaTexts.Remove(txt);
    }
}
