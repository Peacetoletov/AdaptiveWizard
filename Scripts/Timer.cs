using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private readonly float basePeriod;
    private readonly float periodVariance;
    private float period;
    private readonly float initialDelay;
    private bool initialDelayActive;
    private float time = 0f;

    public Timer(float basePeriod, float periodVariance=0f, float initialDelay=0f) {
        if (periodVariance < 0f) {
            throw new System.ArgumentException("Attempted to create a timer with negative period variance, which is forbidden.");
        }

        this.basePeriod = basePeriod;
        this.periodVariance = periodVariance;
        this.period = NewPeriod();
        this.initialDelay = initialDelay;
        this.initialDelayActive = Mathf.Abs(initialDelay) > 0.00001f;
    }

    public bool UpdateAndCheck() {
        this.time += Time.deltaTime;
        if ((initialDelayActive && time >= period + initialDelay) || 
                (!initialDelayActive && time >= period)) {
            this.time = 0f;
            this.period = NewPeriod();
            initialDelayActive = false;
            return true;
        }
        return false;
    }

    private float NewPeriod() {
        if (periodVariance < 0.00001f) {
            return basePeriod;
        }
        return basePeriod + Random.Range(0f, periodVariance);
    }
}
