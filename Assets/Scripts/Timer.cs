using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Timer : MonoBehaviour
{
    public float numberOfMinutes;
    private float initialTime;
    private float timeLeft;
    private bool over;

    public EventTrigger.TriggerEvent onGameOver;
    // Start is called before the first frame update
    void Start()
    {
        over = false;
        numberOfMinutes = Mathf.Floor(numberOfMinutes);
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
                this.GetComponent<Text>().text = minutes.ToString() + " : " + seconds.ToString();
            }
            else
            {
                this.GetComponent<Text>().text = minutes.ToString() + " : 0" + seconds.ToString();
            }
        }
        else
        {
            this.GetComponent<Text>().fontSize = 40;
            this.GetComponent<Text>().color = Color.red;
            if (seconds >= 10)
            {
                this.GetComponent<Text>().text = seconds.ToString();
            }
            else
            {
                this.GetComponent<Text>().text = "0" + seconds.ToString();
            }
        }
        if (Mathf.RoundToInt(timeLeft) <= 0)
        {
            this.GetComponent<Text>().text = "00:00";
            onGameOver.Invoke(null);
            over = true;

        }
    }
}
