using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DuplicateGameObjectWindow : EditorWindow {
    private GameObject prefab;
    private int numberDuplicates;

    [MenuItem ("Create/Duplicate GameObject Window")]
	public static void ShowDuplicateGameObjectWindow () {
        DuplicateGameObjectWindow window = GetWindow<DuplicateGameObjectWindow>();
        window.title = "Duplicate Game Object";
    }

    public void OnGUI() {
        // custom gui
        prefab = (GameObject)EditorGUILayout.ObjectField("GameObject to Duplicate", prefab, typeof(GameObject), true);
        numberDuplicates = EditorGUILayout.IntField("Number of Duplicates", numberDuplicates);

        if (GUILayout.Button("Create Duplicates")) {
            for (int i = 0; i < numberDuplicates; i++) {
                GameObject dup = Instantiate(prefab);
                bool collision = true;
                while (collision) {
                    Vector3 proposedPosition = new Vector3(Random.Range(10.0f, 450.0f), 5.6f, Random.Range(10.0f, 450.0f));
                    Vector3 center = proposedPosition + dup.transform.localScale / 2;

                    Collider[] collisions = Physics.OverlapBox(center, dup.transform.localScale / 2, dup.transform.rotation);
                    if (collisions.Length == 0) {
                        dup.transform.position = proposedPosition;
                        collision = false;

                        RaycastHit hit;
                        if (Physics.Raycast(new Ray(proposedPosition, Vector3.down), out hit)) { // cast ray from current player position into ground beneath
                            dup.transform.position = new Vector3(proposedPosition.x, hit.point.y + (dup.transform.localScale.y / 2) + 1 + 0.2f, proposedPosition.z); // set the player to land on the ground beneath
                        }

                    }
                }
            }
        }
    }
}