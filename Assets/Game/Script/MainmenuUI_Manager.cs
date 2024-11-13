using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainmenuUI_Manager : MonoBehaviour
{
    public void Button_Start(){
        SceneManager.LoadScene("GameScene");

    }

    public void Button_Quit(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
