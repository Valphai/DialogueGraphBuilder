using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger
{
    public static class DangerLogger
    {
        private static List<DangerFlag> dangerFlags = new List<DangerFlag>();
        private static List<Func<bool>> fixConditions = new List<Func<bool>>();

        public static void Clear()
        {
            dangerFlags.Clear();
            fixConditions.Clear();
        }

        public static bool IsEditorInDanger()
        {
            if (dangerFlags.IsNullOrEmpty())
            {
                fixConditions.Clear();
                return false;
            }

            if (FixErrorsAutomatically())
            {
                return false;
            }

            DangerFlag dangerFlag = dangerFlags.First();
            dangerFlag.Display();

            Debug.LogWarning("There are errors in the editor.");
            return true;
        }

        private static bool FixErrorsAutomatically()
        {
            if (fixConditions.IsNullOrEmpty())
            {
                return false;
            }

            if (dangerFlags.Count != fixConditions.Count)
            {
                return false;
            }

            bool errorsFixed = fixConditions.All(condition => condition?.Invoke() == true);
            if (!errorsFixed)
            {
                return false;
            }

            Clear();
            return true;
        }

        public static void LogDanger(string message, IDangerCauser causer = null)
        {
            DangerFlag dangerFlag = new DangerFlag(message, DangerType.Log, causer);
            dangerFlags.Add(dangerFlag);
            dangerFlag.Display();
        }

        public static void WarnDanger(string message, IDangerCauser causer = null)
        {
            DangerFlag dangerFlag = new DangerFlag(message, DangerType.Warning, causer);
            dangerFlags.Add(dangerFlag);
            dangerFlag.Display();
        }

        public static void ErrorDanger(string message, IDangerCauser causer = null)
        {
            DangerFlag dangerFlag = new DangerFlag(message, DangerType.Error, causer);
            dangerFlags.Add(dangerFlag);
            dangerFlag.Display();
        }

        public static void UnmarkDangerFlag(IDangerCauser causer)
        {
            DangerFlag existingFlag = dangerFlags.Find(flag => flag.dangerCauser == causer);
            if (existingFlag == null)
            {
                return;
            }

            dangerFlags.Remove(existingFlag);
        }

        public static void MarkNodeDangerous(BaseNode dangerousNode, Func<bool> fixCondition = null)
        {
            dangerousNode.style.backgroundColor = UIStyles.ErrorColor;

            (dangerousNode as IDangerCauser).IsMarkedDangerous = true;

            if (fixCondition != null)
            {
                fixConditions.Add(fixCondition); 
            }
        }
        
        public static void UnmarkNodeDangerous(BaseNode dangerousNode)
        {
            UnmarkDangerFlag(dangerousNode);
            dangerousNode.style.backgroundColor = Color.clear;

            (dangerousNode as IDangerCauser).IsMarkedDangerous = false;
        }
    }
}