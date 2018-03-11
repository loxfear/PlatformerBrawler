using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyArea : MonoBehaviour
{
    public Renderer render;
    public Material mat;
    public Color baseColor;
    public float emission;
    public float speed;
    public bool stay;
    // animate the game object from -1 to +1 and back
    public float minimum = 0;
    public float maximum = 1.0F;

    // starting value for the Lerp
    public float t = 0.0f;


    // Use this for initialization
    void Start()
    {
        render = GetComponentInParent<Renderer>();
        mat = render.material;
        emission = 0;
        stay = false;
    }

    // Update is called once per frame
    void Update()
    {
        // animate the position of the game object...
        emission = Mathf.Lerp(minimum, maximum, t);


        if(!stay)
        {
            if (t > 0)
            {
                t -= Time.deltaTime * speed;
            }
        }

            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            mat.SetColor("_EmissionColor", finalColor);
        }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            stay = true;
            if (t < 1)
            {
                t += Time.deltaTime * speed;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            stay = false;
        }
    }
   
       /*  IEnumerator EnteredTrigger()
    {
        runningCorutine = true;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            yield return null;
        }
        runningCorutine = false;
        yield return null;
    }
    IEnumerator LeftTrigger()
    {
        runningCorutine = true;
        while (t > 0)
        {
            t -= Time.deltaTime * speed;
            yield return null;
        }
        runningCorutine = false;
        yield return null;
    }*/
}