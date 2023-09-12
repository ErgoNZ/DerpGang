using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeHandler : MonoBehaviour
{
    PlayerData PData;
    public List<NpcLogic.LineData> lines;
    public GameObject MainTextHolder;
    public GameObject MainText;
    public GameObject PersonTalkingName;
    public GameObject PersonTalkIcon;
    public GameObject Canvas;
    public GameObject Inventory;
    StateManager StateManager;
    bool waitingForPlayerInput;
    int currentLine;
    bool allFlagsMet;
    private void Start()
    {
        PData = GetComponent<PlayerData>();
        StateManager = GetComponent<StateManager>();
    }
    public void StartDialogue()
    {
        currentLine = 0;
        StateManager.State = StateManager.GameState.Talking;
        Inventory.SetActive(false);
        MainTextHolder.SetActive(true);
        Canvas.SetActive(true);
        StartCoroutine(ProgressDialogue());
    }
    private void Update()
    {
        if (waitingForPlayerInput)
        {
            if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
            {
                StartCoroutine(ProgressDialogue());
            }
        }
    }
    IEnumerator WriteDialogue(string dialogue)
    {
        string currentLineBeingSpoken = "";
        for (int i = 0; i < dialogue.Length; i++)
        {
            currentLineBeingSpoken += dialogue[i];
            MainText.GetComponent<TMPro.TextMeshProUGUI>().SetText(currentLineBeingSpoken);
            yield return new WaitForSecondsRealtime(0.025f);
        }
        waitingForPlayerInput = true;
        yield break;
    }
    public void EndDialogue()
    {
        StateManager.State = StateManager.GameState.Overworld;
        MainTextHolder.SetActive(false);
        Canvas.SetActive(false);
        Inventory.SetActive(true);
        MainText.GetComponent<TMPro.TextMeshProUGUI>().SetText("");
    }
    IEnumerator ProgressDialogue()
    {
        allFlagsMet = true;
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
            if(flagsMet - 1 != lines[currentLine].flagsRequired.Count)
            {
                allFlagsMet = false;
            }
            
        }
        if (lines[currentLine].lastLine)
        {
            EndDialogue();
            yield break;
        }
        if (allFlagsMet)
        {
            yield return StartCoroutine(WriteDialogue(lines[currentLine].dialouge));
            currentLine++;
        }
        else
        {
            currentLine = lines[currentLine].skipTo;
            yield return StartCoroutine(ProgressDialogue());
        }
        yield break;
    }
}
