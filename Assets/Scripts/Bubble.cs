/*using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bubble : MonoBehaviour
{
    public float blowDuration = 1.0f;
    public float jiggleDuration = 2.0f;
    public float flyDuration = 2.0f;
    public float riseDuration = 3.0f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnEnable()
    {

        anim(() =>
        {
           // pop();
        });
    }

    void pop()
    {
        audioSource.Play();
        this.transform.DOScale(0.5f, 0.5f).OnComplete(() =>
        {
            DOTween.Kill(this.gameObject);
            DOTween.Kill(this.transform);
            Lifeguard.Instance.totalBubble--;
            Destroy(this.gameObject);
            
            
        });
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "crab") pop();
    }
    void anim(System.Action onCompleteCallback = null)
    {
        // Create a sequence of animations
        Sequence bubbleSequence = DOTween.Sequence();
        originalScale = transform.localScale;
        originalPosition = transform.position;

        float endX = Random.Range(9f, -15f);
        float riseY = Random.Range(5f, 7f);
        

        // Step 1: Blow out
        bubbleSequence.Append(transform.DOScale(originalScale * 1.5f, blowDuration));

        // Step 2: Jiggle up and down
        var jiggle = transform.DOShakePosition(jiggleDuration, strength: 0.1f, vibrato: 20, fadeOut: false);

        // Step 3: Fly to a horizontal position
        float targetX = endX; // Adjust this value to your desired horizontal position
        var fly = transform.DOMoveX(targetX, flyDuration);

        bubbleSequence.Join(jiggle);
        bubbleSequence.Join(fly);

        // Step 4: Slowly rise up
        float targetY = originalPosition.y + riseY; // Adjust this value to your desired vertical position
        bubbleSequence.Append(transform.DOMoveY(targetY, riseDuration));

        // Start the bubble animation sequence
        bubbleSequence.Play();

        bubbleSequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
            pop();
            bubbleSequence.Kill();
        });
    }


}
*/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bubble : MonoBehaviour
{
    public enum BubbleState
    {
        Blowout,
        Fly,
        Rise
    }

    private BubbleState currentState = BubbleState.Blowout;

    public float blowDuration = 1.0f;
    public float flyDuration = 2.0f;
    public float riseDuration = 3.0f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent < AudioSource>();
    }

    private void OnEnable()
    {
        UpdateBubbleState();
    }

    private void pop()
    {
        audioSource.Play();
        this.transform.DOScale(0.5f, 0.5f).OnComplete(() =>
        {
            DOTween.Kill(this.gameObject);
            DOTween.Kill(this.transform);
            Lifeguard.Instance.totalBubble--;
            Destroy(this.gameObject);
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "crab")
        {
            pop();
        }
    }

    private void UpdateBubbleState()
    {
        switch (currentState)
        {
            case BubbleState.Blowout:
                StartBlowout();
                break;
            case BubbleState.Fly:
                StartFly();
                break;
            case BubbleState.Rise:
                StartRise();
                break;
        }
    }

    private void StartBlowout()
    {
        currentState = BubbleState.Blowout;
        originalScale = transform.localScale;
        originalPosition = transform.position;

        // Create a sequence of animations for blowout
        Sequence bubbleSequence = DOTween.Sequence();

        // Step 1: Blow out
        bubbleSequence.Append(transform.DOScale(originalScale * 1.5f, blowDuration));

        bubbleSequence.OnComplete(() =>
        {
            currentState = BubbleState.Fly;
            UpdateBubbleState();
        });

        // Start the bubble animation sequence
        bubbleSequence.Play();
    }

    private void StartFly()
    {
        currentState = BubbleState.Fly;

        // Step 2: Jiggle up and down and then Fly to a horizontal position
        float endX = Random.Range(9f, -15f);
        float riseY = Random.Range(5f, 7f);

        // Create a sequence of animations for fly
        Sequence bubbleSequence = DOTween.Sequence();
        bubbleSequence.Append(transform.DOShakePosition(flyDuration, strength: 0.1f, vibrato: 20, fadeOut: false))
            .Join(transform.DOMoveX(endX, flyDuration))
            .Join(transform.DOMoveY(originalPosition.y + riseY, riseDuration));

        bubbleSequence.OnComplete(() =>
        {
            pop();
        });

        // Start the bubble animation sequence
        bubbleSequence.Play();
    }

    private void StartRise()
    {
        currentState = BubbleState.Rise;

        // Step 3: Slowly rise up
        float riseY = Random.Range(5f, 7f);
        float targetY = originalPosition.y + riseY; // Adjust this value to your desired vertical position

        // Create a sequence of animations for rising
        Sequence bubbleSequence = DOTween.Sequence();
        bubbleSequence.Append(transform.DOMoveY(targetY, riseDuration));

        bubbleSequence.OnComplete(() =>
        {
            pop();
        });

        // Start the bubble animation sequence
        bubbleSequence.Play();
    }
}
