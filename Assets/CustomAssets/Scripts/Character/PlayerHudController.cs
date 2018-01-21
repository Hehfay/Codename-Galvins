using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHudController : MonoBehaviour {

    public bool timeStopsWhenCLIActivated;
    public bool isCliActivated;
    public bool backtickInput;

    private GameInterpreter gameInterpreter;

	// Use this for initialization
	void Start () {
        isCliActivated = false;
        gameInterpreter = GameInterpreter.getInstance();
    }
	
	// Update is called once per frame
	void Update () {
        getInput();
        if (!isCliActivated && backtickInput) {
            isCliActivated = true;
            if (timeStopsWhenCLIActivated == true) { // since this option is checked, set time to stop
                Command command = new Command("time.stop");
                gameInterpreter.enqueueCommand(command);
            }
            CommandLineInterface cli = CommandLineInterface.getInstance();
            cli.OpenCommandLine();

            // now turn off all controls for player movement controller
            PlayerMovementController playerMovementController;
        } else if (isCliActivated && backtickInput) {
            isCliActivated = false;
            CommandLineInterface cli = CommandLineInterface.getInstance();
            cli.CloseCommandLine();
            // resume game time
            Command command = new Command("time.resume");
            gameInterpreter.enqueueCommand(command);
        }
	}

  /**
  * This function gets the player input for this controller
  */
    void getInput() {
        backtickInput = Input.GetButtonDown("Backtick"); // is user trying to jump
    }

}
