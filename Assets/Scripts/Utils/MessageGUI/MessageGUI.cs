using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.utils
{
    public abstract class MessageGUI : MonoBehaviour
    {
        public int fontSize = 16;
        public Color fontColor = Color.black;
        public float textBoxWidth = 200;
        public float textBoxHeight = 50;
        public ScreenPosition currentScreenPosition = ScreenPosition.BottomRight;
        //
        protected GUIStyle guiStyle;
        protected string message;
        protected virtual int FontSize => fontSize;
        protected virtual Color FontColor => fontColor;
        protected virtual float TextBoxWidth => textBoxWidth;
        protected virtual float TextBoxHeight => textBoxHeight;
        protected virtual ScreenPosition CurrentScreenPosition => currentScreenPosition;
        protected abstract void SetMessage();
        protected virtual void Start()
        {
            guiStyle = new GUIStyle();
        }
        protected virtual void Update()
        {

        }
        protected virtual void OnGUI()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            Rect position = GetPositionRect(CurrentScreenPosition); // 위치 계산
            SetMessage();
            SetGuiStyle();
            GUI.Label(position, message, guiStyle); // 메시지 표시
        }
        protected void SetGuiStyle()
        {
            guiStyle.fontSize = FontSize;
            guiStyle.normal.textColor = FontColor;
            guiStyle.alignment = TextAnchor.MiddleCenter;
        }
        protected Rect GetPositionRect(ScreenPosition screenPosition)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            switch (screenPosition)
            {
                case ScreenPosition.TopLeft:
                    return new Rect(0, 0, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.TopCenter:
                    return new Rect((screenWidth - TextBoxWidth) / 2, 0, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.TopRight:
                    return new Rect(screenWidth - TextBoxWidth, 0, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.MiddleLeft:
                    return new Rect(0, (screenHeight - TextBoxHeight) / 2, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.MiddleCenter:
                    return new Rect((screenWidth - TextBoxWidth) / 2, (screenHeight - TextBoxHeight) / 2, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.MiddleRight:
                    return new Rect(screenWidth - TextBoxWidth, (screenHeight - TextBoxHeight) / 2, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.BottomLeft:
                    return new Rect(0, screenHeight - TextBoxHeight, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.BottomCenter:
                    return new Rect((screenWidth - TextBoxWidth) / 2, screenHeight - TextBoxHeight, TextBoxWidth, TextBoxHeight);
                case ScreenPosition.BottomRight:
                    return new Rect(screenWidth - TextBoxWidth, screenHeight - TextBoxHeight, TextBoxWidth, TextBoxHeight);
                default:
                    return new Rect((screenWidth - TextBoxWidth) / 2, (screenHeight - TextBoxHeight) / 2, TextBoxWidth, TextBoxHeight);
            }
        }
        public enum ScreenPosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
    }

}
