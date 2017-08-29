using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveListener : MonoBehaviour {
    // This class looks at the active quests and determines if the action you just did
    // satisfies the objective.

    void Start () {
    }

    public void ObserveAction (Pickup interaction) {
        // TODO Have objectives complicated enough where the type matters.

        QuestManagerScript questManager = GameObject.Find ("QuestManager").GetComponent<QuestManagerScript>();
        Debug.Assert (questManager != null);

        for (int i = 0; i < questManager.QuestsIHave.Count; ++i) {
            Quest currentQuest = questManager.QuestsIHave[i];

            switch (currentQuest.currentObjective().questType) {

                case QuestType.Fetch: {

                    PickupData interactionPickupData = interaction.pickupData;
                    int count = interaction.count;

                    // If this is a fetch quest and I didn't pick something up skip.
                    if (interactionPickupData == null) continue;

                    for (int j = 0; i < currentQuest.currentObjective().FetchItems.Length; ++j) {
                        if (interactionPickupData == currentQuest.currentObjective().FetchItems[i] &&
                            count == currentQuest.currentObjective().FetchCount[i]) {

                            currentQuest.AdvanceObjective ();

                            Debug.Log (currentQuest.currentObjective().description);
                        }
                    }

                } break;

                case QuestType.Talk: {

                } break;
            }


        }
    }
}
