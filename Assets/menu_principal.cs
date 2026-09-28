using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_principal : MonoBehaviour
{
    [SerializeField] private string nomeDoJogo;

    public void Jogar(){
        SceneManager.LoadScene(nomeDoJogo);
    }

    public void Sair(){
        Debug.Log("Sair do jogo");
        Application.Quit();
    }
}
