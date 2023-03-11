using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadDataRequester : MonoBehaviour
{
    static ThreadDataRequester instance;
    Queue<ThreadData> dataQueue = new Queue<ThreadData>();

    private void Awake() 
    {
        instance = FindObjectOfType<ThreadDataRequester>();
    }


    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate
        {
            instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
    }


    private void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadData(callback, data));
        }
    }


    private void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadData threadData = dataQueue.Dequeue();
                threadData.callback(threadData.parameter);
            }
        }
    }



    struct ThreadData
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadData(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
