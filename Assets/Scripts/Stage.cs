using System.Collections.Generic;
using UnityEngine;

public static class Stage
{
    public static readonly StageType[][] Chapters =
    {
        new[] {StageType.MainStory, StageType.SubStory, StageType.Battle, StageType.SubStory, StageType.MainStory, StageType.SubStory, StageType.Battle, StageType.SubStory, StageType.MainStory, StageType.Battle},
    };
}
