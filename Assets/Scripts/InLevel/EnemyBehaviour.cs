using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    Transform trPlayer;
    Animator animator;
    Rigidbody rb;
    public GameObject goHitterTomate, goCorazon;

    public float flVelocMovim;
    public int inHealth;

    public Vector2 v2Destino;

    bool isStopped = false;
    bool canAct = false, blHurt = false;
    bool atackedOnce = false;

    //          VARIABLES FUNCION
    bool isAngryPimiento()
    {
        return ((kind == kEnemy.Pimiento) && (atackedOnce));
    }
    Vector3 nearestPoint()
    {
        float f = flVelocMovim / 2;
        Vector3[] vectores =
        {
            transform.position + new Vector3(f, 0, 0),
            transform.position + new Vector3(-f, 0, 0),
            transform.position + new Vector3(0, f, 0),
            transform.position + new Vector3(0, -f, 0),

            transform.position + new Vector3(f, f, 0),
            transform.position + new Vector3(-f, f, 0),
            transform.position + new Vector3(f, -f, 0),
            transform.position + new Vector3(-f, -f, 0)
        };
        Vector3 resultado = new Vector3();
        float distActual = flVelocMovim * 200;

        bool[] pasables =
        {
            false, false, false, false,
            false, false, false, false
        };
        int index = 0;

        foreach (var item in vectores)
        {
            RaycastHit hiti;
            if (Physics.Linecast(transform.position, item, out hiti))
            {
                pasables[index] = false;
            }
            else
            {
                pasables[index] = true;
            }

            index += 1;
        }

        index = 0;

        foreach (var item in pasables)
        {
            if (item)
            {
                float distancia = Vector3.Distance(vectores[index], trPlayer.position);
                if (distancia < distActual)
                {
                    distActual = distancia;
                    resultado = vectores[index];
                }
            }
            index += 1;
        }
        Debug.DrawLine(transform.position, resultado, Color.black);
        return resultado;
    }

    //          ENUMS, STRUCTS...
    public enum kEnemy
    {
        Brocoli, Pimiento, Tomate, Cebolla
    }
    public kEnemy kind;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ienVarReferences());
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "P_RastrilloHit":
                if (blHurt == false)
                {
                    Instantiate(GameManager.GM.efectos[0], Vector3.MoveTowards(transform.position, other.gameObject.transform.position, (Vector3.Distance(transform.position, other.gameObject.transform.position))) + new Vector3(0, 0, -1), new Quaternion());
                    if (other.gameObject.GetComponentInParent<RastrilloBehaviour>().state == RastrilloBehaviour.statRast.disparo)
                    {
                        switch (kind)
                        {
                            case kEnemy.Cebolla:
                                if (inHealth > 1)
                                {
                                    RastrilloBehaviour rastrillo = other.gameObject.GetComponentInParent<RastrilloBehaviour>();
                                    rastrillo.RastChocar();
                                    rastrillo.gameObject.transform.parent = this.gameObject.transform;
                                }
                                else
                                {
                                    inHealth -= 4;
                                }
                                break;

                            default:
                                inHealth -= 4;
                                break;
                        }
                    }
                    StartCoroutine(ienHurt());
                }
                break;

            default:
                break;
        }
    }

    void DecidirMov(GameObject go, Vector3 v2 = new Vector3())
    {
        Debug.DrawLine(transform.position, ((v2 != Vector3.zero) ? v2 : go.transform.position), Color.red);
        switch (go.tag)
        {
            case "Player":
            case "P_Rastrillo":
            case "P_RastrilloHit":
                if (Vector3.Distance(transform.position, go.transform.position) <= 1.5f)
                {
                    switch (kind)
                    {
                        case kEnemy.Tomate:
                            StartCoroutine(ienHurt());
                            break;

                        default:
                            break;
                    }
                    
                }
                else
                {
                    UpdateGoal(trPlayer.position);
                }
                break;

            case "Enemy":
                UpdateGoal(nearestPoint());
                break;

            default:
                if (go.layer != 2)
                {
                    UpdateGoal(trPlayer.position);
                }
                else
                {
                    UpdateGoal(nearestPoint());
                }
                break;
        }
    }

    void UpdateGoal(Vector2 objetivo)
    {
        v2Destino = objetivo;
        isStopped = false;
    }

    void StopWalking()
    {
        isStopped = true;
    }
    IEnumerator ienVarReferences()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        trPlayer = GameManager.GM.obPlayer.transform;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        canAct = true;
        StartCoroutine(ienUpdate());
    }

    IEnumerator ienUpdate()
    {
        float Seg = 0.025f;
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if ((canAct) && (blHurt == false))
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, trPlayer.position, out hit))
                {
                    DecidirMov(hit.collider.gameObject, hit.point);
                }
                else
                {
                    DecidirMov(trPlayer.gameObject);
                }

                transform.position = Vector3.MoveTowards(transform.position, v2Destino, (isStopped) ? 0 : (flVelocMovim * ((isAngryPimiento()) ? 2f : 1)) * (Seg));

                if (trPlayer.position.x < transform.position.x)
                {
                    transform.rotation = new Quaternion(0, 180, 0, 0);
                }
                else
                {
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                }
            }

            yield return new WaitForSeconds(Seg);
        }
    }

    void GenerarCorazon()
    {
        int aleatorio = Random.Range(0, 4 + GameManager.GM.oleadas);
        bool ProbCierta(int suelo, int techo)
        {
            return (aleatorio >= suelo) && (aleatorio <= techo);
        }

        if (ProbCierta(0, 8 - GameManager.GM.nivel))
        {
            Instantiate(goCorazon, transform.position, new Quaternion());
        }
    }

    IEnumerator ienHurt()
    {
        if (blHurt)
        {
            StopCoroutine(ienHurt());
        }


        blHurt = true;
        inHealth -= 1;
        
        
        if (inHealth <= 0)
        {
            Collider[] colliders = GetComponents<Collider>();
            animator.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Encima";
            foreach (var item in colliders)
            {
                item.enabled = false;
            }
            
            switch (kind)
            {
                case kEnemy.Brocoli:
                case kEnemy.Cebolla:
                    animator.Play("Defeat");
                    GenerarCorazon();
                    yield return new WaitForSeconds(1f);
                    GameManager.GM.enemiDef += 1;
                    Destroy(this.gameObject);
                    break;

                case kEnemy.Pimiento:
                    if (isAngryPimiento())
                    {
                        animator.Play("ADefeat");
                    }
                    else
                    {
                        animator.Play("HDefeat");
                    }
                    GenerarCorazon();
                    yield return new WaitForSeconds(1f);
                    GameManager.GM.enemiDef += 1;
                    Destroy(this.gameObject);
                    break;

                case kEnemy.Tomate:
                    animator.SetBool("boom", true);
                    yield return new WaitForSeconds(1f);

                    GameObject charco = Instantiate(goHitterTomate, transform.position, new Quaternion());
                    float taman = Random.Range(-0.4f, 0.4f);
                    charco.transform.localScale += new Vector3(taman, taman, 0);

                    yield return new WaitForSeconds(1f);
                    GameManager.GM.enemiDef += 1;
                    Destroy(this.gameObject);
                    break;
                

                default:
                    break;
            }
        }
        else
        {
            
            switch (kind)
            {
                case kEnemy.Brocoli:
                case kEnemy.Cebolla:
                    animator.Play("Hit");
                    yield return new WaitForSeconds(0.2f);
                    animator.Play("Walk");

                    blHurt = false;
                    break;

                case kEnemy.Pimiento:
                    if (atackedOnce)
                    {
                        animator.Play("Hit");
                        yield return new WaitForSeconds(0.2f);
                        animator.Play("AWalk");

                        blHurt = false;
                    }
                    else
                    {
                        atackedOnce = true;
                        animator.Play("Trans");
                        yield return new WaitForSeconds(2f);

                        //Debug.Log("Ay que dolor por tu culpa ahora tengo solo " + inHealth + " HP");
                        animator.Play("AWalk");

                        blHurt = false;
                    }

                    
                    break;

                case kEnemy.Tomate:
                    break;

                default:
                    break;
            }
        }

        
    }
}
