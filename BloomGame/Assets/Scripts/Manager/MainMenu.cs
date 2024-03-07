using Cinemachine;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameState playState = GameState.Combat;
    public CinemachineVirtualCameraBase menuCam, combatCam;
    public GameObject mainMenuUI;

    private Player player;
    private GameManager manager;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        player = Player.instance;
        manager = GameManager.instance;
    }

    private void OnGameStateChanged(GameState state)
    {
        if(state == GameState.MainMenu)
        {
            player.hitmarkerAM.PlaySpriteAnimation(player.Out, player.hitmarker.image);
            menuCam.Priority = 1;
            combatCam.Priority = 0;
            mainMenuUI.SetActive(true);
        }
    }

    public void PlayGame()
    {
        manager.UpdateGameState(playState);
        menuCam.Priority = 0;
        combatCam.Priority = 1;
        mainMenuUI.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
