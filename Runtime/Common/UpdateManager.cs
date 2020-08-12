using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soap.Update
{
    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance;

        public Action UpdateEvent;
        public Action FixedUpdateEvent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke();
        }

        public void RequestUpdate(Action _action)
        {
            UpdateEvent += _action;
        }

        public void RemoveUpdate(Action _action)
        {
            UpdateEvent -= _action;
        }

        public void RequestFixedUpdate(Action _action)
        {
            FixedUpdateEvent += _action;
        }

        public void RemoveFixedUpdate(Action _action)
        {
            FixedUpdateEvent -= _action;
        }
    }
}