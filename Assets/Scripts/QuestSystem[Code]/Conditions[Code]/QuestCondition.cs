using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    public abstract bool Evaluate(PictureInfo pictureInfo);

}

