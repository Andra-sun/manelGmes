using TMPro;
using UnityEngine;

public class Plate : Interactable
{
    [SerializeField] private GameObject painel;
    [SerializeField] private TMP_Text textUI;
    [TextArea]
    [SerializeField] private string mensagem;

    private void Start()
    {
        if (painel != null)
        {
            painel.SetActive(false);
        }
    }

    public override void Interact()
    {
        if (painel == null || textUI == null)
            return;

        bool open = !painel.activeSelf;
        painel.SetActive(open);

        if (open)
        {
            textUI.text = mensagem;
        }
    }

    public override void OnLoseFocus()
    {
        if (painel != null) painel.SetActive(false);
    }
}
