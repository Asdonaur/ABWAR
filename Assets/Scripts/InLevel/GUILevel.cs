using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUILevel : MonoBehaviour
{
    public static GUILevel scr;

    public Image[] corazones;
    public Sprite[] sprsCorazones;

    public Text txtOleadas;
    public Image imgFlag;

    public Sprite sprBN, sprBC;
    bool stretching = false;

    Image imgEstirando;
    Vector3 tamEstirando;

    // Start is called before the first frame update
    void Start()
    {
        scr = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActualizarVidas(int index)
    {
        // Cada 2 es un corazon completo
        int num = 0; // Corazon actual
        foreach (var item in corazones)
        {
            
            num += 1;

            int num2 = (num) * 2; //num2 = corazon actual por 2
            if (index >= num * 2) // si la vida es mayor o igual al corazon actual por dos
            {
                item.sprite = sprsCorazones[2];
            }
            else
            {
                int num3 = num2 - index;
                switch (num3)
                {
                    case 1:
                        item.sprite = sprsCorazones[1];
                        break;

                    case 2:
                        item.sprite = sprsCorazones[0];
                        break;
                }
            }

            // Animar el corazon
            int val = num2 - 2;
            if ((index > val) && (index <= num2))
            {
                vSnS(item);
            }

        }
    }

    public void CambiarBandera(bool cebolla)
    {
        imgFlag.sprite = (cebolla) ? sprBC : sprBN;
        vSnS(imgFlag);
    }

    void vSnS(Image imagen)
    {
        if (stretching)
        {
            StopAllCoroutines();
            imgEstirando.GetComponent<RectTransform>().localScale = tamEstirando;
        }
        StartCoroutine(ienSquashStretchImage(imagen));
    }

    public IEnumerator ienSquashStretchImage(Image index, float intensity = 0.25f)
    {
        stretching = true;
        Vector2 size = index.GetComponent<RectTransform>().localScale;
        float factor = 0.2f;

        imgEstirando = index;
        tamEstirando = size;

        while (index.GetComponent<RectTransform>().localScale.x <= size.x + intensity)
        {
            Vector2 sizeActual = index.GetComponent<RectTransform>().localScale;
            index.GetComponent<RectTransform>().localScale = new Vector2(sizeActual.x + factor, sizeActual.y - factor);
            yield return new WaitForSeconds(0.05f);
        }

        while (index.GetComponent<RectTransform>().localScale.x >= size.x - (intensity / 2))
        {
            Vector2 sizeActual = index.GetComponent<RectTransform>().localScale;
            index.GetComponent<RectTransform>().localScale = new Vector2(sizeActual.x - factor, sizeActual.y + factor);
            yield return new WaitForSeconds(0.05f);
        }

        while (index.GetComponent<RectTransform>().localScale.x < size.x)
        {
            Vector2 sizeActual = index.GetComponent<RectTransform>().localScale;
            index.GetComponent<RectTransform>().localScale = new Vector2(sizeActual.x + factor, sizeActual.y - factor);
            yield return new WaitForSeconds(0.05f);
        }
        float tamano = (size.x + size.y) / 2;
        Vector2 sizeNew = new Vector2(tamano, tamano);
        index.GetComponent<RectTransform>().localScale = sizeNew;
        stretching = false;

    }
}
