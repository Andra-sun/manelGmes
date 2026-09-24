using System.Collections;
using TMPro;
using UnityEngine;

public enum TypingMode
{
    UseGlobal, AlwaysOn, AlwaysOff
}

[RequireComponent(typeof(TMP_Text))]
public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TypingMode mode = TypingMode.UseGlobal;
    [Tooltip("0 = usa a velocidade do config global")]
    [SerializeField] private float speedOverride = 0f;

    private TMP_Text tmp;
    private Coroutine routine;

    public bool IsTyping { get; private set; }

    private TMP_Text Tmp
    {
        get
        {
            if (tmp == null)
                tmp = GetComponent<TMP_Text>();
            return tmp;
        }
    }

    public void Show(string text)
    {
        Stop();
        Tmp.text = text;

        if (ShouldType())
            routine = StartCoroutine(TypeRoutine());
        else
            Tmp.maxVisibleCharacters = int.MaxValue;
    }

    public void Skip()
    {
        Stop();
    }

    private bool ShouldType()
    {
        switch (mode)
        {
            case TypingMode.AlwaysOn: return true;
            case TypingMode.AlwaysOff: return false;
            default: return TypewriterConfig.Instance.typingEnabled;
        }
    }

    private IEnumerator TypeRoutine()
    {
        IsTyping = true;
        Tmp.maxVisibleCharacters = 0;
        Tmp.ForceMeshUpdate();

        int total = Tmp.textInfo.characterCount;
        float speed = speedOverride > 0f ? speedOverride : TypewriterConfig.Instance.charPerSecond;
        float delay = 1f / Mathf.Max(1f, speed);

        for (int i = 1; i <= total; i++)
        {
            Tmp.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        IsTyping = false;
        routine = null;
    }

    private void Stop()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = null;
        IsTyping = false;
        Tmp.maxVisibleCharacters = int.MaxValue;
    }

    private void OnDisable()
    {
        Stop();
    }
}