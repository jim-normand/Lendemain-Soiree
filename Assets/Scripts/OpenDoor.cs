using UnityEngine;
using Valve.VR.InteractionSystem;

public class OpenDoor : MonoBehaviour {

	public float smooth = 2.0f;
	public float DoorOpenAngle = 90.0f;

	public AudioClip OpenAudio;
	public AudioClip CloseAudio;
	private bool AudioS;

    private Transform rotateAnchor;
    private bool open;
	private bool enter = false;

    private PhoneBehavior phoneScript;

	// Use this for initialization
	void Start () {
        rotateAnchor = transform.parent;
        phoneScript = GameObject.FindWithTag("Phone").GetComponent<PhoneBehavior>();
    }
	
	// Update is called once per frame
	void Update () {
		if (open) {
            rotateAnchor.Rotate(Vector3.up, (DoorOpenAngle - rotateAnchor.rotation.eulerAngles.y) * Time.deltaTime * smooth);
            if (!AudioS)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(OpenAudio);
                AudioS = true;
            }
            
		} else {
            rotateAnchor.rotation = Quaternion.Slerp(rotateAnchor.rotation, Quaternion.identity, Time.deltaTime * smooth);
			if (AudioS) {
				gameObject.GetComponent<AudioSource> ().PlayOneShot (CloseAudio);
				AudioS = false;
			}
		}

        enter = GetComponent<Interactable>().isHovering;

		//peut s'ouvrir si on a résolu l'enigme du telephone et qu'on selectionne la porte
		if (!phoneScript.isLocked && enter)
        {
            open = true;
        }
    }

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Player")) {
			enter = true;
			}
		}

    void OnTriggerExit(Collider col)
    {
	    if (col.CompareTag("Player")) {
		    enter = false;
	    }
    }
}