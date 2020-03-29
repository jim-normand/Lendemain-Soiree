using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class bookBehavior : MonoBehaviour
{
    public bool isHeld;
    public bool firstHold;
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        isHeld = false;
        firstHold = true;
        anim = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        isHeld = GetComponent<Throwable>().GetAttached();

        if(isHeld)
        {
            if(firstHold)
            {
                anim.Play("bookOpen");      //déclenche l'animation d'ouverture du livre
            }
            
            firstHold = false;
        }
        else if(!firstHold)
        {
            anim.Play("bookClose");         //déclenche l'animation de fermeture du livre
            firstHold = true;
        }
    }
}
