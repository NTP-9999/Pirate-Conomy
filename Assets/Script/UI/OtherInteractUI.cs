using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Mono.Cecil;

public class OtherInteractUI : MonoBehaviour
{
    public InteractUIState interactUIState = InteractUIState.ShowInteractable;
    public InteractBehavior interactBehavior = InteractBehavior.Tap;
    private string objectName;
    private string actionName;
    private Animator anim;

    [SerializeField] private CanvasGroup interactableIcon;
    [SerializeField] private TMP_Text interactName;
    [SerializeField] private TMP_Text interactActionName;
    [SerializeField] private Image keyPress;
    [SerializeField] private CanvasGroup interactableNameBG;
    [SerializeField] private CanvasGroup interactableActionBG;
    [SerializeField] private Image radianIcon;
    private KeyCode interactKey = KeyCode.G; // Default key for interaction, can be changed later
    [SerializeField] private Image progressBar; // Optional, if you want to show progress for hold interactions
    private float holdTime;

    private float holdTimer = 0f;
    private Tween radianTween;
    private Tween pressTween;
    
    public Sprite defaultKeyIcon;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKey(interactKey))
        {
            if (interactUIState == InteractUIState.Interactable)
            {
                if (interactBehavior == InteractBehavior.Hold)
                {
                    holdTimer += Time.deltaTime;
                    progressBar.fillAmount = Mathf.Clamp01(holdTimer / holdTime);

                    if (holdTimer >= holdTime)
                    {
                        HideUI();
                        holdTimer = 0f;
                        progressBar.fillAmount = 0f;
                    }
                }
                else
                {
                    // shake the keyPress icon
                    ShakePressE();
                }
            }
        }
        else if (!Input.GetKey(interactKey))
        {
            holdTimer = 0f;
            progressBar.fillAmount = 0f;
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }

    public void SetUp(string _objectName, string _actionName, Sprite keySprite = null, KeyCode key = KeyCode.G)
    {
        interactKey = key;
        objectName = _objectName;
        actionName = _actionName;
        interactName.text = objectName;
        interactActionName.text = actionName;
        if (keySprite == null)
        {
            keySprite = defaultKeyIcon; // Use a default icon if none is provided
        }
        keyPress.sprite = keySprite;
        interactKey = key;
        interactBehavior = InteractBehavior.Tap;
        ShowInteractable();
    }

    public void SetUp(string _objectName, string _actionName, Sprite keySprite, KeyCode key, float _holdTime)
    {
        interactKey = key;
        objectName = _objectName;
        actionName = _actionName;
        interactName.text = objectName;
        interactActionName.text = actionName;
        keyPress.sprite = keySprite;
        interactKey = key;
        interactBehavior = InteractBehavior.Hold;
        holdTime = _holdTime;
        ShowInteractable();
    }

    public void ShowInteractable()
    {
        interactUIState = InteractUIState.ShowInteractable;

        interactName.gameObject.SetActive(false);
        keyPress.gameObject.SetActive(false);
        interactActionName.gameObject.SetActive(false);
        interactableNameBG.gameObject.SetActive(false);
        interactableActionBG.gameObject.SetActive(false);
        radianIcon.gameObject.SetActive(false);

        interactableIcon.gameObject.SetActive(true);
        interactableIcon.alpha = 0f;
        interactableIcon.DOFade(1f, 0.3f);
        progressBar.gameObject.SetActive(false);
    }

    public void Interactable()
    {
        interactUIState = InteractUIState.Interactable;

        // Fade out icon
        interactableIcon.DOFade(0f, 0.2f).OnComplete(() => interactableIcon.gameObject.SetActive(false));

        // Fade in text and key icons
        interactName.gameObject.SetActive(true);
        interactName.DOFade(1f, 0.25f);

        interactActionName.gameObject.SetActive(true);
        interactActionName.DOFade(1f, 0.25f);

        keyPress.gameObject.SetActive(true);
        keyPress.DOFade(1f, 0.25f);

        interactableNameBG.gameObject.SetActive(true);
        interactableNameBG.alpha = 0f;
        interactableNameBG.DOFade(1f, 0.3f);

        interactableActionBG.gameObject.SetActive(true);
        interactableActionBG.alpha = 0f;
        interactableActionBG.DOFade(1f, 0.3f);

        radianIcon.gameObject.SetActive(true);
        radianIcon.color = new Color(radianIcon.color.r, radianIcon.color.g, radianIcon.color.b, 0f);
        radianIcon.DOFade(1f, 0.3f);
        progressBar.gameObject.SetActive(interactBehavior == InteractBehavior.Hold);
        progressBar.fillAmount = 0f;
    }

    public void ReturnToShowInteractable()
    {
        interactUIState = InteractUIState.ShowInteractable;

        interactName.DOFade(0f, 0.2f).OnComplete(() => interactName.gameObject.SetActive(false));
        interactActionName.DOFade(0f, 0.2f).OnComplete(() => interactActionName.gameObject.SetActive(false));
        keyPress.DOFade(0f, 0.2f).OnComplete(() => keyPress.gameObject.SetActive(false));

        interactableNameBG.DOFade(0f, 0.2f).OnComplete(() => interactableNameBG.gameObject.SetActive(false));
        interactableActionBG.DOFade(0f, 0.2f).OnComplete(() => interactableActionBG.gameObject.SetActive(false));

        radianTween?.Kill();
        radianIcon.gameObject.SetActive(false);

        interactableIcon.gameObject.SetActive(true);
        interactableIcon.alpha = 0f;
        interactableIcon.DOFade(1f, 0.3f);
    }

    public void HideUI()
    {
        interactableIcon.DOFade(0f, 0.2f).OnComplete(() => Destroy(gameObject));

        interactName.gameObject.SetActive(false);
        interactActionName.gameObject.SetActive(false);
        keyPress.gameObject.SetActive(false);
        interactableNameBG.gameObject.SetActive(false);
        interactableActionBG.gameObject.SetActive(false);

        radianTween?.Kill();
        radianIcon.gameObject.SetActive(false);
    }

    public void ShakePressE()
    {
        if (!keyPress.gameObject.activeSelf) return;

        // Kill pulse tween if exists
        pressTween?.Kill();

        // Perform quick shake
        keyPress.transform.DOShakeScale(0.3f, 0.3f, 10, 90f)
            .OnComplete(() =>
            {
                // Restart pulsing after shake
                pressTween = keyPress.transform.DOScale(1.1f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            });
    }
}
public enum InteractBehavior
{
    Hold,
    Tap
}
