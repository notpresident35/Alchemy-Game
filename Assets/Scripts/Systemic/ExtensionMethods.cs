using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Systemic
{
    public static class ExtensionMethods
    {
        public static Tag[] GetTags<T>(this GameObject self) where T: Tag
        {
            return self.GetComponents<T>();
        }

        public static Tag[] GetTags<T>(this Component self) where T: Tag
        {
            return self.GetComponents<T>();
        }

        public static T AddTag<T>(this GameObject self) where T : Tag, new()
        {
            return self.AddComponent<T>();
        }

        public static T AddTag<T>(this Component self) where T : Tag, new()
        {
            return self.gameObject.AddComponent<T>();
        }
    }
}