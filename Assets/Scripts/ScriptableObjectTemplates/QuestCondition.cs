using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    [TextArea(2, 5)] public string DebugDescription;
    public abstract bool Evaluate(PictureInfo pictureInfo);
}

