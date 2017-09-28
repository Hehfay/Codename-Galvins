using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateItems : EditorWindow {
    [MenuItem ("Window/CreatePrefabFromDataSheet")]

    public static void ShowWindow () {
        EditorWindow.GetWindow (typeof(CreateItems));
    }

    void OnGUI () {
        if (GUILayout.Button("Create Items")) {

            DataSheet[] dataSheets = Resources.LoadAll<DataSheet> ("DataSheets");

            Debug.Assert (dataSheets.Length != 0);

            for (int i = 0; i < dataSheets.Length; ++i) {


                GameObject newPrefab = new GameObject ();

                RectTransform rectTransform = newPrefab.AddComponent<RectTransform> ();
                rectTransform.localScale = new Vector3 (1, 1, 1);

                MeshFilter meshFilter = newPrefab.AddComponent<MeshFilter> ();
                meshFilter.mesh = Resources.Load<GameObject> ("MeshFilters/" + dataSheets[i].MeshFilterName).GetComponent<MeshFilter> ().sharedMesh;

                // Based on the data in the sheet, create a prefab
                BoxCollider boxCollider = newPrefab.AddComponent<BoxCollider> ();
                boxCollider.isTrigger = true;
                boxCollider.material = null;
                boxCollider.center = new Vector3 (0, 0, 0);
                boxCollider.size = new Vector3 (3, 1, 3);

                MeshRenderer meshRenderer = newPrefab.AddComponent<MeshRenderer> ();
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                meshRenderer.receiveShadows = true;
                meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;

                meshRenderer.material = Resources.Load ("Materials/" + dataSheets[i].MaterialName) as Material;

                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
                meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
                meshRenderer.probeAnchor = null;

                DataSheetWrapper dataSheetWrapper = newPrefab.AddComponent<DataSheetWrapper> ();
                dataSheetWrapper.dataSheet = dataSheets[i];

                Pickup pickup = newPrefab.AddComponent<Pickup> ();

                Rigidbody rigidBody = newPrefab.AddComponent<Rigidbody> ();
                rigidBody.mass = 1;
                rigidBody.drag = 0;
                rigidBody.angularDrag = 0.05f;
                rigidBody.useGravity = false;
                rigidBody.isKinematic = false;
                rigidBody.interpolation = RigidbodyInterpolation.None;
                rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

                newPrefab.name = dataSheets[i].name;
                newPrefab.tag = "Pickup";

                Object obj = PrefabUtility.CreateEmptyPrefab ("Assets/CustomAssets/Resources/Items/" + dataSheets[i].name + ".prefab");
                PrefabUtility.ReplacePrefab (newPrefab, obj);

                DestroyImmediate (newPrefab);
            }
            Debug.Log ("Done creating prefabs.");
        }
    }
}