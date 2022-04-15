using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Systemic
{
    public class Mortal : Tag
    {
        private void Update()
        {
            if (Stat.Health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}