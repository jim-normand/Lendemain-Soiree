using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;


public class PhoneBehavior : MonoBehaviour
{
    // Nombre total de boutons
    private const int numButtons = 12;

    //si le smartphone est tenu dans la main du joueur ou pas
    public bool isHeld;  
    public float angle; 
    public SteamVR_ActionSet actionSet;
    public SteamVR_Action_Vector2 menuScroll;
    public SteamVR_Action_Boolean selectNumber;
    public SteamVR_Input test;
    public Vector2 menuPosition;
    public bool isLocked = true;
    [Tooltip("End game message.")]
    public TextDisplay message;
    [Tooltip("Code entered by user.")]
    public Text tryCode;
    public Text screenText;
    public GameObject canvas;
    public Material screenLocked;

    private int idCurrentButton;
    private int idFormerButton;
    private float espacementBoutons;
    private float rayonBoutons;
    public bool initWheelOK;
    [SerializeField]
    [Tooltip("Code to unlcok phone.")]
    private string code = "4565"; 
    [Tooltip("Array of buttons prefabs")]
    public GameObject[] prefabButtons;
    [Tooltip("Array of buttons instances")]
    private GameObject[] instancedButtons;

    private GameObject[] screens;

    public void SelectNumber(Hand hand)
    {
        menuScroll.GetAxis(hand.handType);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Paramètres de la roue de chiffres
        espacementBoutons = 360 / numButtons;
        rayonBoutons = 0.1f;

        isHeld = false;
        tryCode.text = "";
        initWheelOK = true;

        //Par défault, c'est le bouton 'en haut' de la roue qui est sélectionné (bouton correction => g10)
        instancedButtons = new GameObject[numButtons];
        // Modifies instancedButtons
        InstantiateButtonsWheel();
        DisableButtonsWheel();
        idCurrentButton = 1;
        idFormerButton = idCurrentButton;
        instancedButtons[idCurrentButton].GetComponent<ButtonBehavior>().Select();
        angle = 0;

        InitialiseScreens();
    }


