using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class Seagull : MonoBehaviour
{
    public enum SeagullState
    {
        Rest,
        Fly,
        Fall
    }

    public SeagullState currentState = SeagullState.Fly;
    [SerializeField] private float maxStamina = 1000f;
    [SerializeField] private float stamina = 1000f;
    [SerializeField] private float flyDuration = 10f;
    [SerializeField] private float restDuration = 5f;
    [SerializeField] private int dir = 1;
    [SerializeField] private float speed = 0.03f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drownClip;
    [SerializeField] private AudioClip sgClip;
    private Vector3 originalScale;


    private void Start()
    {
        currentState = SeagullState.Fly;
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        updateSeagullState(currentState);
    }
    private void Update()
    {
        if (currentState == SeagullState.Fly)
        {
            transform.position += new Vector3(dir * speed, 0f, 0f);
            stamina--;
            if(stamina <= 0f)
            {
                updateSeagullState(SeagullState.Rest);
            }
        }
    }
   
    void updateSeagullState(SeagullState state)
    {
        switch (state)
        {
            case SeagullState.Rest:
                handleRest();
                break;
            case SeagullState.Fly:
                handleFly();
                break;
            case SeagullState.Fall:
                handleFall();
                break;
            default:
                break;
        }
    }

    private void handleFly()
    {
        currentState = SeagullState.Fly;
        // Decrease stamina every second
        

        

    }
   


    private void handleRest()
    {
        currentState = SeagullState.Rest;

        // Land the Seagull
        transform.DOMoveY(-1f, 1f).OnComplete(() => StartCoroutine(StartFlyingAfterRest()));
    }

    private IEnumerator StartFlyingAfterRest()
    {
        stamina = maxStamina;
        yield return new WaitForSeconds(restDuration);
        float yVal = Random.Range(5, 8);
        float t = Random.Range(0.5f, 1.5f);
        transform.DOMoveY(yVal, t).OnComplete(() => handleFly());
        //handleFly();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger");
        if(collision.tag == "bound")
        {
            dir *= -1;
            var scale = transform.localScale.x * -1;
            transform.DOScaleX(scale, 0.1f);
        }
        if (collision.tag == "crab")
        {

        }
        if (currentState == SeagullState.Fly && collision.tag == "crab")
        {
            updateSeagullState(SeagullState.Fall);
        }
    }

    private void handleFall()
    {
        currentState = SeagullState.Fall;

        // Lower Y position rapidly
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = drownClip;
            audioSource.Play();
        }
        transform.DOMoveY(0f, 2f).OnComplete(() => Destroy(gameObject));
    }
}
