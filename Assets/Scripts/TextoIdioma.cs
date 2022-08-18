using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]

public class TextoIdioma : MonoBehaviour
{
    Text text;

    public int IDFrase = 0;
    int idioma = 0;
    string[] eng =
    {
        "START",
        "OPTIONS",
        "QUIT",
        "LANGUAGE",
        "CREDITS",
        "BACK",
        "-< CREDITS >-",
        "-- Coding, art, music and design --\n Oscar Moreno (Asdonaur)\n\n  -- Sounds --\n Creative Commons\n \n -- Used software --\n Unity 2020\n Piskel\n Paint.NET\n Audacity\nBoscaCeoil",
        "GAME OVER",
        "Waves survived:",
        "Your record:",
        "RETRY",
        "MAIN MENU"
    };
    string[] esp =
    {
        "EMPEZAR",
        "OPCIONES",
        "SALIR",
        "IDIOMA",
        "CREDITOS",
        "VOLVER",
        "-< CREDITOS >-",
        "-- Programación, arte, música y diseño --\n Oscar Moreno (Asdonaur)\n\n  -- Sonidos --\n Creative Commons\n \n -- Programas usados --\n Unity 2020\n Piskel\n Paint.NET\n Audacity\nBoscaCeoil",
        "JUEGO TERMINADO",
        "Oleadas sobrevividas:",
        "Tu record:",
        "REINTENTAR",
        "MENÚ"
    };

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        AjustarIdioma();
    }

    private void Update()
    {
        if (idioma != PlayerPrefs.GetInt("idioma", 0))
        {
            AjustarIdioma();
        }
    }

    public void AjustarIdioma()
    {
        idioma = PlayerPrefs.GetInt("idioma", 0);
        switch (idioma)
        {
            case 0: // Inglés
            default:
                text.text = eng[IDFrase];
                break;

            case 1: // Español
                text.text = esp[IDFrase];
                break;
        }
    }
}
