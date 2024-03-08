using Cinemachine;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameState playState = GameState.Combat;
    public CinemachineVirtualCameraBase menuCam, combatCam;
    public GameObject mainMenuUI;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if(state == GameState.MainMenu)
        {
            Player.instance.hitmarkerAM.PlaySpriteAnimation(Player.instance.Out, Player.instance.hitmarker.image);
            menuCam.Priority = 1;
            combatCam.Priority = 0;
            mainMenuUI.SetActive(true);
        }
    }

    public void PlayGame()
    {
        GameManager.instance.UpdateGameState(playState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
