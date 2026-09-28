using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Plate : Interactable
{
    [SerializeField] private GameObject painel;
    [SerializeField] private TypewriterText textUI;
    [TextArea]
    [SerializeField] private string mensagem;

    [Header("Áudio")]
    [SerializeField] private AudioClip somInteracao;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private const float DuracaoMinimaPagina = 0.3f;

    private AudioSource audioSource;
    private Coroutine pagingRoutine;
    private bool isPaging;

    public event Action<bool> OnPanelChanged;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = somInteracao;
        audioSource.loop = false;

        if (painel != null)
            painel.SetActive(false);
    }

    public override void Interact()
    {
        if (painel == null || textUI == null)
            return;

        if (isPaging)
        {
            if (textUI.IsTyping)
                textUI.Skip();

            return;
        }

        if (painel.activeSelf)
        {
            FecharPainel();
            return;
        }

        painel.SetActive(true);
        OnPanelChanged?.Invoke(true);

        if (somInteracao != null)
        {
            audioSource.Stop();
            audioSource.volume = volume;
            audioSource.Play();
        }

        string[] paginas = mensagem
            .Replace("\r\n", "\n")
            .Split('\n')
            .Where(l => l.Trim().Length > 0)
            .ToArray();

        if (somInteracao != null && paginas.Length > 1)
            pagingRoutine = StartCoroutine(PlayPages(paginas, somInteracao.length));
        else if (somInteracao != null)
            textUI.ShowSynced(mensagem, somInteracao.length);
        else
            textUI.Show(mensagem);
    }

    public override void OnLoseFocus()
    {
        FecharPainel();
    }

    private IEnumerator PlayPages(string[] paginas, float audioLength)
    {
        isPaging = true;

        int totalChars = 0;
        foreach (string p in paginas)
            totalChars += p.Trim().Length;
        totalChars = Mathf.Max(1, totalChars);

        foreach (string p in paginas)
        {
            string pagina = p.Trim();
            float duracao = audioLength * (pagina.Length / (float)totalChars);
            duracao = Mathf.Max(duracao, DuracaoMinimaPagina);

            textUI.ShowSynced(pagina, duracao);
            yield return new WaitForSeconds(duracao);
        }

        isPaging = false;
        pagingRoutine = null;
    }

    private void FecharPainel()
    {
        if (pagingRoutine != null)
        {
            StopCoroutine(pagingRoutine);
            pagingRoutine = null;
        }

        isPaging = false;

        if (painel != null)
            painel.SetActive(false);

        audioSource.Stop();
        OnPanelChanged?.Invoke(false);
    }
}