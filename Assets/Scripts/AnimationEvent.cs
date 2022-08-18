using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    SpriteRenderer sprRen;
    int sorting;

    private void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();
        
        if (sprRen != null)
        {
            sorting = sprRen.sortingOrder;
        }
    }

    private void Update()
    {
        if (sprRen != null)
        {
            sprRen.sortingOrder = Mathf.RoundToInt((gameObject.transform.parent.gameObject.transform.position.y * 100) * -1) + sorting;
        }
        
    }

    void PlaySound(AudioClip index)
    {
        GameManager.GM.PlaySE(index);
    }

    public void RastrilloHurt(int doler)
    {
        transform.parent.GetComponent<RastrilloBehaviour>().CambiarColisiones((doler == 1) ? true : false);
    }

    void PlayAnimation(string index)
    {
        this.GetComponent<Animator>().Play(index);
    }

    void CambiarVelocidad(float index)
    {
        Time.timeScale = index;
    }

    void Temblar(float intenso = 2f)
    {
        StartCoroutine(GameManager.GM.ienTemblor(intenso));
    }

    void Destruirse()
    {
        Destroy(gameObject);
    }
}
