using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Crab : MonoBehaviour
{
    public float wanderSpeed;
    public float stamina;
    public List<Crab> children = new List<Crab>();
    public crabState curCrabState = crabState.wander;
    public enum crabState
    {
        wander,
        hungry,
        play,
        sleep,
        mate,
        dead
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetType == )
    }
    private void Update()
    {
        
    }

    void updateCrabState(crabState newCrabState)
    {
        switch (newCrabState)
        {
            case crabState.wander:
                break;
            case crabState.hungry:
                break;
            case crabState.play:
                break;
            case crabState.sleep:
                break;
            case crabState.dead:
                break;
            case crabState.mate:
                break;
            default:
                break;
        }
    }
}
