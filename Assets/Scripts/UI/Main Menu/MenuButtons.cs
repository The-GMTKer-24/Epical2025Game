using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuObjects;
        [SerializeField] private GameObject creditsMenuObjects;
        
        [SerializeField] private string sceneToLoad = "Main";

        public void Awake()
        {
            mainMenuObjects.SetActive(true);
            creditsMenuObjects.SetActive(false);
        }
        
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

        public void Credits()
        {
            Debug.Log("The system will credits now!");
            
            mainMenuObjects.SetActive(false);
            creditsMenuObjects.SetActive(true);
        }

        public void CloseCredits()
        {
            Debug.Log("The system will close credits now!");
            
            mainMenuObjects.SetActive(true);
            creditsMenuObjects.SetActive(false);
        }
    }
}