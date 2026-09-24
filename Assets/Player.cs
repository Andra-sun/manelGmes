using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;

    private Animator anim;
    private Rigidbody2D rb;
    private Interactable nearbyInteractable;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (direction.y != 0)
        {
            direction.x = 0;
        }

        rb.linearVelocity = direction.normalized * Speed;

        if (direction != Vector2.zero)
        {
            if (anim != null)
            {
                ResetLayer();
                anim.SetBool("isWalking", true);

                if (direction.x > 0) // right
                {
                    anim.SetLayerWeight(2, 1);
                }
                else if (direction.x < 0) // left
                {
                    anim.SetLayerWeight(1, 1);
                }
                else if (direction.y > 0) // back
                {
                    anim.SetLayerWeight(3, 1);
                }
                else if (direction.y < 0) // front
                {
                    anim.SetLayerWeight(0, 1);
                }
            }
        }
        else if (anim != null)
        {
            anim.SetBool("isWalking", false);
        }
    }

    private void TryInteract()
    {
        if (nearbyInteractable == null)
            return;

        nearbyInteractable.Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponentInParent<Interactable>();

        if (interactable == null)
            return;

        nearbyInteractable = interactable;
        interactable.OnFocus();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponentInParent<Interactable>();

        if (interactable == null)
            return;

        if (nearbyInteractable == interactable)
        {
            interactable.OnLoseFocus();
            nearbyInteractable = null;
        }
    }

    private void ResetLayer()
    {
        if (anim == null)
            return;

        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
    }
    
}