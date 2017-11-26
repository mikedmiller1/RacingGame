using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingAndScoring : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        RaceTime.text = string.Empty;
        PlayerLapTime.text = string.Empty;
        PlayerPosition.text = string.Empty;

        if (!raceState_.TrackState) return;

        raceState_.RaceTime += TimeSpan.FromSeconds(Time.deltaTime);
        RaceTime.text = raceState_.RaceTime.ToString();
        if (raceState_.PlayerData != null && raceState_.PlayerData.LapTimes.Count > 0)
        {
            PlayerLapTime.text = TimeSpan.FromSeconds(raceState_.PlayerData.LapTimes[raceState_.PlayerData.LapTimes.Count - 1]).ToString();
        }

        var playerPosition = raceState_.Positions.IndexOf(raceState_.PlayerData) + 1;
        PlayerPosition.text = string.Format("{0}/{1}", playerPosition, raceState_.CarData.Count);

        foreach (var car in raceState_.CarData)
        {
            var carData = car.Value;
            if (carData.LapTimes.Count > 0)
            {
                carData.LapTimes[carData.LapTimes.Count - 1] += Time.deltaTime;
            }
        }
    }

    public void ReportLap(GameObject car)
    {
        if (!raceState_.TrackState) raceState_.TrackState = true;
        var id = car.GetInstanceID();
        if (!raceState_.CarData.ContainsKey(id))
        {
            raceState_.CarData.Add(id, new CarState());
            if (car.CompareTag("Player"))
            {
                raceState_.PlayerData = raceState_.CarData[id];
            }
        }

        var carData = raceState_.CarData[id];
        carData.CurrentLap++;
        if (carData.LapTimes.Count > 0)
        {
            carData.LastLapTime = carData.LapTimes[carData.LapTimes.Count - 1];
        }

        carData.LapTimes.Add(0);

        // Always do this last
        UpdatePositions(carData);
    }

    // This might be slow as shit. A better programmer would use a reasonable data structure.
    private void UpdatePositions(CarState carData)
    {
        var positions = raceState_.Positions;
        if (positions.Contains(carData))
        {
            positions.Remove(carData);
            for (var i = 0; i != positions.Count; ++i)
            {
                if (positions[i].CurrentLap < carData.CurrentLap)
                {
                    positions.Insert(i, carData);
                    break;
                }
            }
        }

        // The car running last may not get reinserted
        if (!positions.Contains(carData))
        {
            positions.Add(carData);
        }
    }

    private class RaceState
    {
        public RaceState()
        {
            CarData = new Dictionary<int, CarState>();
            TrackState = false;
            CurrentLap = 0;
            TotalLaps = 0;
            Positions = new List<CarState>();
        }

        public bool TrackState { get; set; }
        public int TotalLaps { get; set; }
        public int CurrentLap { get; set; }
        public TimeSpan RaceTime { get; set; }
        public Dictionary<int, CarState> CarData { get; set; }
        public CarState PlayerData { get; set; }
        public List<CarState> Positions { get; set; }
    }

    private class CarState
    {
        public CarState()
        {
            CurrentLap = 0;
            LastLapTime = 0;
            LapTimes = new List<float>();
        }

        public int CurrentLap { get; set; }
        public float LastLapTime { get; set; }
        public List<float> LapTimes { get; set; }
    }

    private RaceState raceState_ = new RaceState();

    [SerializeField]
    private Text RaceTime;

    [SerializeField]
    private Text PlayerLapTime;

    [SerializeField]
    private Text PlayerPosition;
}
