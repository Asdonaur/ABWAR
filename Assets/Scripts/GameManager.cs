using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    AudioSource audSrc;
    public AudioSource audSrcMusic;

    public Vector2 v2Mouse;

    public GameObject obPlayer;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject obCamPoint;
    public GameObject obCebolla;
    public GameObject obCanvasLose;
    public GameObject obCanvasTrans;
    public GameObject[] enemigos,
        puntosSpawn,
        efectos;

    public int oleadaCebolla = 3,
        factorSuma = 2;
    public int enemigosMax = 10;

    bool spawneando = false;
    public bool level = true;
    bool lost = false;

    public AudioClip bgmNormal, bgmIntense, seRecord;

    public int salud;
    [SerializeField] public int oleadas;
    public int nivel;
    public int ToSpawn,
        enemiDef;
    public int vidaVirtual = 0;

    bool BatallaIntensa()
    {
        return (oleadas >= 5) && (salud <= 2);
    }
    bool musicCamb = false;

    // Start is called before the first frame update
    void Start()
    {
        GM = this;
        audSrc = GetComponent<AudioSource>();
        

        if (level)
        {
            audSrcMusic.clip = bgmNormal;
            audSrcMusic.Play();

            obPlayer = GameObject.FindGameObjectWithTag("Player");

            salud = 8;
            oleadas = 0;
            nivel = 0;
            ToSpawn = 0;
            enemiDef = 0;
            factorSuma = 2;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((level) && (!lost))
        {
            v2Mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GUILevel.scr.txtOleadas)
            {
                GUILevel.scr.txtOleadas.text = oleadas + "";
            }
            

            if (enemiDef >= ToSpawn)
            {
                StartCoroutine(ienSpawnEnemies());
                enemiDef = 0;
            }

            if ((BatallaIntensa()) && (!musicCamb))
            {
                musicCamb = true;
                StartCoroutine(ienChangeMusic());
            }
            else if (lost)
            {
                StopCoroutine(ienSpawnEnemies());
            }

            vidaVirtual = salud;
        }

        if ((Input.GetKey(KeyCode.LeftShift)) && (Input.GetKey(KeyCode.RightShift)))
        {
            PlayerPrefs.SetInt("inRecord", 0);
        }
    }

    public void CambiarVidas(int valor)
    {
        salud += valor;

        if (salud > 8)
        {
            salud = 8;
        }
        if (salud < 1)
        {
            salud = 0;
            vLose();
        }
        GUILevel.scr.ActualizarVidas(salud);
    }

    public void PlaySE(AudioClip clip)
    {
        audSrc.PlayOneShot(clip);
    }

    public void vLose()
    {
        StartCoroutine(ienLose());
    }

    public void ChangeScene(string index)
    {
        Transition transicion = Instantiate(obCanvasTrans).GetComponent<Transition>();
        transicion.scene = index;
    }

    IEnumerator ienSpawnEnemies()
    {
        spawneando = true;
        enemiDef = 0;

        if (oleadas % oleadaCebolla == 0)
        {
            nivel += 1;
        }
        oleadas += 1;
        factorSuma += Random.Range(0, 1);
        ToSpawn += (ToSpawn >= enemigosMax) ? -(Random.Range(5, (enemigosMax / 2))) : factorSuma;
        enemigosMax += 1;

        if (oleadas % oleadaCebolla == 0)
        {
            ToSpawn += 1;
            Instantiate(obCebolla, puntosSpawn[Random.Range(0, 2)].transform.position, new Quaternion());
        }

        int Spawneados = 0;

        // CAMBIAR BANDERA
        if ((oleadas % oleadaCebolla) == 0)
        {
            GUILevel.scr.CambiarBandera(true);
        }
        else if ((oleadas - 1) % oleadaCebolla == 0)
        {
            GUILevel.scr.CambiarBandera(false);
        }

        // ESPAWNEAR ENEMIGOS
        while (Spawneados < ToSpawn)
        {
            int medida = enemigos.Length;
            int cual = Random.Range(0, ((nivel <= medida) ? nivel : medida));
            int medida2 = puntosSpawn.Length;

            Instantiate(enemigos[cual], puntosSpawn[Random.Range(0, medida2)].transform.position, new Quaternion());
            Spawneados += 1;
            yield return new WaitForSeconds((2.75f - ((oleadas - 1) * 0.2f)) + Random.Range(0, 0.25f));
            yield return null;
        }
        spawneando = false;
    }

    IEnumerator  ienSpawnLoseScreen()
    {
        bool recor = false;
        GameObject obCanvasN = Instantiate(obCanvasLose);
        yield return new WaitForSeconds(0.01f);

        foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            EnemyBehaviour enemy = item.GetComponent<EnemyBehaviour>();
            if (enemy)
            {
                enemy.flVelocMovim = 0;
                enemy.gameObject.GetComponentInChildren<Animator>().speed = 0;
            }
        }
        int oleadasActuales = oleadas - 1;
        int oleadasRecord = PlayerPrefs.GetInt("inRecord", 0);

        GameObject.Find("txtWavesActN").GetComponent<Text>().text = oleadasActuales.ToString();
        if (oleadasActuales > oleadasRecord)
        {
            GameObject.Find("txtRecord").GetComponent<Text>().text = "RECORD!!";
            PlayerPrefs.SetInt("inRecord", oleadasActuales);
            oleadasRecord = oleadasActuales;
            recor = true;
        }
        else
        {
            GameObject.Find("txtRecord").GetComponent<Text>().text = " ";
        }
        GameObject.Find("txtWavesRecN").GetComponent<Text>().text = oleadasRecord.ToString();

        if (recor)
        {
            yield return new WaitForSeconds(0.5f);
            PlaySE(seRecord);
        }
    }

    IEnumerator ienChangeMusic()
    {
        audSrcMusic.Stop();
        audSrcMusic.clip = bgmIntense;
        yield return new WaitForSeconds(0.75f);
        audSrcMusic.Play();
    }

    IEnumerator ienLose()
    {
        lost = true;
        audSrcMusic.Stop();
        obPlayer.GetComponent<PlayerMovement>().estado = PlayerMovement.sPlayer.Derrota;
        obPlayer.GetComponentInChildren<Animator>().Play("Defeat");
        StopCoroutine(ienSpawnEnemies());
        foreach (var item in GameObject.FindGameObjectsWithTag("P_Rastrillo"))
        {
            RastrilloBehaviour script = item.GetComponent<RastrilloBehaviour>();
            if (script)
            {
                script.state = RastrilloBehaviour.statRast.inactivo;
            }
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(ienSpawnLoseScreen());
    }

    public IEnumerator ienTemblor(float intensity = 5f, float time = 0.25f)
    {
        CinemachineBasicMultiChannelPerlin cbmcp =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmcp.m_AmplitudeGain = intensity;
        float numActual = intensity;

        yield return new WaitForSeconds(time);
        cbmcp.m_AmplitudeGain = 0f;
    }
}
