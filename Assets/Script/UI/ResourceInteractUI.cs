using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ResourceInteractUI : MonoBehaviour
{
    private string resourceName;
    private Animator anim;
    public InteractUIState interactUIState = InteractUIState.ShowInteractable;

    [SerializeField] private CanvasGroup interactableIcon;
    [SerializeField] private CanvasGroup interactableBG;
    [SerializeField] private TMP_Text interactName;
    [SerializeField] private Image pressE;
    [SerializeField] private Slider progressBar;

    private Tween pressETween;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }

    public void SetUp(string resourceName)
    {
        this.resourceName = resourceName;
        interactName.text = resourceName;
        ShowInteractable();
    }

    public void ShowInteractable()
    {
        interactUIState = InteractUIState.ShowInteractable;

        interactName.gameObject.SetActive(false);
        pressE.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);

        interactableIcon.gameObject.SetActive(true);
        interactableIcon.alpha = 0f;
        interactableIcon.DOFade(1f, 0.3f);
        interactableIcon.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Interactable()
    {
        interactUIState = InteractUIState.Interactable;

        interactableBG.DOFade(1f, 0.3f);
        interactableIcon.DOFade(0f, 0.2f).OnComplete(() => interactableIcon.gameObject.SetActive(false));

        interactName.gameObject.SetActive(true);
        interactName.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        pressE.gameObject.SetActive(true);
        pressE.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        pressETween?.Kill();
        pressETween = pressE.transform.DOScale(1.1f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void Interacting()
    {
        interactUIState = InteractUIState.Interacting;

        interactableIcon.gameObject.SetActive(false);
        pressETween?.Kill();

        if (pressE.gameObject.activeSelf)
        {
            pressE.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                pressE.gameObject.SetActive(false);
            });
        }

        progressBar.gameObject.SetActive(true);
        progressBar.value = 0f;
        progressBar.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        progressBar.DOValue(1f, 1.6f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            progressBar.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                progressBar.gameObject.SetActive(false);
                ShowInteractable();
            });
        });
    }

    public void HideUI()
    {
        pressETween?.Kill();
        pressE.transform.DOKill();
        progressBar.transform.DOKill();
        interactableIcon.transform.DOKill();
        interactName.transform.DOKill();

        if (pressE.gameObject.activeSelf)
        {
            pressE.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                pressE.gameObject.SetActive(false);
            });
        }

        if (interactName.gameObject.activeSelf)
        {
            interactName.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                interactName.gameObject.SetActive(false);
            });
        }

        if (progressBar.gameObject.activeSelf)
        {
            progressBar.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                progressBar.gameObject.SetActive(false);
            });
        }

        if (interactableIcon.gameObject.activeSelf)
        {
            interactableIcon.DOFade(0f, 0.2f).OnComplete(() =>
            {
                interactableIcon.gameObject.SetActive(false);
            });
        }

        interactableBG.DOFade(0f, 0.2f);
        interactUIState = InteractUIState.ShowInteractable;
        // Optional: Destroy the UI after hiding
        Destroy(this.gameObject, 0.2f);
    }

    public void ReturnToShowInteractable()
    {
        DOVirtual.DelayedCall(0.25f, () =>
        {
            interactableBG.DOFade(0f, 0.2f);
            ShowInteractable();
        });
    }
}

public enum InteractUIState
{
    ShowInteractable,
    Interactable,
    Interacting
}
