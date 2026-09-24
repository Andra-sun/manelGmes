using System;
using UnityEngine;

public class Plate : Interactable
{
    [SerializeField] private GameObject painel;
    [SerializeField] private TypewriterText textUI;
    [TextArea]
    [SerializeField] private string mensagem;

    public event Action<bool> OnPanelChanged;

    private void Start()
    {
        if (painel != null)
            painel.SetActive(false);
    }

    public override void Interact()
    {
        if (painel == null || textUI == null)
            return;

        if (textUI.IsTyping)
        {
            textUI.Skip();
            return;
        }

        if (painel.activeSelf)
        {
            painel.SetActive(false);
            OnPanelChanged?.Invoke(false);
            return;
        }

        painel.SetActive(true);
        textUI.Show(mensagem);
        OnPanelChanged?.Invoke(true);
    }

    public override void OnLoseFocus()
    {
        if (painel != null)
            painel.SetActive(false);

        OnPanelChanged?.Invoke(false);
    }
}