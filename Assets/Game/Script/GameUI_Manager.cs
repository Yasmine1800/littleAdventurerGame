using UnityEngine;
using UnityEngine.UI;

public class GameUI_Manager : MonoBehaviour
{
    
    public GameManager GM;
    public TMPro.TextMeshProUGUI CoinText;

    public Slider HealthSlider;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;

    private enum GameUI_State{
       GamePlay,
        Pause,
        GameOver,
        GameIsFinished
    }

    GameUI_State currentState;

    private void Start(){
        SwitchUIState(GameUI_State.GamePlay);
    }

    void Update(){

        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        CoinText.text = GM.playerCharacter.Coin.ToString();

    }

    private void SwitchUIState(GameUI_State state){

        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);


        Time.timeScale = 1;

        switch(state){
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.Pause:
                UI_Pause.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameUI_State.GameOver:
                UI_GameOver.SetActive(true);
                break;
            case GameUI_State.GameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break;
        }

        currentState = state;
    }

    public void TogglePauseUI(){

        if(currentState == GameUI_State.GamePlay)
            SwitchUIState(GameUI_State.Pause);
        else if(currentState == GameUI_State.Pause)
            SwitchUIState(GameUI_State.GamePlay);
    }

    public void Button_MainMenu(){

        GM.ReturnToTheMainMenu();
        
    }

    public void Button_Restart(){
        GM.Restart();
    }

    public void ShowGameOverUI(){
        SwitchUIState(GameUI_State.GameOver);
    }

    public void ShowGameIsFinishedUI(){
        SwitchUIState(GameUI_State.GameIsFinished);
    }
}
