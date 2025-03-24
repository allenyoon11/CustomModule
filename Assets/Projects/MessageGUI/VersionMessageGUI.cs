using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class VersionMessageGUI : MessageGUI
    {
        //protected override int FontSize => 20;
        //protected override Color FontColor => Color.black;
        //protected override float TextBoxWidth => 100;
        //protected override float TextBoxHeight => 30;
        //protected override ScreenPosition CurrentScreenPosition => currentScreenPosition;
        protected override void SetMessage()
        {
            message = $"v{Application.version}";
        }

    }

}
