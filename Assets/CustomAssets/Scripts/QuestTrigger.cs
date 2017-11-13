using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class QuestTrigger : ScriptableObject {
    public QuestNode questNode;
    public Quest quest;
    public int task;
}
