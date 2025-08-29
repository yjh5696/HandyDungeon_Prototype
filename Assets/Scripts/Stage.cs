using System.Collections.Generic;
using UnityEngine;

public static class Stage
{
    public static readonly EventType[][] Chapters =
    {
        new[] {EventType.MainStory, EventType.SubStory, EventType.Battle, EventType.SubStory, EventType.Boss},
    };
}
