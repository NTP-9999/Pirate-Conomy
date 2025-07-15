using TMPro;
using UnityEngine;

public class OtherInteractUI : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private TMP_Text interactName;
    [SerializeField] private TMP_Text interactAction;
    private Canvas canvas;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();
    }
    private void LateUpdate()
    {
        Debug.Log("Camera Forward: " + Camera.main.transform.forward);
        transform.forward = Camera.main.transform.forward;
    }
    public void Show()
    {

    }
}
