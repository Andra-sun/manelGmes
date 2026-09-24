using UnityEngine;

public class TextInteractable : Interactable
{
    [TextArea]
    public string text;

    public override void Interact()
    {
        if (isInteracting)
            return;

        isInteracting = true;
        Debug.Log(text);
    }
}
