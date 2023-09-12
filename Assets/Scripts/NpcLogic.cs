using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcLogic : MonoBehaviour
{
    public TextAsset data;
    public GameObject inputShow;
    string[] unprocessedLines;
    public List<LineData> lines;
    bool playerInRange;
    DialogueHandler DialogueHandler;
    StateManager StateManager;
    public struct LineData
    {
        public string charIconPath;
        public string name;
        public string dialouge;
        public List<Choice> choices;
        public bool lastLine;
        public List<FlagInfo> flagsRequired;
        public int skipTo;
        public FlagInfo setFlag;
    }
    public struct Choice
    {
        public int goTo;
        public string choiceText;
    }
    public struct FlagInfo
    {
        public int flag;
        public int state;
    }
    private void Start()
    {
        //Loads all dialogue data into the Npc's memory and grabs scripts relevant to Npc handling
        LoadDialogue();
        DialogueHandler = GameObject.Find("GameManager").GetComponent<DialogueHandler>();
        StateManager = GameObject.Find("GameManager").GetComponent<StateManager>();
    }
    private void Update()
    {
        //If player is in range and not talking to anyone they can press E to start talking to the Npc
        if (playerInRange)
        {
            if (Input.GetKey(KeyCode.E) && StateManager.State == StateManager.GameState.Overworld)
            {
                DialogueHandler.lines = lines;
                DialogueHandler.StartDialogue();
            }
            if(StateManager.State == StateManager.GameState.Talking)
            {
                toggleInRange(false);
            }
        }
    }
    /// <summary>
    /// Looks through targeted text file and loads all data into the Npc's memory
    /// </summary>
    public void LoadDialogue()
    {
        LineData lineData;
        Choice choice;
        FlagInfo flagInfo;
        lines = new();
        unprocessedLines = data.text.Split(Environment.NewLine);
        int lineCount = 1;
        string[] processingArray;
        string[] Processor;
        string[] array;
        while(lineCount < unprocessedLines.Length)
        {
            lineData = new();
            lineData.choices = new();
            lineData.flagsRequired = new();
            processingArray = unprocessedLines[lineCount].Split('*');
            lineData.name = processingArray[0];
            lineData.charIconPath = processingArray[1];
            lineData.dialouge = processingArray[2];
            //checks if any choices are contained within the line
            if(processingArray[3] != "NoChoices")
            {
                //Splits the different choices apart
                Processor = processingArray[3].Split('|');
                for (int i = 0; i < Processor.Length; i++)
                {
                    choice = new();
                    array = Processor[i].Split('/');
                    choice.choiceText = array[0];
                    choice.goTo = int.Parse(array[1]);
                    lineData.choices.Add(choice);
                }
            }
            else
            {
                choice = new();
                choice.choiceText = "null";
                choice.goTo = -1;
                lineData.choices.Add(choice);
            }
            lineData.lastLine = bool.Parse(processingArray[4]);
            //processes all of the flagData
            lineData.skipTo = -1;
            if(processingArray[5] != "NoFlagReq")
            {
                //Splits them into smaller groups if there is more than 1 flag
                Processor = processingArray[5].Split('|');
                for (int i = 0; i < Processor.Length; i++)
                {
                    flagInfo = new();
                    array = Processor[i].Split('/');
                    flagInfo.flag = int.Parse(array[0]);
                    flagInfo.state = int.Parse(array[1]);
                    lineData.flagsRequired.Add(flagInfo);
                }
                lineData.skipTo = int.Parse(processingArray[6]);
            }
            //adds the flag toggle to the lineData if present
            if(processingArray[7].Trim() != "NoFlagsToChange")
            {
                flagInfo = new();
                Processor = processingArray[7].Split('/');
                flagInfo.flag = int.Parse(Processor[0]);
                flagInfo.state = int.Parse(Processor[1]);
                lineData.setFlag = flagInfo;
            }
            else
            {
                flagInfo = new();
                flagInfo.flag = -1;
                flagInfo.state = -1;
                lineData.setFlag = flagInfo;
            }
            lines.Add(lineData);
            lineCount++;
        }
    }
    /// <summary>
    /// A toggle for when the player is in range of the Npc and shows the player they can interact
    /// </summary>
    /// <param name="toggle"></param>
    public void toggleInRange(bool toggle)
    {
        inputShow.SetActive(toggle);
        playerInRange = toggle;
    }
}