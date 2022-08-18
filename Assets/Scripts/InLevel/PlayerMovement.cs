using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //          COMPONENTES Y OBJETOS
    CharacterController chrCnt;
    Animator animator;
    SpriteRenderer sprRen;
    RastrilloBehaviour rasBeh;

    //          SONIDOS
    public AudioClip seHeart;

    //          NUMEROS
    public float flVelocMovim;
    float axisX, axisY;
    Vector3 v3MovePlayer;

    public bool blRecover = false;

    public enum sPlayer
    {
        Rastrillo, Indefenso, Atacado, Victoria, Derrota, Accion
    }
    public sPlayer estado = sPlayer.Rastrillo;

    // Start is called before the first frame update
    void Start()
    {
        chrCnt = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sprRen = GetComponentInChildren<SpriteRenderer>();
        rasBeh = GetComponentInChildren<RastrilloBehaviour>();

        StartCoroutine(ienUpdate());
        StartCoroutine(ienUpdateCharCnt());
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
            case "Charco":
                vHurt();
                break;

            case "Heart":
                if (estado != sPlayer.Atacado)
                {
                    GameManager.GM.PlaySE(seHeart);
                    GameManager.GM.CambiarVidas(2);
                    Destroy(other.gameObject);
                    
                }
                else
                {
                    // nada...
                }
                break;

            default:
                break;
        }
    }

    public void vHurt()
    {
        if ((blRecover == false) && (estado != sPlayer.Atacado))
        {
            StartCoroutine(ienHurt());
        }
    }

    public IEnumerator ienShoot()
    {
        animator.Play("Shoot");
        estado = sPlayer.Accion;
        yield return new WaitForSeconds(0.5f);
        
        if (rasBeh.state == RastrilloBehaviour.statRast.mano)
        {
            estado = sPlayer.Rastrillo;
            animator.Play("NIdle");
        }
        else
        {
            estado = sPlayer.Indefenso;
            animator.Play("SIdle");
        }
    }

    public void Calmarse()
    {
        animator.Play("NIdle");
        estado = sPlayer.Rastrillo;
    }

    IEnumerator ienUpdate()
    {
        yield return null;
        float Sec = 0.015f;

        while (true)
        {
            switch (estado)
            {
                case sPlayer.Rastrillo:
                case sPlayer.Indefenso:
                    if (GameManager.GM.v2Mouse.x < transform.position.x)
                    {
                        animator.transform.parent.transform.rotation = new Quaternion(0, 180, 0, 0);
                    }
                    else
                    {
                        animator.transform.parent.transform.rotation = new Quaternion(0, 0, 0, 0);
                    }

                    axisX = Input.GetAxis("Horizontal");
                    axisY = Input.GetAxis("Vertical");
                    int factorMult = (estado == sPlayer.Indefenso) ? 2 : 1;

                    v3MovePlayer = new Vector3(axisX * (flVelocMovim * factorMult), axisY * (flVelocMovim * factorMult), 0);

                    if (v3MovePlayer == new Vector3())
                    {
                        animator.SetInteger("stateMove", 0);
                    }
                    else
                    {
                        animator.SetInteger("stateMove", 1);
                    }

                    if (chrCnt)
                    {
                        chrCnt.Move(v3MovePlayer * (Sec));
                    }
                    break;
            }

            yield return new WaitForSeconds(Sec);
        }
    }

    public IEnumerator ienHurt()
    {
        blRecover = true;
        estado = sPlayer.Atacado;
        GameManager.GM.CambiarVidas(-1);

        if (GameManager.GM.salud == 0)
        {
            animator.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Encima";
            estado = sPlayer.Derrota;
        }
        else
        {
            animator.Play("Hit");
            yield return new WaitForSeconds(0.3f);

            if (rasBeh.state == RastrilloBehaviour.statRast.mano)
            {
                estado = sPlayer.Rastrillo;
                animator.Play("NIdle");
            }
            else
            {
                estado = sPlayer.Indefenso;
                animator.Play("SIdle");
            }
            animator.Play((estado == sPlayer.Rastrillo) ? "NIdle" : "SIdle");
            float tim = 0, timm = 2, timf = 0.2f;
            Color colorin = sprRen.color;

            while (tim < timm)
            {
                sprRen.color = new Color(colorin.r, colorin.g, colorin.b, (colorin.a == 1) ? 0.5f : 1f);
                tim += timf;
                yield return new WaitForSeconds(0.2f);
            }

            sprRen.color = new Color(colorin.r, colorin.g, colorin.b, 1f);
            blRecover = false;
        }
        
    }

    IEnumerator ienUpdateCharCnt()
    {
        while(true)
        {
            yield return new WaitForSeconds(30f + Random.Range(-15f, 15f));
            float RADIO = chrCnt.radius,
                ALTURA = chrCnt.height;

            Destroy(chrCnt);
            yield return new WaitForSeconds(0.02f);
            CharacterController newChar = gameObject.AddComponent<CharacterController>();
            newChar.radius = RADIO;
            newChar.height = ALTURA;
            chrCnt = newChar;
        }
    }

}
