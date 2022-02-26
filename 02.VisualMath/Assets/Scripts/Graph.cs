using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab = default;
    [SerializeField, Range(10, 200)]
    int resolution = 50;
    [SerializeField]
    FunctionLibrary.FunctionName function = default;
    [SerializeField, Min(0f)]
    float functionDuration = 1f;
    [SerializeField, Min(0f)]
    float transitionDuration = 1f;
    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;
    Transform[] points;
    void Awake()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int i = 0; i < points.Length; ++i)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            function = FunctionLibrary.GetNextFunctionName(function);
        }

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }
    void UpdateFunction()
    {
        float step = 2f / resolution;
        Function f = FunctionLibrary.GetFunction(function);
        float u = 0f;
        float v = 0f;
        float t = Time.time;
        for (int i = 0, j = 0, k = 0; i < points.Length; ++i, ++j)
        {
            if (j == resolution)
            {
                j = 0;
                ++k;
                v = (k + 0.5f) * step - 1f;
            }
            u = (j + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, t);
        }
    }
    void UpdateFunctionTransition()
    {
        Function from = FunctionLibrary.GetFunction(transitionFunction);
        Function to = FunctionLibrary.GetFunction(function);
        float progress = duration / transitionDuration;
        float step = 2f / resolution;
        float u = 0f;
        float v = 0f;
        float t = Time.time;
        for (int i = 0, j = 0, k = 0; i < points.Length; ++i, ++j)
        {
            if (j == resolution)
            {
                j = 0;
                ++k;
                v = (k + 0.5f) * step - 1f;
            }
            u = (j + 0.5f) * step - 1f;
            points[i].localPosition = FunctionLibrary.Morph(
                u, v, t, from, to, progress
            );
        }
    }
}

