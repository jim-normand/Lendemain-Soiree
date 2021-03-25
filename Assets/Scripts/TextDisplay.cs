using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextDisplay : MonoBehaviour
{
    Text[] textObjects;

    bool firstText = false;
    int frameCounter = 0;
    public EventTrigger.TriggerEvent onGameStart = null;

    // Start is called before the first frame update
    void Start()
    {
        textObjects = gameObject.GetComponentsInChildren<Text>();
        
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = camera.transform.position + 4.0f * camera.transform.forward;

        if(firstText)
        {
            frameCounter++;
            if(frameCounter > 600)
            {
                textObjects[0].color = new Color(textObjects[0].color.r, textObjects[0].color.g, textObjects[0].color.b, textObjects[0].color.a - 0.003f);
                if(textObjects[0].color.a <= 0)
                {
                    frameCounter = 0;
                    textObjects[0].text = "";
                    textObjects[0].color = new Color(textObjects[0].color.r, textObjects[0].color.g, textObjects[0].color.b, 1.0f);
                    firstText = false;
                    if (onGameStart != null)
                        onGameStart.Invoke(null);
                }
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
    }

    /// <summary>
    /// Affiche le texte de fin de partie
    /// </summary>
    public void EndGame()
    {
        textObjects[0].fontSize = 40;
        textObjects[0].color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        textObjects[0].text = "Vous avez effacé toutes les photos comprometantes !";
    }

    public void OnGameOver(BaseEventData arg)
    {
        textObjects[0].text = "Vous avez perdu !";
        textObjects[0].fontSize = 50;
        textObjects[0].color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
}
