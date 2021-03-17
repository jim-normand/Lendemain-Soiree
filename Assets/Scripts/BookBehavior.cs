using UnityEngine;
using Valve.VR.InteractionSystem;

public class BookBehavior : MonoBehaviour
{
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animation>();
        //GetComponent<Throwable>().onPickUp.AddListener(OpenBook);
        //GetComponent<Throwable>().onDetachFromHand.AddListener(CloseBook);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Play book opening animation
    /// </summary>
    public void OpenBook()
    {
        anim.CrossFade("bookOpen");
    }

    public void CloseBook()
    {
        anim.CrossFade("bookClose");
    }
}
