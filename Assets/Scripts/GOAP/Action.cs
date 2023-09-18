using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string name { get; private set; }

    public WorldState Effects { get; private set; }
    public WorldState Prerequisites { get; private set; }


}
