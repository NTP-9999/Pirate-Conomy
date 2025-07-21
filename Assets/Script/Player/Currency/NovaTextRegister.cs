using UnityEngine;
using TMPro;

public class NovaTextRegister : MonoBehaviour
{
    TextMeshProUGUI _tmp;

    void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        PlayerCurrency.Instance.RegisterNovaText(_tmp);
    }

    void OnDisable()
    {
        PlayerCurrency.Instance.UnregisterNovaText(_tmp);
    }
}
