using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Crab : MonoBehaviour
{
    public GameObject crab;
    public static List<Crab> crabList = new List<Crab>();
    public List<GameObject> children = new List<GameObject>();

    [SerializeField] private float wanderSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float stamina;
    [SerializeField] private float mateStaminaReq;
    [SerializeField] private float mateDesireLvl;
    [SerializeField] private float mateDesireReq;
    [SerializeField] private float bubbleDetectionRad;
    [SerializeField] private float jmpForce;
    [SerializeField] private float lifeTime;
    public static event Action<Lifeguard> OnBorn;
    public crabState curCrabState;
    public Rigidbody2D rb;
    public int dir;

    #region matepulse
    //Sequence pulseSequence = DOTween.Sequence();
    public float pulseDuration = 1.0f;
    public int numberOfPulses = 5;
    public float totalTime = 5.0f;
    private Vector3 originalScale;
    #endregion

    public enum crabState
    {
        wander,
        sleep,
        mate,
        dead
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bound");
        if (collision.tag == "bound") dir *= -1;
    }

    void Awake()
    {
        
        int customCapacity = 2000; 
        DOTween.SetTweensCapacity(customCapacity, 50);

        setInitValue();


        updateCrabState(curCrabState);

    }
    private void Start()
    {
        /*dir = Random.Range(-1, 1) < 0 ? -1 : 1;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        crabList.Add(this);
        mateDesireLvl = 0;
        originalScale = transform.localScale;
        updateCrabState(curCrabState);*/
    }
    private void Update()
    {
        if (lifeTime <= 0) updateCrabState(crabState.dead);
        else
        {
            updateCrabState(curCrabState);
            if (curCrabState != crabState.mate && curCrabState != crabState.sleep) mateDesireLvl += 0.1f;
            lifeTime -= 0.1f;
        }
    }

    void updateCrabState(crabState newCrabState)
    {
        switch (newCrabState)
        {
            case crabState.wander:
                handleWander();
                break;
            case crabState.sleep:
                handleSleep();
                break;
            case crabState.dead:
                handleDeath();
                break;
            case crabState.mate:
                handleMate();
                break;
            default:
                break;
        }
    }

    void setInitValue()
    {
        wanderSpeed = 0.015f;
        maxStamina = 600;
        stamina = 600;
        mateStaminaReq = 150;
        mateDesireLvl = 0;
        mateDesireReq = 600;
        bubbleDetectionRad = 10;
        jmpForce = 0.2f;
        lifeTime = 5000;
        curCrabState = crabState.wander;
        rb = GetComponent<Rigidbody2D>();
        dir = UnityEngine.Random.Range(-1, 1) < 0 ? -1 : 1;
        originalScale = transform.localScale;
    }
    void handleWander()
    {
        curCrabState = crabState.wander;
        Debug.Log("wandering");
        if (stamina > 0)
        {
            this.transform.position += new Vector3(dir * wanderSpeed, 0, 0);
            stamina -= 0.4f;
            // check for mate
            if (mateDesireLvl >= mateDesireReq && children.Count < 2 && stamina >= mateStaminaReq)
            {
                updateCrabState(crabState.mate);
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, bubbleDetectionRad);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "playable")
                    {
                        var jmpDir = (collider.transform.position - transform.position).normalized;
                        rb.AddForce(jmpDir * jmpForce, ForceMode2D.Impulse);
                        stamina -= 5;
                        break;
                    }
                }
            }
        }
        else
        {
            updateCrabState(crabState.sleep);
        }
    }

    void handleSleep()
    {
        curCrabState = crabState.sleep;
        Debug.Log("sleeping");
        if (stamina < maxStamina)
        {
            stamina += 0.5f;
        }
        else updateCrabState(crabState.wander);
    }

    void handleMate()
    {
        if (curCrabState != crabState.mate)
        {
            Debug.Log("mating");
            curCrabState = crabState.mate;


            
            StartPulsing(() =>
            {
                var child = Instantiate(crab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                children.Add(child);
                OnBorn?.Invoke(Lifeguard.Instance);
                stamina -= mateStaminaReq;
                mateDesireLvl = 0;
                updateCrabState(crabState.wander);
            });

        }
    }

    void handleDeath()
    {
        this.transform.DOScaleY(-originalScale.y, 1f).OnComplete(() =>
        {
            this.transform.DOPunchRotation(new Vector3(0, 0, 5), 0.5f).OnComplete(() =>
            {
                //pulseSequence.Kill();
                DOTween.Kill(this.gameObject);
                DOTween.Kill(this.transform);
                Destroy(this.gameObject);
            });
        });
        
    }

    void StartPulsing(System.Action onCompleteCallback = null)
    {
        Sequence pulseSequence = DOTween.Sequence();

        for (int i = 0; i < numberOfPulses; i++)
        {
            // Enlarge the object
            pulseSequence.Append(transform.DOScale(originalScale * 1.5f, pulseDuration / 2f));

            // Shrink it back to the original size
            pulseSequence.Append(transform.DOScale(originalScale, pulseDuration / 2f));
        }

        // Set the total duration of the pulsing animation using the onComplete callback
        pulseSequence.OnComplete(() =>
        {
            
            onCompleteCallback?.Invoke();
            
        });

        // If totalTime is greater than zero, loop the animation for the specified time
        if (totalTime > 0)
        {
            pulseSequence.SetLoops(Mathf.FloorToInt(totalTime / (pulseDuration * numberOfPulses)), LoopType.Restart);
        }
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Set the color of the Gizmo

        // Draw a wire sphere with the specified radius
        Gizmos.DrawWireSphere(transform.position, bubbleDetectionRad);
    }
}
