using UnityEngine;
using UnityEditor;
using System;

public enum AlertStage

{
    Rauhallinen,
    Kiinostunut,
    Huomannut
}

public class Patrol : MonoBehaviour
{

    public Transform player;
    private float AlertRange = 5f;
    public Transform[] waypointit;
    public int wayPointIndexi;
    public int nopeus = 3;

    private float AlertGainSekunnit = 40f;
    private float AlertLossSekunnit = 25f;

    private float OdotusAika = 1f;
    private float OdotusAjastin = 0f;

    public AlertStage stage;
    [Range(0f, 100f)]
    public float AlertLevel;



    private void Awake()
    {
        stage = AlertStage.Rauhallinen;
        AlertLevel = 0;

    }


    private bool odotus = false;

    public float fov;

    private void OnDrawGizmos()


    {



        float t = AlertLevel / 100f;

        Color c = Color.Lerp(Color.green, Color.red, t);
        c.a = 0.25f;

        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, AlertRange);



    }



    private void AlertLevels()
    {
        if (player == null) return;

        float disti = Vector3.Distance(transform.position, player.position);
        bool pelaajaRangessa = disti <= AlertRange;

        bool visible = pelaajaRangessa && LineOfSight();

        if (visible)
        {
            AlertLevel += AlertGainSekunnit * Time.deltaTime;
        }

        else
        {
            AlertLevel -= AlertLossSekunnit * Time.deltaTime;
        }

        AlertLevel = Mathf.Clamp(AlertLevel, 0, 100f);

        if (AlertLevel < 33f)
        {
            stage = AlertStage.Rauhallinen;


        }
        else if (AlertLevel < 66f)
            stage = AlertStage.Kiinostunut;

        else
        {
            stage = AlertStage.Huomannut;

            transform.position = Vector3.MoveTowards(transform.position, player.position, nopeus * Time.deltaTime);

        }
    }

    private bool LineOfSight()
    {
        if (player == null) return false;

        Vector2 Suunta = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        int mask = LayerMask.GetMask("Wall");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Suunta, dist, mask);

        if (hit.collider != null) return false;

        return true;
        
          
        

        
    }





    void Update()
    {

        AlertLevels();


        if (stage == AlertStage.Huomannut)
            return;



        Transform wp = waypointit[wayPointIndexi];





        if (odotus)
        {
            OdotusAjastin -= Time.deltaTime;
            if (OdotusAjastin <= 0f)
            {
                odotus = false;
                wayPointIndexi = (wayPointIndexi + 1) % waypointit.Length;
            }

            return;

        }

        transform.position = Vector3.MoveTowards(transform.position, wp.position, nopeus * Time.deltaTime);


        if (Vector3.Distance(transform.position, wp.position) < 0.01f)
        {
            odotus = true;
            OdotusAjastin = OdotusAika;
        }










    }
}

