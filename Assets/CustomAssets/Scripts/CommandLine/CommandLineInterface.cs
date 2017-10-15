using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandLineInterface : MonoBehaviour {

    public string cliPromptText = " >";
    public char terminationCharacter = '\n';
    private string terminationCharacterStr = "\n";
    private string commandHistory;
    private string currentCommand;
    private string input;
    private GameInterpreter gameInterpreter;
    private static CommandLineInterface instance;
    private Text commandLineText;
    private ScrollRect scrollbar;
    private Canvas canvas;
	// Use this for initialization
	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
            gameObject.SetActive(false); // this object starts out disabled
            gameInterpreter = GameInterpreter.getInstance();
            commandLineText = GetComponentInChildren<Text>(); // get the text object to hold the text
            commandLineText.text = cliPromptText; // set the text to start with the prompt text
            scrollbar = GetComponentInChildren<ScrollRect>();
            canvas = GetComponentInParent<Canvas>();
            commandHistory = ""; // set history empty string
            terminationCharacterStr = new string(new char[]{ terminationCharacter });
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeInHierarchy) {
            getInput();

            if (!input.Equals("")) { // not empty input
                // there was input, so set scrollbar to the bottom.
                //scrollbar.verticalNormalizedPosition = 0.0f;

                // now handle the input and update the text in the command line
                if (input.Contains("\b") && input.Length > 1) { // do nothing because backspace cancels other characters
                } else if (input.Contains("\b") && input.Length <= 1) { // backspace character encountered, so backspace
                    if (currentCommand.Length > 0) { // cant backspace if nothing there
                        currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                    }
                } else if (input.Contains(terminationCharacterStr)) {
                    string[] parts = input.Split(terminationCharacter);
                    currentCommand += parts[0]; // add part before the enter
                    Command command = new Command(currentCommand); // make the command
                    gameInterpreter.enqueueCommand(command);
                    commandHistory += (cliPromptText + currentCommand + terminationCharacter); // add the command to the history
                    currentCommand = parts[1];
                }
                else {
                    currentCommand += input;
                }
                commandLineText.text = commandHistory + cliPromptText + currentCommand; // now render the text to the command line (only need to if input changed, so
                
                Canvas.ForceUpdateCanvases();
            }
            

        }
		
	}

    public static CommandLineInterface getInstance () {
        if (instance == null) {
            GameObject obj = new GameObject("CommandLine");
            instance = obj.AddComponent<CommandLineInterface>();
        }
        return instance;
    }

    public void print(string s) {
        commandLineText.text += s;
    }
    public void println(string s) {
        commandLineText.text += s + "\n";
    }

    /**
     * This function drops the console down from the top of the screen and intercepts input from the keyboard to enter into the console
     */
    public void OpenCommandLine() {
        gameObject.SetActive(true);
    }
    /**
     * This function closes the console and stops it from intercepting input anymore
     */
    public void CloseCommandLine() {
        gameObject.SetActive(false);
    }

    /**
     * Get input from user
     */
    private void getInput () {
        input = Input.inputString;
        input = input.Replace("\r", "\n"); // windows workaround
    }
}
