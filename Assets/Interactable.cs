using UnityEngine;

public enum ObjectType
{
    ground, door, text, dialogue, collectable, none
}

public abstract class Interactable : MonoBehaviour
{
    public bool isInteracting;
    public ObjectType objectType;

    public virtual void OnFocus()
    {
    }

    public virtual void OnLoseFocus()
    {
    }

    public abstract void Interact();
}
