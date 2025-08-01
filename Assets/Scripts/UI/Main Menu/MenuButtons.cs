using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MenuButtons : MonoBehaviour
    {
        public string sceneToLoad = "Main";
        
        public void PlayGame()
        {
            Debug.Log("The system will play game now!");
            SceneManager.LoadScene(sceneToLoad);
        }
        
        public void QuitGame()
        {
            Debug.Log("The system will shut down now!");
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}