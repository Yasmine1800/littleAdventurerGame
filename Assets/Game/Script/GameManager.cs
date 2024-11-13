using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameUI_Manager GameUI_Manager;
    public Character playerCharacter;

    private bool gameIsOver;

    private void Awake(){

        playerCharacter =  GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver(){

       GameUI_Manager.ShowGameOverUI();
    }

    public void GameIsFinished(){

        GameUI_Manager.ShowGameIsFinishedUI();
    }

    void Update(){

        if(gameIsOver)
            return;

        if(Input.GetKeyDown(KeyCode.Escape)){
            GameUI_Manager.TogglePauseUI();
        }

        
        if(playerCharacter.CurrentState == Character.CharacterState.Dead){
            gameIsOver = true;
            GameOver();
        }
    }

    public void ReturnToTheMainMenu(){

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
