namespace BinaryTreeTest
{
    using System.Diagnostics;

    /// <summary>
    /// A node, Holds some data and points to other nodes to it's left and right
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Node
    {
        /// <summary>
        /// The ID number of this node
        /// </summary>
        public int NodeID { get; set; }

        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }


        private string DebuggerDisplay()
        {
            var leftNodeString = LeftNode != null ? LeftNode.NodeID.ToString() : "null";
            var rightNodeString = RightNode != null ? RightNode.NodeID.ToString() : "null";

            return $"ID: {NodeID}, L: {leftNodeString}, R: {rightNodeString}";
        }

    };
};