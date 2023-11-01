using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SGAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public int count;
    private void Update()
    {
        count = GameObject.FindGameObjectsWithTag("sg").Length;
        if (count <= 0)
        {
            audioSource.Stop();
        }
    }
   
}
