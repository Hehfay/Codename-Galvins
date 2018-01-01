using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Singleton class that encapsulates the Command line interface for this game
 */
public class GameInterpreter : MonoBehaviour {

    Queue<Command> commandsToParse;
    private static GameInterpreter instance;
	// Use this for initialization
	void Start () {
        gameObject.GetInstanceID();
		if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
            commandsToParse = new Queue<Command>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		while (commandsToParse.Count > 0) {
            parseCommand(commandsToParse.Dequeue());
        }
	}

    public static GameInterpreter getInstance() {
        if (instance == null) {
            GameObject obj = new GameObject();
            instance = obj.AddComponent<GameInterpreter>();
        }
        return instance;
    }

    private void parseCommand (Command command) {
        // TODO: replace with real parser; thinking about using Moonsharp (github project). It can be used with lua for real scripting, then I would only have to implement the backend
        char[] delimiters = { ' ', '\t' }; // white space separates tokens
        string[] tokens = command.getSourceString().Split(delimiters);
        if (tokens != null && tokens.Length > 0) {
            if (tokens[0].ToLower().Equals("time.stop")) {
                Time.timeScale = 0.0f;
            } else if (tokens[0].ToLower().Equals("time.resume")) {
                Time.timeScale = 1.0f;
            } else if (tokens[0].ToLower().Equals("time.scale")) {
                string val = tokens[1].ToLower();
                int num = int.Parse(val);
                Time.timeScale = num;
            }
        } else {
            Debug.Log("Error: cannot parse empty string.");
        }
    }

    public void enqueueCommand(Command command) {
        commandsToParse.Enqueue(command);
    }
}
