using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command {

    private string sourceString;

    public Command(string sourceString) {
        this.sourceString = sourceString;
    }

    public string getSourceString() {
        return sourceString;
    }
}
