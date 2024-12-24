using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace allen.utils
{
    public class FpsChecker : MonoBehaviour
    {
        public int fontSize = 30;
        public Color fontColor = Color.black;
        public TextAnchor textAnchor = TextAnchor.LowerLeft;
        public float updateInterval = 0.5f;

        private float fps = 0f;
        private float deltaTime = 0.0f;
        private float timeSinceLastUpdate = 0f;
        private bool isOn = true;
        private void Start()
        {
            //INFO::set font size
            fontSize = Mathf.Clamp(fontSize, 1, 100);

            //INFO::switch show/hide 'fps' when 'F' key down 3 times in 0.3s
            var keyDownStream = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.F));
            keyDownStream
                .Buffer(keyDownStream.Throttle(TimeSpan.FromMilliseconds(300)))
                .Where(x => x.Count >= 3)
                .Subscribe(_ => isOn = !isOn).AddTo(this);
        }

        void Update()
        {
            //INFO::'time' since last frame
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            timeSinceLastUpdate += Time.unscaledDeltaTime;

            //INFO::update 'fps' every 'updateInterval'
            if (timeSinceLastUpdate >= updateInterval)
            {
                fps = 1.0f / deltaTime;
                timeSinceLastUpdate = 0.0f;
            }
        }
        private void OnDisable()
        {
            isOn = false;
        }

        void OnGUI()
        {
            if (isOn)
            {
                //INFO::set GUI style
                GUIStyle style = new GUIStyle();
                int w = Screen.width, h = Screen.height;
                Rect rect = new Rect(0, 0, w, h);
                style.alignment = TextAnchor.LowerLeft;
                style.fontSize = fontSize;
                style.normal.textColor = fontColor;

                //INFO::set text
                string text = string.Format(" {0:0.0} ms ({1:0.} fps)", deltaTime * 1000.0f, fps);

                //INFO::show text
                GUI.Label(rect, text, style);
            }
        }
    }

}
