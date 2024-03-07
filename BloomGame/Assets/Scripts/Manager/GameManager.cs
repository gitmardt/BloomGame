using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameState startState = GameState.MainMenu;
    public GameState state;

    public static event Action<GameState> OnGameStateChanged;

    private void Start()
    {
        UpdateGameState(startState);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Combat:
                break;
            case GameState.Gambling:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    MainMenu,
    Combat,
    Gambling,
}
