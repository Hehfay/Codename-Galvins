using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using MoonSharp.Interpreter;

public class MoonsharpTest : MonoBehaviour {

    static Script script;
	// Use this for initialization
	void Start () {
        script = new Script();
        RegisterUnityAssembly();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(CallbackTest());
        
	}

    private static int Mul(int a, int b) {
        return a * b;
    }

    private static double CallbackTest() {
        string scriptCode = @"
        -- defines a factorial function
        function fact ()
            local obj = GameObject.__new();
            id = obj.getInstanceID();
            return id;
        end";

        //Script script = new Script();

        //UserData.RegisterType<GameObject>();
        //script.Globals["GameObject"] = typeof(GameObject);
        

        script.DoString(scriptCode);

        DynValue res = script.Call(script.Globals["fact"], 4);

        return res.Number;
    }

    private static void RegisterType (System.Type type) {
        UserData.RegisterType(type);
    }

    private static void RegisterUnityAssembly () {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        System.Type[] types = assembly.GetTypes();
        foreach (System.Type type in types) {
            RegisterType(type);
            script.Globals[type.Name.Trim()] = type;
        }
        Assembly assembly1 = Assembly.Load("UnityEngine");
        System.Type[] types1 = assembly1.GetTypes();
        foreach (System.Type type1 in types1) {
            RegisterType(type1);
            Debug.Log(type1.Name.Trim());
            script.Globals[type1.Name.Trim()] = type1;
        }
        /*Table table = script.Globals;
        IEnumerable<DynValue> keys = table.Keys;
        foreach (DynValue d in keys) {
            Debug.Log(d.String);
        }*/
        /*Assembly assmebly = Assembly.Load("Assembly-CSharp");
        System.Type[] types = assmebly.GetTypes();
        foreach (System.Type type in types) {
            RegisterType(type);
            script.Globals[type.Name] = type;
        }*/
    }

}
