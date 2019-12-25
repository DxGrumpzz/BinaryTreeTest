namespace BinaryTreeTest
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A Tree data structre, holds a "list" of nodes
    /// </summary>
    public class Tree
    {
        public List<Node> Nodes => Traverse();

        /// <summary>
        /// Root node, The beggining of this tree 
        /// </summary>
        public Node RootNode { get; set; }


        /// <summary>
        /// Adds a single node
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            // If root node is empty 
            if (RootNode is null)
            {
                // Set root node 
                RootNode = node;
                return;
            };

            AddNode(RootNode, node);
        }

        public List<Node> Traverse()
        {
            var nodes = new List<Node>();
            Traverse(RootNode, nodes);
            return nodes;
        }

        /// <summary>
        /// Finds the first ancestor between 2 nodes
        /// </summary>
        /// <param name="firstNode"></param>
        /// <param name="secondNode"></param>
        /// <returns></returns>
        public Node FindFirstCommonAncestor(Node firstNode, Node secondNode)
        {
            // Traverse through the tree and get the routes that lead to the nodes
            var path1 = GetNodePath(firstNode);
            var path2 = GetNodePath(secondNode);

            // Get intersections between routes
            var intersections = path1.Intersect(path2);

            // If there are more than 1 intersections
            if (intersections.Count() > 0)
            {
                // Return the last node 
                return intersections.Last();
            };

            // Return the first node 
            return path1.FirstOrDefault();
        }

        /// <summary>
        /// Gets a list path that contains node that lead to <paramref name="node"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Node> GetNodePath(Node node)
        {
            // Hold node path
            var nodePath = new List<Node>();

            // Find path
            GetNodePath(node, RootNode, nodePath);

            return nodePath;
        }


        private void GetNodePath(Node nodeToFind, Node currentNode, List<Node> path)
        {
            // Check if currentNode is nodeToFind
            if (nodeToFind.NodeID == currentNode.NodeID)
            {
                // Add it to the node path list
                path.Add(currentNode);
                return;
            };

            // Check if nodeToFind is bigger than currentNode
            if (nodeToFind.NodeID > currentNode.NodeID)
            {
                // Add to node path
                path.Add(currentNode);

                // Continue to node on the right
                GetNodePath(nodeToFind, currentNode.RightNode, path);
            }
            // Check if nodeToFind is smaller than currentNode
            else if (nodeToFind.NodeID < currentNode.NodeID)
            {
                // Add to node path
                path.Add(currentNode);

                // Continue to node on the left
                GetNodePath(nodeToFind, currentNode.LeftNode, path);
            };
        }

        private void Traverse(Node node, List<Node> nodes)
        {
            if (node.LeftNode != null)
                Traverse(node.LeftNode, nodes);

            nodes.Add(node);

            if (node.RightNode != null)
                Traverse(node.RightNode, nodes);
        }

        private void AddNode(Node parentNode, Node nodeToAdd)
        {
            // If nodeToAdd ID is smaller than parent ID
            if (nodeToAdd.NodeID < parentNode.NodeID)
            {
                // If parent already has a value to it's left
                if (parentNode.LeftNode != null)
                {
                    AddNode(parentNode.LeftNode, nodeToAdd);
                    return;
                };

                parentNode.LeftNode = nodeToAdd;
            }
            // If nodeToAdd ID is bigger than parent ID
            else if (nodeToAdd.NodeID > parentNode.NodeID)
            {
                // If parent already has a value to it's left
                if (parentNode.RightNode != null)
                {
                    AddNode(parentNode.RightNode, nodeToAdd);
                    return;
                };

                parentNode.RightNode = nodeToAdd;
            };
        }

    };
};