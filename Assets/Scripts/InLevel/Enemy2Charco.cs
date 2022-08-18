using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Charco : MonoBehaviour
{
    public float flSegundos = 15,
        flSegundos2 = 5;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enumerator());
    }

    IEnumerator enumerator()
    {
        yield return new WaitForSeconds(flSegundos);
        SpriteRenderer sprRen = GetComponent<SpriteRenderer>();
        if (sprRen == null)
        {
            sprRen = GetComponentInChildren<SpriteRenderer>();
        }
        sprRen.color = new Color(sprRen.color.r, sprRen.color.g, sprRen.color.b, 0.5f);
        yield return new WaitForSeconds(flSegundos2);
        Destroy(this.gameObject);
    }
}
