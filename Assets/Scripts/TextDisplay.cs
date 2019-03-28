using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    public Camera camera;
    public float numberOfMinutes;

    Text[] textObjects;

    bool firstText = false;
    int frameCounter = 0;

    float initialTime;
    float timeLeft;

    bool timerRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        textObjects = gameObject.GetComponentsInChildren<Text>();
        
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.transform.position + 4.0f * camera.transform.forward;

        if(firstText)
        {
            frameCounter++;
            if(frameCounter > 900)
            {
                textObjects[0].color = new Color(textObjects[0].color.r, textObjects[0].color.g, textObjects[0].color.b, textObjects[0].color.a - 0.003f);
                if(textObjects[0].color.a <= 0)
                {
                    frameCounter = 0;
                    textObjects[0].text = "";
                    textObjects[0].color = new Color(textObjects[0].color.r, textObjects[0].color.g, textObjects[0].color.b, 1.0f);
                    firstText = false;
                    initialTime = Time.realtimeSinceStartup;
                }
            }
        } else if(timerRunning)
        {
            DisplayTime();
        }
    }

    private void StartGame()
    {
        textObjects[0].text = "Vous vous réveillez dans une chambre inconnue...";
        textObjects[0].text += "Trouvez le code du portable de votre hôte pour effacer les photos compromettantes et enfuyez vous !";
        firstText = true;
    }

    public void EndGame()
    {
        textObjects[0].fontSize = 40;
        textObjects[0].color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        textObjects[0].text = "Vous avez effacé toutes les photos comprometantes !";
        timerRunning = false;
    }

    private void DisplayTime()
    {
        float timeSinceBeginning = Time.realtimeSinceStartup - initialTime;
        timeLeft = 60 * numberOfMinutes - timeSinceBeginning;
        int minutes = (Mathf.CeilToInt(timeLeft) - Mathf.CeilToInt(timeLeft) % 60) / 60;
        int seconds = Mathf.CeilToInt(timeLeft) % 60;
        if (minutes > 0)
        {
            textObjects[1].text = minutes.ToString() + "min " + seconds.ToString() + "s";
        } else
        {
            textObjects[1].fontSize = 40;
            textObjects[1].color = Color.red;
            textObjects[1].text = seconds.ToString() + "s";
        }
        if(Mathf.RoundToInt(timeLeft) <= 0)
        {
            textObjects[1].text = "";
            textObjects[0].text = "Vous avez perdu !";
            textObjects[0].fontSize = 50;
            textObjects[0].color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
}
