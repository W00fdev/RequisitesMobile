using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class UnityMainThread : MonoBehaviour
{
    internal static UnityMainThread wkr;
    private Queue<Action<string>> _jobs = new Queue<Action<string>>(15);
    private Queue<string> _jobsParams = new Queue<string>(15);

    private void Awake()
    {
        wkr = this;
    }

    private void Update()
    {
        while(_jobs.Count > 0)
        {
            _jobs.Dequeue()?.Invoke(_jobsParams.Dequeue());
        }
    }

    internal void AddJob(Action<string> newJob, string newJobParam)
    {
        _jobs.Enqueue(newJob);
        _jobsParams.Enqueue(newJobParam);
    }
}
