using UnityEngine;


public class GameManager : MonoBehaviour
{
    public SaveSystem saveSystem;

    void Start()
    {
        if (saveSystem.SaveExists())
            saveSystem.LoadGame();
    }
}