using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float blowDuration = 1.0f;
    public float jiggleDuration = 2.0f;
    public float flyDuration = 2.0f;
    public float riseDuration = 3.0f;

    private Vector3 originalScale;
    private Vector3 originalPosition;


    

    void OnEnable()
    {

        anim(() =>
        {
           // pop();
        });
    }

    void pop()
    {
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
