using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Events
{
    /// <summary>
    /// Игровое событие, передающее список идентификаторов (List<Guid>).
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Guid List Event")]
    public class GuidListEvent : BaseGameEvent<List<Guid>>
    {
    }
}
