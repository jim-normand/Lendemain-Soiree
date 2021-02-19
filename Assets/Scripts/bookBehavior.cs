using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class BookBehavior : MonoBehaviour
{
    //public bool isHeld;
    //public bool firstHold;
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        //isHeld = false;
        //firstHold = true;
        anim = GetComponentInChildren<Animation>();
        GetComponent<Throwable>().onPickUp.AddListener(OpenBook);
        GetComponent<Throwable>().onDetachFromHand.AddListener(CloseBook);
    }

    // Update is called once per frame
    void Update()
    {
        /*isHeld = GetComponent<Throwable>().GetAttached();

        if (isHeld)
        {
            if (firstHold)
            {
                anim.Play("bookOpen");      //déclenche l'animation d'ouverture du livre
            }
            
            firstHold = false;
        }
        else if (!firstHold)
        {
            anim.Play("bookClose");         //déclenche l'animation de fermeture du livre
            firstHold = true;
        }*/
    }

    /// <summary>
    /// Play book opening animation
    /// </summary>
    void OpenBook()
    {
        anim.Play("bookOpen");
    }

    void CloseBook()
    {
        anim.CrossFade("bookClose", .1f);
    }
}
