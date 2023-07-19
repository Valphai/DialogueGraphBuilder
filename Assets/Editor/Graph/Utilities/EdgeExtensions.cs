namespace UnityEditor.Experimental.GraphView
{
    public static class EdgeExtensions
    {
        public static Port GetOtherPort(this Edge edge, Port thisPort) => edge.input == thisPort ? edge.output : edge.input;
    }
}