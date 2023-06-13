using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonMgr : MonoBehaviour
{
    
    public GameObject playB;
    public GameObject stopB;
    public Button soundB;
    public Button animateB;
    public TMP_Dropdown dropdown;
    public GameObject setFirstAI;
    public GameObject setSecondAI;
    public GameObject chessAudio;

    private bool music = true;
    private bool animate = true; 
    private int playerMode = 0;
    private int firstAILevel = 0;
    private int secondAILevel = 0;

    public void Play(){
        playB.SetActive(false);
        stopB.SetActive(true);
        dropdown.interactable = false;
        setFirstAI.GetComponentInChildren<Slider>().interactable = false;
        setSecondAI.GetComponentInChildren<Slider>().interactable = false;
        ChessMgr.instance.InitialSetup(playerMode);
        if (playerMode == 0) { // Player vs AI
            ChessMgr.instance.AILevel = firstAILevel;
        }
        if (playerMode == 2) { // AI vs AI
            ChessMgr.instance.AILevel = firstAILevel;
            ChessMgr.instance.SecondAILevel = secondAILevel;
        }
        TileSelect selector = GetComponent<TileSelect>();
        selector.EnterState();
   }

   public void Stop() {
        playB.SetActive(true);
        ChessMgr.instance.ClearBoard();
        stopB.SetActive(false);
        dropdown.interactable = true;
        setFirstAI.GetComponentInChildren<Slider>().interactable = true;
        setSecondAI.GetComponentInChildren<Slider>().interactable = true;
   }

   public void ToggleMusic() {
        music = !music;
        if (music == false) {
            soundB.GetComponent<Image>().color = Color.grey;
            soundB.GetComponentInChildren<TextMeshProUGUI>().text = "MUSIC (OFF)";
            chessAudio.GetComponent<AudioSource>().Stop();
        } else {
            soundB.GetComponent<Image>().color = Color.green;
            soundB.GetComponentInChildren<TextMeshProUGUI>().text = "MUSIC (ON)";
            chessAudio.GetComponent<AudioSource>().Play();
        }
   }

   public void ToggleAnimation() {
        animate = !animate;
   }

   public void SetPlayers() {
        playerMode = dropdown.value;
        switch (playerMode) {
            case 0:
                setFirstAI.SetActive(true);
                setSecondAI.SetActive(false);
                break;
            case 1:
                setFirstAI.SetActive(false);
                setSecondAI.SetActive(false);
                break;
            case 2:
                setFirstAI.SetActive(true);
                setSecondAI.SetActive(true);
                break;
        }
   }

   public void SetFirstAILevel() {
        //Debug.Log(setFirstAI.transform.GetChild(0));
        Slider slider = setFirstAI.GetComponentInChildren<Slider>();
        TextMeshProUGUI mText = setFirstAI.GetComponentInChildren<TextMeshProUGUI>();
        mText.text = "Blue AI Level: " + slider.value;
        firstAILevel = (int) slider.value;
   }

   public void SetSecondAILevel() {
        Slider slider = setSecondAI.GetComponentInChildren<Slider>();
        TextMeshProUGUI mText = setSecondAI.GetComponentInChildren<TextMeshProUGUI>();
        mText.text = "White AI Level: " + slider.value;
        secondAILevel = (int) slider.value;
   }

    public void Quit() {
        Application.Quit();
    }

}
