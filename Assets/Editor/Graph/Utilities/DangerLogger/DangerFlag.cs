using UnityEngine;

namespace Chocolate4.Dialogue.Edit.Graph.Utilities.DangerLogger
{
    internal class DangerFlag
    {
        internal string message;
        internal DangerType dangerLevel;
        internal IDangerCauser dangerCauser;

        internal DangerFlag(string message, DangerType dangerLevel, IDangerCauser dangerCauser)
        {
            this.message = message;
            this.dangerLevel = dangerLevel;
            this.dangerCauser = dangerCauser;
        }

        internal void Display()
        {
            switch (dangerLevel)
            {
                case DangerType.Log:
                    Debug.Log(message);
                    break;
                case DangerType.Warning:
                    Debug.LogWarning(message);
                    break;
                case DangerType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    break;
            }
        }
    }
}