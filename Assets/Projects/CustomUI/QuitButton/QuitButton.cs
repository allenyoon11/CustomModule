using System;
using UnityEngine;
using UnityEngine.UI;

namespace neuroears.allen.utils
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        private Button quitBtn;

        private void Awake()
        {
            quitBtn = GetComponent<Button>();
            quitBtn.onClick.AddListener(() => OnQuitClicked());
        }

        private void OnQuitClicked()
        {
            //Debug.Log("application quit");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

}
