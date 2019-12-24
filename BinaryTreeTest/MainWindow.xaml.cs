namespace BinaryTreeTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// A node, Holds some data and points to other nodes to it's left and right
        /// </summary>
        [DebuggerDisplay("{DebuggerDisplay(), nq}")]
        private class Node
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

      
        /// <summary>
        /// A Tree data structre, holds a "list" of nodes
        /// </summary>
        private class Tree
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

    
        /// <summary>
        /// Holds a node with it's position relative to canvas
        /// </summary>
        [DebuggerDisplay("{DebuggerDisplay(), nq}")]
        private class NodePosition
        {
            public Node Node { get; set; }

            public int X { get; set; }
            public int Y { get; set; }


            private string DebuggerDisplay()
            {
                return $"ID: {Node.NodeID}, X: {X}, Y: {Y}";
            }

        };



        private Tree _tree = new Tree();

        /// <summary>
        /// A list of node with coordinate positions
        /// </summary>
        private List<NodePosition> _nodePositions = new List<NodePosition>();

        /// <summary>
        /// How much space to put between nodes
        /// </summary>
        private const int NODE_PADDING = 20;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize tree
            SetupTree();

            // Get node positions
            GetNodePositions(_tree.RootNode);

            
            // Because Knuth's binary tree algorithm doesn't start drawing with the root what is, is
            // The root's X is set after all the left-hand branches are draw 
            // so I translate back to center (horizontally)
            
            // Find root coordinates
            var rootDrawnNode = _nodePositions.FirstOrDefault(nodePosition => nodePosition.Node == _tree.RootNode).X;

            // Translate every node's X back by rootDrawnNode
            _nodePositions.ForEach(nodePosition =>
            {
                nodePosition.X -= rootDrawnNode;
            });

            // Draws the tree based on node positions
            DrawTree(_nodePositions);

            // Draws the lines that "connect" the nodes
            DrawLines(_tree.RootNode);
        }

        /// <summary>
        /// Draws a series of lines that show the connections between the nodes
        /// </summary>
        /// <param name="rootNode"></param>
        private void DrawLines(Node rootNode)
        {
            DrawLines(rootNode, 0, 0, 0, 0);
        }

        /// <summary>
        /// Draws a series of nodes by taking a node and passing its and it's neighbour X and Y coordinate to itself
        /// </summary>
        /// <param name="node"></param>
        /// <param name="x1"> The line's starting X coordinate </param>
        /// <param name="y1"> The line's starting Y coordinate </param>
        /// <param name="x2"> The line's ending X coordinate</param>
        /// <param name="y2"> The line's ending Y coordinate</param>
        private void DrawLines(Node node, int x1, int y1, int x2, int y2)
        {
            // Find current node's position
            var currentNodePosition = _nodePositions.FirstOrDefault(nodePosition => nodePosition.Node == node);

            // If the node on the left exists
            if (node.LeftNode != null)
            {
                // Find it's position
                var leftNodePosition = _nodePositions.FirstOrDefault(nodePosition => nodePosition.Node == node.LeftNode);
                
                DrawLines(node.LeftNode, currentNodePosition.X, currentNodePosition.Y, leftNodePosition.X, leftNodePosition.Y);
            };

            // Draw the line between the nodes
            DrawLine(x1, y1, x2, y2);


            // If the node on the right exists
            if (node.RightNode != null)
            {
                // Find it's position
                var rightNodePosition = _nodePositions.FirstOrDefault(nodePosition => nodePosition.Node == node.RightNode);

                DrawLines(node.RightNode, currentNodePosition.X, currentNodePosition.Y, rightNodePosition.X, rightNodePosition.Y);
            };
        }


        /// <summary>
        /// Prepares a list of <see cref="NodePosition"/> 
        /// </summary>
        /// <param name="rootNode"> The root node of the tree </param>
        private void GetNodePositions(Node rootNode)
        {
            int x = 0;
            GetNodePositions(rootNode, ref x, 0);
        }


        /// <summary>
        /// Takes the root node, and passes <paramref name="x"/> and <paramref name="y"/> to itself
        /// </summary>
        /// <remarks>
        /// Using D. Knuth's algorithm example from  https://llimllib.github.io/pymag-trees/
        /// </remarks>
        /// <param name="node"> The Current node </param>
        /// <param name="x"> Assigned node position X </param>
        /// <param name="y"> Assigned node position Y </param>
        private void GetNodePositions(Node node, ref int x, int y)
        {
            // If left node isn't null
            if (node.LeftNode != null)
                // Increment Y and pass X without modifiying it yet
                GetNodePositions(node.LeftNode, ref x, y + 1);


            // Left node is null, This is the end of the branch.
            // Draw node and increment X without modyfing Y
            _nodePositions.Add(new NodePosition()
            {
                Node = node,

                X = x++,
                Y = y,
            });


            // If right node isn't null
            if (node.RightNode != null)
                // Increment Y and pass X without modifiying it yet
                GetNodePositions(node.RightNode, ref x, y + 1);
        }


        /// <summary>
        /// Takes a list of NodePosition and draws each node accordingly
        /// </summary>
        /// <param name="nodePositions"></param>
        private void DrawTree(List<NodePosition> nodePositions)
        {
            nodePositions.ForEach(nodePosition =>
            {
                DrawNode(nodePosition.Node, nodePosition.X, nodePosition.Y);
            });
        }


        /// <summary>
        /// Draws a single node 
        /// </summary>
        /// <param name="node"> The node to draw </param>
        /// <param name="x"> The node's position in the X coordinate</param>
        /// <param name="y"> The node's position in the Y coordinate</param>
        private void DrawNode(Node node, int x, int y)
        {
            // The circle around the node
            var border = new Border()
            {
                Height = 20,
                Width = 20,

                Background = Brushes.White,

                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1d),

                ToolTip = $"{x},{y}",

                CornerRadius = new CornerRadius(20d),
            };

            // The text inside
            var textBlock = new TextBlock()
            {
                Text = node.NodeID.ToString(),
                TextAlignment = TextAlignment.Center,

                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set the Border's child
            border.Child = textBlock;

            // Set node's X and Y position inside the canvas
            Canvas.SetLeft(border, (NODE_PADDING * x));
            Canvas.SetTop(border, (NODE_PADDING * y));

            // Set boder Z position to 1 to draw over the lines
            Panel.SetZIndex(border, 1);

            // Add the "node" to the canvas
            MainCanvas.Children.Add(border);
        }
        
        /// <summary>
        /// Draws a line between 2 nodes
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private void DrawLine(int x1, int y1, int x2, int y2)
        {
            // Setup the line 
            var line = new Line()
            {
                X1 = (NODE_PADDING * x1) + NODE_PADDING / 3,
                Y1 = (NODE_PADDING * y1) + NODE_PADDING / 2,
                X2 = (NODE_PADDING * x2) + NODE_PADDING / 3,
                Y2 = (NODE_PADDING * y2) + NODE_PADDING / 2,

                Stroke = Brushes.Black,
                StrokeThickness = 1d,
            };

            // Add line to canvas
            MainCanvas.Children.Add(line);
        }


        private void SetupTree()
        {
           var rng = new Random();

            const int NUMBER = 20;

            List<int> numbers = new List<int>();

            for (int i = 0; i < NUMBER; i++)
            {
                int number = rng.Next(0, NUMBER);

                while (numbers.Contains(number) == true)
                    number = rng.Next(0, NUMBER);

                numbers.Add(number);

                _tree.AddNode(new Node()
                {
                    NodeID = numbers[i],
                });

            };
        }

    };
};