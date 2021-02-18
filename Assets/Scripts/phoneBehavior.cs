using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;


public class phoneBehavior : MonoBehaviour
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

    private buttonBehavior selectedButton;
    private buttonBehavior formerButton;
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
    private GameObject[] instancedButtons;
    //private List<GameObject> gObjList;
    //private List<buttonBehavior> boutons;
    //private Transform originalParent;

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
        InitiateButtonWheel();
        for (int i = 0; i < numButtons; i++)
            instancedButtons[i].SetActive(false);
        idCurrentButton = 9;
        idFormerButton = idCurrentButton;
        selectedButton = instancedButtons[idCurrentButton].GetComponent<buttonBehavior>();
        selectedButton.Select();

        canvas = transform.GetChild(0).gameObject;
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isHeld = GetComponent<Throwable>().GetAttached();

        // Si on prend le smartphone, alors la roue de choix du code apparaît
        if (isHeld)
        {
            if (initWheelOK)
            {
                //initiateButtonWheel();
                for (int i = 0; i < numButtons; i++)
                    instancedButtons[i].SetActive(true);
                initWheelOK = false;
            }

            //Activation du canvas 
            canvas.SetActive(true);

            // On récupère la position du pouce sur le trackpad de la main droite
            menuPosition = menuScroll.GetAxis(SteamVR_Input_Sources.RightHand);

            // On calcule l'angle
            angle = Mathf.Atan(menuPosition[0] / menuPosition[1]);
            // Mathf.Atan2(menuPosition[0], menuPosition[1]) plus sûr

            //On change le bouton en fonction de l'angle
            SwitchButton(angle);

            // En mode debug
#if DEBUG
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SwitchButtonDebug();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                AddNumber(idCurrentButton);
            }
#endif

            //On ajoute un nombre avec le bouton de la main gauche
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
        else
        {
            //DestroyButtonWheel();
            for (int i = 0; i < numButtons; i++)
                instancedButtons[i].SetActive(false);
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
        if (idFormerButton != idCurrentButton)
        {
            selectedButton = instancedButtons[idCurrentButton].GetComponent<buttonBehavior>();
            formerButton = instancedButtons[idFormerButton].GetComponent<buttonBehavior>();

            selectedButton.Select();
            formerButton.Deselect();

            idFormerButton = idCurrentButton;
        }
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
        //Debug.Log(angle * Mathf.Rad2Deg);
        if (menuPosition[0] >= 0)
        {
            if (0.0f < angle && angle <= Mathf.PI / 12)
            {
                idCurrentButton = 10;
            }

            if (Mathf.PI / 12 < angle && angle <= 3 * Mathf.PI / 12)
            {
                idCurrentButton = 9;
            }

            if (3*Mathf.PI / 12 < angle && angle <= 5 *Mathf.PI / 12)
            {
                idCurrentButton = 8;
            }

            if ((5 * Mathf.PI / 12 < angle && angle <= Mathf.PI / 2) || (-Mathf.PI / 2 <= angle && angle <= -5 * Mathf.PI / 12))
            {
                idCurrentButton = 7;
            }

            if (-5 * Mathf.PI / 12 < angle && angle <= -3 * Mathf.PI / 12)
            {
                idCurrentButton = 6;
            }

            if (-3 * Mathf.PI / 12 < angle && angle <= -Mathf.PI / 12)
            {
                idCurrentButton = 5;
            }

            if (-Mathf.PI / 12 < angle && angle <= 0.0)
            {
                idCurrentButton = 4;
            }
        }

        if (menuPosition[0] < 0)
        {
            if (0.0f < angle && angle <= Mathf.PI / 12)
            {
                idCurrentButton = 4;
            }

            if (Mathf.PI / 12 < angle && angle <= 3 * Mathf.PI / 12)
            {
                idCurrentButton = 3;
            }

            if (3 * Mathf.PI / 12 < angle && angle <= 5 * Mathf.PI / 12)
            {
                idCurrentButton = 2;
            }

            if ((5 * Mathf.PI / 12 < angle && angle <= Mathf.PI / 2) || (-Mathf.PI / 2 <= angle && angle <= -5 * Mathf.PI / 12))
            {
                idCurrentButton = 1;
            }

            if (-5 * Mathf.PI / 12 < angle && angle <= -3 * Mathf.PI / 12)
            {
                idCurrentButton = 12;
            }

            if (-3 * Mathf.PI / 12 < angle && angle <= -Mathf.PI / 12)
            {
                idCurrentButton = 11;
            }

            if (-Mathf.PI / 12 < angle && angle <= 0.0)
            {
                idCurrentButton = 10;
            }
        }
        // Quick fix : if no HMD, menuPosition[0] is NaN
        if (menuPosition[0] < 0 || menuPosition[0] > 0)
            idCurrentButton -= 1;

        // Enfin, selon l'id du bouton, on fait l'échange de sélection entre les boutons
        if (idFormerButton != idCurrentButton)
        {
            selectedButton = instancedButtons[idCurrentButton].GetComponent<buttonBehavior>();
            formerButton = instancedButtons[idFormerButton].GetComponent<buttonBehavior>();

            selectedButton.Select();
            formerButton.Deselect();

            idFormerButton = idCurrentButton;
        }
    }

    /// <summary>
    /// Permet d'ajouter le chiffre validé (via son id) au code d'essai
    /// </summary>
    /// <param name="id"></param>
    public void AddNumber(int id) 
    {

        //Si on appuie sur la gâchette de la main gauche, alors on valide l'entrée d'un chiffre du code
        // Les id 10, 11 et 12 correspondent aux boutons Corriger, Valider et Annuler (respectivement)
        if (tryCode.text.Length < 4 && unChiffreEnPlusPasPlus)
        {
            unChiffreEnPlusPasPlus = false;
            switch (idCurrentButton)
            {
                case 10:
                    isLocked = TryUnlock(tryCode.text);
                    break;

                case 11:
                    tryCode.text = "";
                    break;
                // Delete last character
                case 9:
                    tryCode.text = tryCode.text.Substring(0, tryCode.text.Length - 1);
                    break;
                default:
                    tryCode.text += id;
                    break;
            }
        }

        if (tryCode.text.Length == 4) 
        {
            // Why is this block ???
            switch (idCurrentButton)
            {
                case 10:
                    isLocked = TryUnlock(tryCode.text);
                    unChiffreEnPlusPasPlus = false;
                    break;
                case 11:
                    tryCode.text = "";
                    unChiffreEnPlusPasPlus = false;
                    break;
                case 9:
                    tryCode.text = tryCode.text.Substring(0, tryCode.text.Length - 1);
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
    public void InitiateButtonWheel()
    {
        for (int i = 0; i < numButtons; i++)
        {
            instancedButtons[i] = Instantiate(prefabButtons[i], transform);
            instancedButtons[i].transform.localPosition = rayonBoutons * new Vector3(Mathf.Cos(Mathf.Deg2Rad * i * espacementBoutons), 0, Mathf.Sin(Mathf.Deg2Rad * i * espacementBoutons));
            instancedButtons[i].transform.localScale /= 3;
        }
    }
}
