using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Lifeguard : MonoBehaviour
{
    public enum lifeguardState
    {
        whistle,
        blow,
        idle
    }
    public static Lifeguard Instance;
    public lifeguardState curState;
    public GameObject bubble;
    [SerializeField] private int bubbleCount;
    [SerializeField] private int blowCount;
    [SerializeField] private float bubbleCap;
    [SerializeField] private float bubbleReload;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blowClip;
    [SerializeField] private AudioClip whistleClip;
    public int totalBubble = 0;

    private void Awake()
    {
        Instance = this;
        Crab.OnBorn += Crab_OnBorn;
    }

    private void Crab_OnBorn(Lifeguard obj)
    {
        updateLifeguardState(lifeguardState.whistle);
        updateLifeguardState(lifeguardState.idle);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bubbleCap = 1500;
        bubbleReload = 0;
        curState = lifeguardState.idle;
    }
    private void Update()
    {
        if (curState == lifeguardState.idle)
        {
            handleIdle();
            bubbleReload += 0.6f;
        }
        
        
    }
   
    void updateLifeguardState(lifeguardState state)
    {
        //Debug.Log(curState + " going into " + state);
        if (state != curState)
        {
            curState = state;
            switch (state)
            {
                case lifeguardState.whistle:
                    handleWhistle();
                    break;
                case lifeguardState.blow:
                    handleBlow();
                    break;
                case lifeguardState.idle:
                    handleIdle();
                    break;
                default:
                    break;
            }
        }

    }

    void handleWhistle()
    {
        curState = lifeguardState.whistle;
        if (!audioSource.isPlaying)
        {
            audioSource.clip = whistleClip;
            //audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            if (bubbleReload >= bubbleCap)
            {
                audioSource.Stop();
                Debug.Log("stopped");
                updateLifeguardState(lifeguardState.blow);
            }
            updateLifeguardState(lifeguardState.idle);
        }
    }

    void handleBlow()
    {
        curState = lifeguardState.blow;
        if (totalBubble < 20)
        {
            StartCoroutine(blowBubbles(blowCount));
            updateLifeguardState(lifeguardState.idle);
        }
        else
        {
            bubbleReload = 0;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            updateLifeguardState(lifeguardState.idle);
        }
    }

    void handleIdle()
    {
        //if (audioSource.isPlaying) audioSource.Stop();
        blowCount = UnityEngine.Random.Range(1, 4);
        //Debug.Log("im blowing " +  blowCount);
        if (bubbleReload >= bubbleCap)
        {
            Debug.Log("???");
            updateLifeguardState(lifeguardState.blow);
        }

    }

    private IEnumerator blowBubbles(int numBubbles)
    {
        Debug.Log(numBubbles);
        for (int i = 0; i < numBubbles; i++)
        {
            audioSource.clip = blowClip;
            audioSource.Play();
            Instantiate(bubble, transform.position, Quaternion.identity);
            totalBubble++;
            yield return new WaitForSeconds(3.0f);
        }
    }

    private IEnumerator DelayNextRound(float delay)
    {
        // Wait for the specified delay before starting the next round
        yield return new WaitForSeconds(delay);
    }
}

