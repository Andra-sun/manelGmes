using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Animal : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 1.5f;
    [SerializeField] private float tamanhoPasso = 1f;
    [SerializeField] private int passosMinimos = 1;
    [SerializeField] private int passosMaximos = 3;

    [Header("Paradas")]
    [SerializeField] private float tempoParadoMinimo = 0.5f;
    [SerializeField] private float tempoParadoMaximo = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 ultimaDirecao;
    private Vector2 direcaoBloqueada;
    private bool colidiu;

    private static readonly Vector2[] direcoes =
    {
        Vector2.down, Vector2.left, Vector2.up, Vector2.right
    };

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(MovimentoAleatorio());
    }

    private IEnumerator MovimentoAleatorio()
    {
        while (true)
        {
            Vector2 direcao = EscolherDirecao();
            direcaoBloqueada = Vector2.zero;

            int passos = Random.Range(passosMinimos, passosMaximos + 1);

            for (int i = 0; i < passos; i++)
            {
                yield return Walk(direcao);

                if (colidiu)
                {
                    colidiu = false;
                    break;
                }
            }

            SetWalking(false);

            float tempoParado = Random.Range(tempoParadoMinimo, tempoParadoMaximo);
            yield return new WaitForSeconds(tempoParado);
        }
    }

    private IEnumerator Walk(Vector2 direcao)
    {
        ultimaDirecao = direcao;
        SetFacing(direcao);
        SetWalking(true);

        Vector2 inicio = rb.position;
        Vector2 destino = inicio + direcao * tamanhoPasso;

        while (Vector2.Distance(rb.position, destino) > 0.01f)
        {
            if (colidiu)
                yield break;

            Vector2 proximo = Vector2.MoveTowards(rb.position, destino, velocidade * Time.fixedDeltaTime);
            rb.MovePosition(proximo);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        colidiu = true;
        direcaoBloqueada = ultimaDirecao;
    }

    private Vector2 EscolherDirecao()
    {
        Vector2[] opcoes = (Vector2[])direcoes.Clone();
        for (int i = 0; i < opcoes.Length; i++)
        {
            int j = Random.Range(i, opcoes.Length);
            (opcoes[i], opcoes[j]) = (opcoes[j], opcoes[i]);
        }

        foreach (Vector2 d in opcoes)
        {
            if (d != direcaoBloqueada)
                return d;
        }

        return direcoes[0];
    }

    private void SetWalking(bool value)
    {
        if (anim != null) anim.SetBool("isWalking", value);
    }

    private void SetFacing(Vector2 direcao)
    {
        if (anim == null) return;

        for (int i = 0; i < 4; i++) anim.SetLayerWeight(i, 0);

        if (direcao.x > 0) anim.SetLayerWeight(2, 1);
        else if (direcao.x < 0) anim.SetLayerWeight(1, 1);
        else if (direcao.y > 0) anim.SetLayerWeight(3, 1);
        else anim.SetLayerWeight(0, 1);
    }
}