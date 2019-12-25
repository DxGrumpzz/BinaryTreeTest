namespace BinaryTreeTest
{
    using System.Diagnostics;

    /// <summary>
    /// Holds a node with it's position relative to canvas
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class NodePosition
    {
        public Node Node { get; set; }

        public int X { get; set; }
        public int Y { get; set; }


        private string DebuggerDisplay()
        {
            return $"ID: {Node.NodeID}, X: {X}, Y: {Y}";
        }

    };
};