    // Update is called once per frame
    void Update()
    {
        // Si on prend le smartphone, alors la roue de choix du code apparaît
        if (isHeld)
        {
            // On récupère la position du pouce sur le trackpad de la main droite
            if (SteamVR.connected[0] && !SteamVR.initializing && !SteamVR.calibrating && !SteamVR.outOfRange)
            {
                menuPosition = menuScroll.GetAxis(SteamVR_Input_Sources.RightHand);

                // Angle on trackpad
                float temp = Mathf.Atan2(menuPosition[1], menuPosition[0]);

                // On s'assure qu'il y a bien un angle d'entré (pas toujours de doigt sur le touchpad)
                if (!float.IsNaN(temp) && !float.IsInfinity(temp))
                {
                    // On change le bouton en fonction de l'angle
                    angle = temp + Mathf.PI;
                    SwitchButton(angle);
                }

                // On ajoute un nombre avec le bouton de la main gauche
                if (selectNumber.GetStateUp(SteamVR_Input_Sources.LeftHand))
                {
                    AddNumber(idCurrentButton);
                }
            } else
            {
                angle += Input.GetAxis("Mouse ScrollWheel") * 2;
                angle %= 2 * Mathf.PI;
                SwitchButton(angle);

                // Contrôle classique : espace, touche retour, et chiffres
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    AddNumber(idCurrentButton);
                }

                if (Input.GetKeyUp(KeyCode.Return))
                {
                    AddNumber(idCurrentButton);
                }
                for (int i = 0; i < 9; i++)
                    if (Input.GetKeyUp(KeyCode.Alpha1 + i))
                        AddNumber(i);
                if (Input.GetKeyUp(KeyCode.Backspace))
                    AddNumber(9);
            }
        }

    }

    private void InitialiseScreens()
    {
        screens = new GameObject[2];

        screens[0] = transform.GetChild(0).gameObject;

        screens[1] = GameObject.Find("Screen locked");
        screens[1].transform.SetParent(this.transform);
        screens[1].transform.localPosition = 0.0074f * Vector3.up;
        screens[1].transform.localRotation = Quaternion.identity;
        screens[1].AddComponent(typeof(MeshRenderer));
        screens[1].AddComponent(typeof(MeshFilter));
        List<Material> mats = new List<Material>();
        this.GetComponent<MeshRenderer>().GetSharedMaterials(mats);
        screens[1].GetComponent<MeshRenderer>().sharedMaterial = mats[1];
        float screenWidth = 0.115f;
        float screenHeight = 0.215f;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-screenWidth / 2f, 0, +screenHeight / 2f);
        vertices[1] = new Vector3(+screenWidth / 2f, 0, +screenHeight / 2f);
        vertices[2] = new Vector3(+screenWidth / 2f, 0, -screenHeight / 2f);
        vertices[3] = new Vector3(-screenWidth / 2f, 0, -screenHeight / 2f);
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(1, 0);
        uvs[1] = new Vector2(0, 0);
        uvs[2] = new Vector2(1, 0.85f);
        uvs[3] = new Vector2(0, 0.85f);
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        screens[1].GetComponent<MeshFilter>().sharedMesh = mesh;

        GameObject canvas = GameObject.Find("Phone timer");
        canvas.transform.SetParent(screens[1].transform);
        canvas.transform.localPosition = 0.001f * Vector3.up;
        canvas.transform.localRotation = Quaternion.Euler(90f, 0, 180f);
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(screenWidth, screenHeight);
        canvas.transform.GetChild(0).localScale = new Vector3(screenWidth / 100f, 0.75f * screenHeight / 100f, 1);
        canvas.transform.GetChild(1).localScale = new Vector3(screenWidth / 100f, screenHeight / 100f, 1);
    }

    public void ShowLockedScreen()
    {
        if (screens[0].activeSelf)
        {
            screens[0].SetActive(false);
            screens[1].SetActive(true);
            Debug.Log("lock");
        }
    }

    public void ShowCodeScreen()
    {
        if (screens[1].activeSelf)
        {
            screens[1].SetActive(false);
            screens[0].SetActive(true);
            screenText.text = "Code :";
            screenText.color = Color.white;
        }
    }

    /// <summary>
    /// Sélectionne le bouton suivant
    /// </summary>
    public void SwitchButtonDebug()
    {
        idCurrentButton = (idFormerButton + 1) % numButtons;
        
        // Enfin, selon l'id du bouton, on fait l'échange de sélection entre les boutons
        instancedButtons[idCurrentButton].GetComponent<ButtonBehavior>().Select();
        instancedButtons[idFormerButton].GetComponent<ButtonBehavior>().Deselect();

        idFormerButton = idCurrentButton;
    }

    /// <summary>
    /// Change le bouton sélectionné en fonction de l'angle 
    /// </summary>
    /// <param name="angle">L'angle du bouton sur le menu radial.</param>
    public void SwitchButton(float angle)
    {
        // Il y a 12 boutons => répartition en 12 zones
        // De manière empirique : on a regardé l'angle calculé sur l'inspecteur en runtime pour savoir quel id correspond à quel angle
        idCurrentButton = Mathf.RoundToInt(angle / 2 / Mathf.PI * numButtons);
        // We use entire division to come back in ]-numButtons;numButtons[ interval 
        idCurrentButton -= idCurrentButton / numButtons * numButtons;
        // Then we clamp to [0; numButtons[
        if (idCurrentButton < 0)
            idCurrentButton += numButtons;
        // Enfin, selon l'id du bouton, on fait l'échange de sélection entre les boutons
        if (idFormerButton != idCurrentButton)
        {
            instancedButtons[idCurrentButton].GetComponent<ButtonBehavior>().Select();
            instancedButtons[idFormerButton].GetComponent<ButtonBehavior>().Deselect();

            idFormerButton = idCurrentButton;
        }
    }

    /// <summary>
    /// Permet d'ajouter le chiffre validé (via son id) au code d'essai
    /// </summary>
    /// <param name="id"></param>
    public void AddNumber(int id) 
    {
        id = (id + 1) % numButtons;
        //Si on appuie sur la gâchette de la main gauche, alors on valide l'entrée d'un chiffre du code
        // Les id 10, 11 et 0 correspondent aux boutons Corriger, Valider et Annuler (respectivement)
        switch (id)
        {
            // Delete last character
            case 10:
                tryCode.text = tryCode.text.Substring(0, tryCode.text.Length - 1);
                break;
            case 11:
                tryCode.text = "";
                break;
            case 0:
                isLocked = TryUnlock(tryCode.text);
                break;
            default:
                if (tryCode.text.Length < 4)
                {
                    tryCode.text += id;
                    if (tryCode.text.Length == 4)
                        TryUnlock(tryCode.text);
                }
                else
                    TryUnlock(tryCode.text);
                break;
        }
    }

    /// <summary>
    /// Test si le code d'essai est juste, et agit en conséquence
    /// </summary>
    /// <param name="trycode">Code à tester.</param>
    public bool TryUnlock(string trycode)
    {
        if (tryCode.text == code)
        {
            isLocked = false;
            screenText.color = Color.green;
            screenText.text = "Code\nbon";
            message.EndGame();
        }
        else
        {
            isLocked = true;
            tryCode.text = "";
            screenText.color = Color.red;
            screenText.text = "Code \nfaux !";
        }
        return trycode == code;
    }

    /// <summary>
    /// Initialise les boutons du téléphone
    /// </summary>
    public void InstantiateButtonsWheel()
    {
        for (int i = 0; i < numButtons; i++)
        {
            instancedButtons[i] = Instantiate(prefabButtons[i], transform);
            instancedButtons[i].transform.localPosition = rayonBoutons * new Vector3(Mathf.Cos(Mathf.Deg2Rad * i * espacementBoutons), 0, Mathf.Sin(Mathf.Deg2Rad * i * espacementBoutons));
            instancedButtons[i].transform.localScale /= 3;
        }
    }

    public void SetWheelActive(bool state)
    {
        for (int i = 0; i < numButtons; i++)
            instancedButtons[i].SetActive(state);
    }

    /// <summary>
    /// Make the buttons appear
    /// </summary>
    public void EnableButtonsWheel()
    {
        SetWheelActive(true);
        isHeld = true;
    }

    public void DisableButtonsWheel()
    {
        SetWheelActive(false);
        isHeld = false;
    }
}
