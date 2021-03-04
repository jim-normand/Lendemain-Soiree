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
    public Vector2 menuPosition;
    public bool isLocked = true; 
    [Tooltip("Code entered by user.")]
    public Text tryCode;
    public Text screenText;
    public GameObject canvas;

    private int idCurrentButton;
    private int idFormerButton;
    private float espacementBoutons;
    private float rayonBoutons;
    // Pour ajouter un chiffre au code un par un
    private bool unChiffreEnPlusPasPlus = true; 
    public bool initWheelOK;
    // Le code à trouver pour déverouiller
    private string code; 
    [Tooltip("Array of buttons prefabs")]
    public GameObject[] prefabButtons;
    [Tooltip("Array of buttons instances")]
    private GameObject[] instancedButtons;

    // Start is called before the first frame update
    void Start()
    {
        //Paramètres de la roue de chiffres
        espacementBoutons = 360 / numButtons;
        rayonBoutons = 0.1f;

        //Initialisation de quelques variables oklm
        isHeld = false;
        code = "6476";
        tryCode.text = "";
        initWheelOK = true;
        screenText.text = "Code :";

        //Par défault, c'est le bouton 'en haut' de la roue qui est sélectionné (bouton correction => g10)
        instancedButtons = new GameObject[numButtons];
        // Modifies instancedButtons
        InstantiateButtonsWheel();
        DisableButtonsWheel();
        idCurrentButton = 1;
        idFormerButton = idCurrentButton;
        instancedButtons[idCurrentButton].GetComponent<ButtonBehavior>().Select();
        angle = 0;

        canvas = transform.GetChild(0).gameObject;
        canvas.SetActive(false);
        GetComponent<Throwable>().onPickUp.AddListener(EnableButtonsWheel);
        GetComponent<Throwable>().onDetachFromHand.AddListener(DisableButtonsWheel);
    }


    // Update is called once per frame
    void Update()
    {
        //GetComponent<Throwable>().attached
        //isHeld = GetComponent<Throwable>().GetAttached();

        // Si on prend le smartphone, alors la roue de choix du code apparaît
        if (isHeld)
        {
            if (initWheelOK)
            {
                initWheelOK = false;
            }

            //Activation du canvas 
            canvas.SetActive(true);

            // On récupère la position du pouce sur le trackpad de la main droite
            try
            {
                menuPosition = menuScroll.GetAxis(SteamVR_Input_Sources.RightHand);

                // Angle on trackpad
                float temp = Mathf.Atan2(menuPosition[0], menuPosition[1]);

                // Quick fix whe no HMD connected
                if (!float.IsNaN(temp) && !float.IsInfinity(temp) && false)
                {
                    // On change le bouton en fonction de l'angle
                    angle = temp;
                    SwitchButton(angle);
                }
            } catch
            {
                angle += Input.GetAxis("Mouse ScrollWheel") * 2;
                angle %= 2 * Mathf.PI;
                SwitchButton(angle);
            }

            // En mode debug
#if DEBUG
            if(Input.GetKeyUp(KeyCode.Space))
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
#endif

            //On ajoute un nombre avec le bouton de la main gauche
            //device = Valve.VR.SteamVR_Controller.Input(Valve.VR.OpenVR.k_unTrackedDeviceIndex_Hmd);
            // true si casque ready
            //Debug.Log(SteamVR.connected[0] && !SteamVR.initializing && !SteamVR.calibrating && !SteamVR.outOfRange);
            try
            {
                if (selectNumber.GetState(SteamVR_Input_Sources.LeftHand))
                {
                    AddNumber(idCurrentButton);
                }

                //Quand on relâche la gâchette on peut rajouter un chiffre
                // selectNumber.GetStateUp(SteamVR_Input_Sources.LeftHand) permettrait d'effacer UnChiffreEnPlusPasPlus
                if (!selectNumber.GetState(SteamVR_Input_Sources.LeftHand))
                {
                    unChiffreEnPlusPasPlus = true;
                }
            }
            catch { 
            }

        }
        else
        {
            initWheelOK = true;
            canvas.SetActive(false);
        }

    }
#if DEBUG
    /// <summary>
    /// Change la sélection du bouton [debug]
    /// </summary>
    public void SwitchButtonDebug()
    {
        idCurrentButton = (idFormerButton + 1) % numButtons;
        
        // Enfin, selon l'id du bouton, on fait l'échange de sélection entre les boutons
        instancedButtons[idCurrentButton].GetComponent<ButtonBehavior>().Select();
        instancedButtons[idFormerButton].GetComponent<ButtonBehavior>().Deselect();

        idFormerButton = idCurrentButton;
    }
#endif
    /// <summary>
    /// Change le bouton sélectionné en fonction de l'angle 
    /// </summary>
    /// <param name="angle"></param>
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
        if (tryCode.text.Length < 4 && unChiffreEnPlusPasPlus)
        {
            unChiffreEnPlusPasPlus = false;
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
                    tryCode.text += id;
                    break;
            }
        }

        if (tryCode.text.Length == 4) 
        {
            // Why is this block ???
            switch (id)
            {
                case 10:
                    tryCode.text = tryCode.text.Substring(0, tryCode.text.Length - 1);
                    unChiffreEnPlusPasPlus = false;
                    break;
                case 11:
                    tryCode.text = "";
                    unChiffreEnPlusPasPlus = false;
                    break;
                case 0:
                    isLocked = TryUnlock(tryCode.text);
                    unChiffreEnPlusPasPlus = false;
                    break;
            }

            if (TryUnlock(tryCode.text))
            {
                isLocked = false;
                screenText.color = Color.green;
                screenText.text = "True";
                TextDisplay message = new TextDisplay();
                message.EndGame();
            }
            else
            {
                tryCode.text = "";
                screenText.color = Color.red;
                screenText.text = "Wrong \ncode";
            }
            
        }
    }

    /// <summary>
    /// Test si le code d'essai est juste 
    /// </summary>
    /// <param name="trycode">Code à tester.</param>
    public bool TryUnlock(string trycode)
    {
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
