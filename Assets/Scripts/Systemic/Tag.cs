using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Systemic
{
    public class Tag : MonoBehaviour
    {
        public EntityStat Stat { get; private set; }

        protected virtual void Awake()
        {
            Stat = GetComponent<EntityStat>();
        }
    }
}