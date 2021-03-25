using UnityEngine;
using Valve.VR.InteractionSystem;

public class ChestBehavior : MonoBehaviour
{
    [Tooltip("The key that will be used as trigger.")]
    public GameObject key;

    private CircularDrive chestTopDrive;

    // Start is called before the first frame update
    void Start()
    {
        chestTopDrive = transform.parent.GetComponentInChildren<CircularDrive>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == key)
        {
            chestTopDrive.enabled = true;
            key.GetComponent<Interactable>().attachedToHand.DetachObject(key);
            key.GetComponent<Interactable>().enabled = false;
            key.GetComponent<Throwable>().enabled = false;
            key.GetComponent<Collider>().enabled = false;
            key.GetComponent<Rigidbody>().isKinematic = true;
            key.transform.parent.eulerAngles = new Vector3(0, 180, 90);
            key.transform.position = transform.position + transform.right / 10;
            key.transform.parent.SetParent(transform, true);
            GetComponent<AudioSource>().Play();
        }
    }
}
