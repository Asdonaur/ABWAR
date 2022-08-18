using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RastrilloBehaviour : MonoBehaviour
{
    //              OBJETOS Y COMPONENTES
    Collider col;
    Rigidbody rb;
    Animator animator;
    PlayerMovement plMvm;
    public SpriteRenderer borde;

    GameObject oPlayer,
        oPadre;
    public GameObject obBoxColid;

    public AudioClip sndChoque;

    //              Numericos y vectores

    Vector3 objetivo;
    Vector3 v3Stop;
    Quaternion quStop;

    public float flVelocRotar;
    public float flVelocDisparo, flVDA;

    public bool blDamage;
    bool FueraDelMapa()
    {
        float x = transform.position.x,
            y = transform.position.y,
            bigger = (x > y) ? x : y;

        return (bigger >= 30);
    }

    public enum statRast
    {
        mano, disparo, suelto, inactivo
    }
    public statRast state = statRast.mano;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        oPlayer = GameManager.GM.obPlayer;
        oPadre = transform.parent.gameObject;
        plMvm = oPlayer.GetComponent<PlayerMovement>();

        animator.Play("Idle");
        StartCoroutine(ienUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        objetivo = GameManager.GM.v2Mouse;
        if (state == statRast.mano)
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.Play("Attack");
            }
            if (Input.GetMouseButtonUp(0))
            {
                animator.Play("Idle");
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (plMvm.estado != PlayerMovement.sPlayer.Atacado)
                {
                    RastShoot();
                }
            }
        }
    }

    IEnumerator ienUpdate()
    {
        float secondrate = 0.02f;
        while (true)
        {
            Vector2 v3Apuntar = objetivo - transform.position;

            switch (state)
            {
                case statRast.mano:
                    borde.color = new Color(1, 1, 1, 0);
                    col.isTrigger = true;
                    rb.freezeRotation = false;
                    transform.right = v3Apuntar;
                    transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, 0, transform.rotation.z, transform.rotation.w), flVelocRotar);
                    transform.localPosition = new Vector3(0, 0, -0.5f);
                    break;

                case statRast.disparo:
                    animator.speed = 0;
                    flVDA = 1;
                    CambiarColisiones(true);
                    DoRaycast01();

                    break;

                case statRast.suelto:
                    animator.speed = 1;
                    col.isTrigger = true;
                    animator.Play("Idle");
                    borde.color = new Color(1, 1, 1, 1);
                    CambiarColisiones(false);

                    if (transform.parent == null)
                    {
                        AccionesSuelto();
                    }
                    else if (transform.parent.gameObject.tag == "Enemy")
                    {
                        // SI EL PADRE ES UNA CEBOLLA, ENTONCES SE ASEGURA QUE NO ESTÉ A KILOMETROS
                        GameObject obCebolla = transform.parent.gameObject;
                        int mirada = (obCebolla.transform.rotation.y == 180) ? -1 : 1;

                        if (Vector3.Distance(this.gameObject.transform.position, obCebolla.transform.position) > 0.8f)
                        {
                            transform.position = Vector3.MoveTowards(transform.position, obCebolla.transform.position, 0.25f);
                        }
                        else
                        {
                            AccionesSuelto();
                        }
                    }
                    else
                    {
                        AccionesSuelto();
                    }

                    if (Vector3.Distance(transform.position, GameManager.GM.obPlayer.transform.position) < 1.75f)
                    {
                        col.isTrigger = true;
                        transform.parent = oPadre.transform;
                        plMvm.Calmarse();
                        state = statRast.mano;
                    }
                    break;

                case statRast.inactivo:
                    borde.color = new Color(1, 1, 1, 0);
                    animator.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                    animator.Play("Idle");
                    CambiarColisiones(false);
                    break;
            }

            if (FueraDelMapa())
            {
                transform.position = new Vector3();
            }
            yield return new WaitForSeconds(secondrate);
        }
    }

    void RastShoot()
    {
        Debug.Log("dispara we");
        RastShoot1();
    }

    void RastShoot1()
    {
        state = statRast.disparo;
        StartCoroutine(plMvm.ienShoot());
        rb.freezeRotation = true;
        transform.parent = null;
        rb.AddRelativeForce(new Vector3(flVelocDisparo, 0, 0), ForceMode.Impulse);
    }

    public void RastChocar()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        AccionesSuelto();
        col.isTrigger = false;
        state = statRast.suelto;
        GameManager.GM.PlaySE(sndChoque);
    }

    void AccionesSuelto()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void DoRaycast01()
    {
        Vector3 ve01 = transform.localPosition,
                    ve02 = ve01 + transform.right * -0.5f,
                    ve03 = ve01 + transform.right * 0.75f;

        RaycastHit hit;
        if (Physics.Linecast(ve02, ve03, out hit))
        {
            switch (hit.collider.gameObject.tag)
            {
                case "Enemy":
                case "Charco":
                    EnemyBehaviour eb = hit.collider.gameObject.GetComponent<EnemyBehaviour>();
                    if (eb)
                    {
                        if ((eb.kind == EnemyBehaviour.kEnemy.Cebolla))
                        {
                            RastChocar();
                        }
                    }
                    break;

                default:
                    RastChocar();
                    break;
            }
        }
    }

    public void CambiarColisiones(bool a)
    {
        obBoxColid.tag = (a == true) ? "P_RastrilloHit" : "P_Rastrillo";
    }
}
