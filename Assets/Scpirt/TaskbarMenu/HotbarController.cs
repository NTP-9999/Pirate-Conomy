using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public Image[] slots;
    public int selectedIndex = 0;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedIndex = i;
                UpdateSelection();
            }
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }
}
