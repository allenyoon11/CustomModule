using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace neuroears.allen.utils
{
    public class CustomInputHandler : IDisposable
    {
        private class InputActionData
        {
            public InputAction action;
            public Key key;
            public Action cb;
        }

        private readonly Dictionary<int, InputActionData> actionMap = new Dictionary<int, InputActionData>();
        private bool isPaused = false;

        public bool AddAction(int id, Key key, Action callback)
        {
            if (actionMap.ContainsKey(id)) return false;

            string keyName = key.ToString().ToLower();
            var inputAction = new InputAction(binding: $"<Keyboard>/{keyName}");
            inputAction.performed += _ => { if (!isPaused) callback?.Invoke(); };
            inputAction.Enable();

            actionMap[id] = new InputActionData { action = inputAction, key = key, cb = callback };
            return true;
        }

        public bool ChangeKey(int id, Key newKey)
        {
            if (!actionMap.ContainsKey(id)) return false;

            var data = actionMap[id];
            data.action.Disable();

            string newKeyName = newKey.ToString().ToLower();
            data.action = new InputAction(binding: $"<Keyboard>/{newKeyName}");
            data.action.performed += _ => { if (!isPaused) data.cb?.Invoke(); }; 
            data.action.Enable();

            data.key = newKey;
            return true;
        }

        public bool ChangeAction(int id, Action newAction)
        {
            if (!actionMap.ContainsKey(id)) return false;

            var data = actionMap[id];
            data.cb = newAction;
            data.action.performed -= _ => data.cb?.Invoke();
            data.action.performed += _ => { if (!isPaused) data.cb?.Invoke(); }; 
            return true;
        }
        public Dictionary<int, (Key key, Action cb)> GetActions()
        {
            var actions = new Dictionary<int, (Key, Action)>();

            foreach (var entry in actionMap)
            {
                actions[entry.Key] = (entry.Value.key, entry.Value.cb);
            }

            return actions;
        }
        public void PauseInputHandling()
        {
            isPaused = true;
        }
        public void ResumeInputHandling()
        {
            isPaused = false;
        }
        public void Dispose()
        {
            foreach (var entry in actionMap.Values)
            {
                entry.action.Disable();
                entry.action.Dispose();
            }
            actionMap.Clear();
        }
    }

}
