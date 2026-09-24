using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NpcPatrol : MonoBehaviour
{
    [Header("Patrulha")]
    [SerializeField] private float distanciaEsquerda = 2f;
    [SerializeField] private float distanciaDireita = 2f;
    [SerializeField] private float velocidade = 1.5f;

    [Header("Paradas")]
    [Range(0f, 1f)]
    [SerializeField] private float chanceParadaNoMeio = 0.4f;
    [SerializeField] private float tempoPorDirecao = 0.6f;

    private Rigidbody2D rb;
    private Animator anim;
    private Plate plate;
    private Transform player;
    private bool paused;
    private float leftX;
    private float rightX;

    private static readonly Vector2[] lookOrder =
    {
        Vector2.down, Vector2.left, Vector2.up, Vector2.right
    };

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        plate = GetComponent<Plate>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        leftX = rb.position.x - distanciaEsquerda;
        rightX = rb.position.x + distanciaDireita;

        if (plate != null)
            plate.OnPanelChanged += HandlePanel;

        StartCoroutine(PatrolRoutine());
    }

    private void OnDestroy()
    {
        if (plate != null)
            plate.OnPanelChanged -= HandlePanel;
    }

    private void HandlePanel(bool open)
    {
        paused = open;

        if (!open)
            return;

        SetWalking(false);

        if (player != null)
        {
            Vector2 d = player.position - transform.position;
            Vector2 face = Mathf.Abs(d.x) > Mathf.Abs(d.y)
                ? new Vector2(Mathf.Sign(d.x), 0)
                : new Vector2(0, Mathf.Sign(d.y));
            SetFacing(face);
        }
    }

    private IEnumerator PatrolRoutine()
    {
        int dir = 1;

        while (true)
        {
            float target = dir > 0 ? rightX : leftX;
            Vector2 face = new Vector2(dir, 0);

            if (Random.value < chanceParadaNoMeio)
            {
                float meio = (rb.position.x + target) / 2f;
                yield return WalkTo(meio, face);
                yield return LookAround();
            }

            yield return WalkTo(target, face);
            yield return LookAround();

            dir = -dir;
        }
    }

    private IEnumerator WalkTo(float x, Vector2 face)
    {
        SetFacing(face);
        SetWalking(true);

        while (Mathf.Abs(rb.position.x - x) > 0.01f)
        {
            if (paused)
            {
                yield return WaitWhilePaused(face);
                SetWalking(true);
            }

            Vector2 next = Vector2.MoveTowards(
                rb.position,
                new Vector2(x, rb.position.y),
                velocidade * Time.fixedDeltaTime
            );

            rb.MovePosition(next);
            yield return new WaitForFixedUpdate();
        }

        SetWalking(false);
    }

    private IEnumerator LookAround()
    {
        foreach (Vector2 d in lookOrder)
        {
            SetFacing(d);
            yield return Wait(tempoPorDirecao, d);
        }
    }

    private IEnumerator Wait(float time, Vector2 face)
    {
        while (time > 0f)
        {
            if (paused)
                yield return WaitWhilePaused(face);

            time -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitWhilePaused(Vector2 face)
    {
        if (!paused)
            yield break;

        SetWalking(false);

        while (paused)
            yield return null;

        SetFacing(face);
    }

    private void SetWalking(bool value)
    {
        if (anim != null)
            anim.SetBool("isWalking", value);
    }

    private void SetFacing(Vector2 d)
    {
        if (anim == null)
            return;

        for (int i = 0; i < 4; i++)
            anim.SetLayerWeight(i, 0);

        if (d.x > 0) anim.SetLayerWeight(2, 1);      // direita
        else if (d.x < 0) anim.SetLayerWeight(1, 1); // esquerda
        else if (d.y > 0) anim.SetLayerWeight(3, 1); // costas
        else anim.SetLayerWeight(0, 1);              // frente
    }
}