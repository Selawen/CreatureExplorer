using System;

[Flags]
public enum Condition
{
    IsHungry = 1 << 0,
    IsSleepy = 1 << 1,
    IsFrightened = 1 << 2,
    IsAnnoyed = 1 << 3,
    SeesFood = 1 << 4,
    IsNearFood = 1 << 5,
    SeesTree = 1 << 6,
    IsNearDanger = 1 << 7
}
