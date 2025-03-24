using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class FpsMessageGUI : MessageGUI
    {
        //protected override int FontSize => 30;
        //protected override Color FontColor => Color.black;
        //protected override float TextBoxWidth => 250;
        //protected override float TextBoxHeight => 50;
        //protected override ScreenPosition CurrentScreenPosition => currentScreenPosition;
        //
        private float fps = 0f;
        private float deltaTime = 0.0f;
        private float timeSinceLastUpdate = 0f;
        public float updateInterval = 0.5f;

        protected override void Update()
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
        protected override void SetMessage()
        {
            message = string.Format(" {0:0.0} ms ({1:0.} fps)", deltaTime * 1000.0f, fps);
        }
    }

}
