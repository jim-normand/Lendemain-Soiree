using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextDisplay : MonoBehaviour
{
    Text[] textObjects;

    bool firstText = false;
    public EventTrigger.TriggerEvent onGameStart = null;

    [Tooltip("Time (in seconds) during which the text is displayed.")]
    public float textDisplayDuration;
    private float startTime;

    private Color initialColor;
    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        textObjects = gameObject.GetComponentsInChildren<Text>();
        initialColor = textObjects[0].color;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        transform.SetParent(mainCamera.transform);
        transform.localPosition = mainCamera.GetComponent<Camera>().nearClipPlane * 1.1f * Vector3.forward;
        transform.localRotation = Quaternion.identity;
        // Better use camera characteristics
        transform.localScale = .0001f * Vector3.one;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstText)
        {
            if(Time.time < startTime + textDisplayDuration)
            {
                textObjects[0].color = Color.Lerp(initialColor, Color.clear, (Time.time - startTime - textDisplayDuration) / textDisplayDuration);
            } 
            else
            {
                textObjects[0].text = "";
                textObjects[0].color = initialColor;
                firstText = false;
                if (onGameStart != null)
                    onGameStart.Invoke(null);
            }
        }
    }

    /// <summary>
    /// Affiche le texte de début de partie
    /// </summary>
    private void StartGame()
    {
        textObjects[0].text = "Vous vous réveillez dans une chambre inconnue...";
        textObjects[0].text += "\nTrouvez le code du portable de votre hôte pour effacer les photos compromettantes et enfuyez vous !";
        firstText = true;
        startTime = Time.time;
    }

    /// <summary>
    /// Affiche le texte de fin de partie
    /// </summary>
    public void EndGame()
    {
        textObjects[0].fontSize = 40;
        textObjects[0].color = Color.green;
        textObjects[0].text = "Vous avez effacé toutes les photos comprometantes !";
    }

    public void OnGameOver(BaseEventData arg)
    {
        textObjects[0].text = "Vous avez perdu !";
        textObjects[0].fontSize = 50;
        textObjects[0].color = Color.red;
    }
}
