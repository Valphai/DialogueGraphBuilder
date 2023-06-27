using Chocolate4.Dialogue.Edit.Graph.Nodes;
using Chocolate4.Edit.Graph.Utilities;
using Chocolate4.Dialogue.Runtime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger
{
    public static class DangerLogger
    {
        private static List<DangerFlag> dangerFlags = new List<DangerFlag>();

        public static bool IsEditorInDanger()
        {
            if (dangerFlags.IsNullOrEmpty())
            {
                return false;
            }

            DangerFlag dangerFlag = dangerFlags.First();
            dangerFlag.Display();

            Debug.LogWarning("Editor was not saved! Fix the errors first.");
            return true;
        }

        public static void LogDanger(string message, IDangerCauser causer = null)
        {
            dangerFlags.Add(new DangerFlag(message, DangerType.Log, causer));
        }
        
        public static void WarnDanger(string message, IDangerCauser causer = null)
        {
            dangerFlags.Add(new DangerFlag(message, DangerType.Warning, causer));
        }

        public static void ErrorDanger(string message, IDangerCauser causer = null)
        {
            dangerFlags.Add(new DangerFlag(message, DangerType.Error, causer));
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

        public static void MarkNodeDangerous(string message, BaseNode dangerousNode)
        {
            ErrorDanger(message, dangerousNode);
            dangerousNode.style.backgroundColor = UIStyles.ErrorColor;

            (dangerousNode as IDangerCauser).IsMarkedDangerous = true;
        }
        
        public static void UnmarkNodeDangerous(BaseNode dangerousNode)
        {
            UnmarkDangerFlag(dangerousNode);
            dangerousNode.style.backgroundColor = Color.clear;

            (dangerousNode as IDangerCauser).IsMarkedDangerous = false;
        }
    }
}