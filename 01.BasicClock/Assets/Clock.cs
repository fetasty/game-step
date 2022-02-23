using System.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform hourTransform;
    public Transform minuteTransform;
    public Transform secondTransform;
    public bool continuous = false;

    private const float anglePerHour = 30f;
    private const float anglePerMinute = 6f;
    private const float anglePerSecond = 6f;

    void Update()
    {
        UpdatePointer();
    }

    private void UpdatePointer()
    {
        if (continuous)
        {
            var time = DateTime.Now.TimeOfDay;
            hourTransform.rotation = Quaternion.Euler(0f, (float) time.TotalHours * anglePerHour, 0f);
            minuteTransform.rotation = Quaternion.Euler(0f, (float) time.TotalMinutes * anglePerMinute, 0f);
            secondTransform.rotation = Quaternion.Euler(0f, (float) time.TotalSeconds * anglePerSecond, 0f);
        }
        else
        {
            var time = DateTime.Now;
            hourTransform.rotation = Quaternion.Euler(0f, (float) time.Hour * anglePerHour, 0f);
            minuteTransform.rotation = Quaternion.Euler(0f, (float) time.Minute * anglePerMinute, 0f);
            secondTransform.rotation = Quaternion.Euler(0f, (float) time.Second * anglePerSecond, 0f);
        }
    }
}
