using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFinish : MonoBehaviour {

    // Use this for initialization
    void Start () {
        timingAndScoring_ = (TimingAndScoring)Component.FindObjectOfType(typeof(TimingAndScoring));
        backwardsCars_ = new List<int>();
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Opponent"))
        {
            if (Vector3.Dot(TrackDirection, other.transform.up) <= 0)
            {
                print("backwards: " + other.name);
                backwardsCars_.Add(other.GetInstanceID());
            }
            else
            {
                if (backwardsCars_.Contains(other.GetInstanceID()))
                {
                    print("removing bad: " + other.name);
                    backwardsCars_.Remove(other.GetInstanceID());
                }
                else
                {
                    timingAndScoring_.ReportLap(other.gameObject);
                }
            }
        }
    }

    private TimingAndScoring timingAndScoring_;

    private List<int> backwardsCars_;

    [SerializeField]
    private Vector3 TrackDirection;
}
