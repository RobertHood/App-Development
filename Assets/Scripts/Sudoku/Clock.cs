using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Clock : MonoBehaviour
{
    int hour_ = 0;
    int minute_ = 0;
    int second_ = 0;
    private TextMeshProUGUI textClock;
    float delta_time;
    private bool stopClock = false;

    void Awake()
    {
        textClock = GetComponent<TextMeshProUGUI>();
        delta_time = 0;
    }
    void Start()
    {
        stopClock = false;
    }


    void Update()
    {
        if (stopClock == false)
        {
            delta_time += Time.deltaTime;
            TimeSpan span = TimeSpan.FromSeconds(delta_time);
            string hour = LeadingZero(span.Hours);
            string minute = LeadingZero(span.Minutes);
            string second = LeadingZero(span.Seconds);
            textClock.text = hour + ":" + minute + ":" + second;
        }
    }

    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    public void OnGameOver()
    {
        stopClock = true;
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= OnGameOver;
    }
}
