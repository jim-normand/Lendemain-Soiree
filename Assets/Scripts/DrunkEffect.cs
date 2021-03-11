using UnityEngine;
using Valve.VR.InteractionSystem;

// Create a "drunk effect" on hands, they are sometimes less precise
public class DrunkEffect : MonoBehaviour
{
    [Tooltip("Ratio of time you no not have controls.")]
    [Range(0, 100)]
    public float drunkRatio;
    [Tooltip("The amount of time you loose control over hand (in seconds).")]
    public float drunkDuration;
    [Tooltip("Minimum time between two drunks periods")]
    public float drunkEvaluationPeriod;

    [Tooltip("Last time each hand was drunk.")]
    private float[] drunkHandTimes;

    [Tooltip("List of possible Hands, including no-SteamVR fallback Hands.")]
    private Hand[] hands;

    [Tooltip("Is this hand drunk?")]
    private bool drunkHand;


    void Start()
    {
        hands = GetComponent<Player>().hands;
        drunkHandTimes = new float[3] { 
            Time.time - drunkEvaluationPeriod, 
            Time.time - drunkEvaluationPeriod, 
            Time.time - drunkEvaluationPeriod
        };
        //InvokeRepeating("DisableHands", 0, 1);
    }

    private void Update()
    {
        // For each hand (left, right and fallback)
        for (int i = 0; i < 3; i++)
        {
            // Are we still drunk ?
            drunkHand = Time.time < drunkHandTimes[i] + drunkDuration;
            // Can we get drunk again (regarding drunk period)
            if (!drunkHand && Time.time > drunkHandTimes[i] + drunkEvaluationPeriod)
            {
                drunkHand = Random.Range(0, 100) < drunkRatio;
                if (drunkHand)
                {
                    drunkHandTimes[i] = Time.time;
                    // Sometimes we drop held objects
                    if (hands[i].currentAttachedObject && Random.Range(0, 100) < drunkRatio)
                        hands[i].DetachObject(hands[i].currentAttachedObject);
                }
            }
            hands[i].enabled = !drunkHand;
        }
    }

}
