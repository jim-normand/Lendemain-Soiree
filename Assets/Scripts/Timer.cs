using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Timer : MonoBehaviour
{
    [Tooltip("Number of minutes before end game.")]
    public float numberOfMinutes;
    private float initialTime;
    private float timeLeft;
    private bool over;

    private Text timeText;

    public EventTrigger.TriggerEvent onGameOver;

    // Start is called before the first frame update
    void Start()
    {
        over = false;
        numberOfMinutes = Mathf.Floor(numberOfMinutes);
        timeText = GetComponent<Text>();
        initialTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayTime();
    }

    /// <summary>
    /// Affiche le chronomètre du temps restant pour l'escape game
    /// </summary>
    private void DisplayTime()
    {
        if (over)
            return;

        float timeSinceBeginning = Time.realtimeSinceStartup - initialTime;
        timeLeft = 60 * numberOfMinutes - timeSinceBeginning;
        int minutes = (Mathf.CeilToInt(timeLeft) - Mathf.CeilToInt(timeLeft) % 60) / 60;
        int seconds = Mathf.CeilToInt(timeLeft) % 60;
        if (minutes > 0)
        {
            if (seconds >= 10)
            {
                timeText.text = minutes.ToString() + " : " + seconds.ToString();
            }
            else
            {
                timeText.text = minutes.ToString() + " : 0" + seconds.ToString();
            }
        }
        else
        {
            timeText.fontSize = 40;
            timeText.color = Color.red;
            if (seconds >= 10)
            {
                timeText.text = seconds.ToString();
            }
            else
            {
                timeText.text = "0" + seconds.ToString();
            }
        }
        if (timeLeft <= 0)
        {
            timeText.text = "00:00";
            // ?
            onGameOver.Invoke(null);
            over = true;

        }
    }
}
