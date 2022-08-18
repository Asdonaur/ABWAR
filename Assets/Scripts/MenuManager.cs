using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator animator;
    public AudioClip sePress;

    bool presionado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressButton(string kind)
    {
        if (!presionado)
        {
            StartCoroutine(ienPressButton(kind));
        }
        
    }

    public IEnumerator ienPressButton(string kind)
    {
        presionado = true;
        GameManager.GM.PlaySE(sePress);
        yield return new WaitForSeconds(0.2f);
        switch (kind)
        {
            case "start":
                GameManager.GM.ChangeScene("SampleScene");
                break;

            case "options":
                animator.SetBool("options", true);
                break;

            case "optLang":
                switch (PlayerPrefs.GetInt("idioma", 0))
                {
                    case 0:
                        PlayerPrefs.SetInt("idioma", 1);
                        break;

                    case 1:
                        PlayerPrefs.SetInt("idioma", 0);
                        break;

                    default:
                        break;
                }
                break;

            case "optCred":
                animator.SetBool("credits", true);
                break;

            case "credBack":
                animator.SetBool("credits", false);
                break;

            case "optBack":
                animator.SetBool("options", false);
                break;

            case "quit":
                Application.Quit();
                break;

            case "lostRetry":
                GameManager.GM.ChangeScene("SampleScene");
                break;

            case "lostQuit":
                GameManager.GM.ChangeScene("MainMenu");
                break;
        }
        presionado = false;
    }
}
