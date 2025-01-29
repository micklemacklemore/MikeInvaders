using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text textGameOver; 
    // Start is called before the first frame update
    void Start()
    {
        GameManager.EnsureInstance(); 
        textGameOver.text = "Game Over!\nScore: " + GameManager.Instance.Score; 
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame(); 
    }

    public void NewGame()
    {
        GameManager.Instance.StartNewGame(); 
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
