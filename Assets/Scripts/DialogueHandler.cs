using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    PlayerData PData;
    public List<NpcLogic.LineData> lines;
    public GameObject MainTextHolder;
    public GameObject MainText;
    public GameObject PersonTalkingName;
    public GameObject PersonTalkIcon;
    public GameObject Canvas;
    public GameObject Inventory;
    public GameObject ChoiceHolder;
    public GameObject[] Choices = new GameObject[4];
    TMPro.TextMeshProUGUI Text;
    StateManager StateManager;
    bool waitingForPlayerInput;
    int currentLine;
    bool allFlagsMet;
    bool pickingAnOption;
    private void Start()
    {
        //Gets needed scripts and objects
        PData = GetComponent<PlayerData>();
        StateManager = GetComponent<StateManager>();
        Text = MainText.GetComponent<TMPro.TextMeshProUGUI>();
    }
    /// <summary>
    /// Starts the dialogue with the npc
    /// </summary>
    public void StartDialogue()
    {
        //Sets everything to default
        currentLine = 0;
        StateManager.State = StateManager.GameState.Talking;
        Inventory.SetActive(false);
        //Shows dialogue menu and starts the dialogue
        MainTextHolder.SetActive(true);
        Canvas.SetActive(true);
        StartCoroutine(ProgressDialogue());
    }
    private void Update()
    {
        //If no option is being picked and game is waiting for the player to continue the dialogue
        if (waitingForPlayerInput && !pickingAnOption)
        {
            //Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) || This was here but caused issues
            if (Input.GetMouseButton(0))
            {
                if (currentLine + 1 > lines.Count || lines[currentLine].lastLine == true)
                {
                    EndDialogue();
                }
                else if(!lines[currentLine].lastLine)
                {
                    waitingForPlayerInput = false;
                    StartCoroutine(ProgressDialogue());
                }
            }
        }
        //if the player is picking an option show the options avaliable to the player
        else if (pickingAnOption)
        {
            //Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) || here as well
            if (Input.GetMouseButton(0))
            {
                MainText.SetActive(false);
                ChoiceHolder.SetActive(true);
            }
        }
    }
    /// <summary>
    /// Writes out dialogue 1 character at a time until the whole string has been written
    /// </summary>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    IEnumerator WriteDialogue(string dialogue)
    {
        string currentLineBeingSpoken = "";
        for (int i = 0; i < dialogue.Length; i++)
        {
            currentLineBeingSpoken += dialogue[i];
            Text.SetText(currentLineBeingSpoken);
            yield return new WaitForSecondsRealtime(0.025f);
        }
        //Waits for player input once finished
        waitingForPlayerInput = true;
        yield break;
    }
    /// <summary>
    /// Ends the current dialogue with the npc
    /// </summary>
    public void EndDialogue()
    {
        //Sets everything back to how it was before interacting with the npc
        StateManager.State = StateManager.GameState.Overworld;
        MainTextHolder.SetActive(false);
        Canvas.SetActive(false);
        Inventory.SetActive(true);
        waitingForPlayerInput = false;
        MainText.GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
    /// <summary>
    /// Progresses the dialogue along with the help of flags and choices.
    /// </summary>
    /// <returns></returns>
    IEnumerator ProgressDialogue()
    {
        allFlagsMet = true;
        pickingAnOption = false;
        //Checks if all flags for the current line to be spoken are met otherwise it skips that line to the specified line given
        if(lines[currentLine].skipTo != -1)
        {
            int flagsMet = 0;
            for (int i = 0; i < lines[currentLine].flagsRequired.Count; i++)
            {
                int flag = lines[currentLine].flagsRequired[i].flag;
                int state = lines[currentLine].flagsRequired[i].state;
                if(PData.Flags[flag] == state)
                {
                    flagsMet++;
                }
            }
            if(flagsMet != lines[currentLine].flagsRequired.Count)
            {
                allFlagsMet = false;
            }
            
        }
        //Checks all choices and the options avaliable
        if(lines[currentLine].choices[0].goTo != -1)
        {
            pickingAnOption = true;
            for (int i = 0; i < Choices.Length; i++)
            {
                Choices[i].SetActive(false);
            }
            for (int i = 0; i < lines[currentLine].choices.Count; i++)
            {
                Choices[i].SetActive(true);
                Choices[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(lines[currentLine].choices[i].choiceText);
            }
            yield break;
        }
        //These check the ending conditions from checking everything beforehand. (Look at code above comment)
        if (allFlagsMet)
        {
            //Changes any flags as needed
            if(lines[currentLine].setFlag.flag != -1)
            {
                PData.Flags[lines[currentLine].setFlag.flag] = lines[currentLine].setFlag.state;
            }
            PersonTalkingName.GetComponent<TMPro.TextMeshProUGUI>().SetText(lines[currentLine].name);
            yield return StartCoroutine(WriteDialogue(lines[currentLine].dialouge));
            if (!lines[currentLine].lastLine)
            {
                currentLine++;
            }
        }
        else
        {
            //If no flags are met skip to the line specified and look through that line's data
            currentLine = lines[currentLine].skipTo;
            yield return StartCoroutine(ProgressDialogue());
        }
        yield break;
    }
    /// <summary>
    /// Based on what button is pressed the dialogue continues differently by sending you to another line specified by the choice
    /// </summary>
    /// <param name="Choice"></param>
    public void ChoicePicked(int Choice)
    {
        currentLine = lines[currentLine].choices[Choice].goTo;        
        MainText.SetActive(true);
        ChoiceHolder.SetActive(false);
        //Continues dialogue from the new line the player has been sent to based on their choice
        StartCoroutine(ProgressDialogue());
    }
}
