using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soap.Update
{
    public class UpdateManager : SingletonMonoBehaviour<UpdateManager>
    {
        protected override bool IsNeedDontDestoryOnLoad => true;

        public Action UpdateEvent;
        public Action FixedUpdateEvent;

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