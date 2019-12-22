namespace BinaryTreeTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        [DebuggerDisplay("{DebuggerDisplay(), nq}")]
        private class Node
        {
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

        private class Tree
        {
            public List<Node> Nodes => Traverse();
            public Node RootNode { get; set; }



            public void AddNode(Node node)
            {
                if (RootNode is null)
                {
                    RootNode = node;
                    return;
                }

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



        private List<NodePosition> _drawnNodes = new List<NodePosition>();

        private List<TextBlock> _drawnNodeText = new List<TextBlock>();

        private const int PADDING = 15;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupTree();

            DrawTree(_tree.RootNode);

            PadTree();
        }



        private void PadTree()
        {
            // Convert NodePositions to coordinates
            var positions = _drawnNodes
            .Select(node =>
            new
            {
                node.X,
                node.Y,
            });

            // Find overlaps
            var overlaps = positions
            // Find groups
            .GroupBy(group => group)
            // Return duplicates 
            .Where(group => group.Count() > 1)
            // Conert from group tyoe back to "positions" anonymous type
            .Select(y => y.Key)
            // Select duplicates into NodePositions list
            .Select(position =>
            {
                var nodes = _drawnNodes.Where(node => ((node.X == position.X) && (node.Y == position.Y)));

                return nodes.ToArray();
            })
            .ToList();


            //Debugger.Break();
        }



        private void DrawTree(Node rootNode)
        {
            DrawTree(rootNode, 0, 0);
        }


        private void DrawTree(Node node, int currentX, int currentY)
        {
            // Check if node already exists in this coordinate
            if (_drawnNodes.FirstOrDefault(coord => coord.X == currentX && coord.Y == currentY) is NodePosition nodePosition)
            {
                // Get ancestor of "node" and "nodePositon.Node"
                var ancestorNode = _tree.FindFirstCommonAncestor(node, nodePosition.Node);

                // Add spaces between the nodes
                PadNodes(ancestorNode, currentX, currentX);
            };

            if (node.LeftNode != null)
            {
                DrawTree(node.LeftNode, currentX - 1, currentY + 1);
            };

            DrawNode(node, currentX, currentY);


            if (node.RightNode != null)
            {
                DrawTree(node.RightNode, currentX + 1, currentY + 1);
            };
        }


        private void DrawNode(Node node, int x, int y)
        {
            var textBlock = new TextBlock()
            {
                Text = node.NodeID.ToString(),
            };

            Canvas.SetLeft(textBlock, (PADDING * x));
            Canvas.SetTop(textBlock, PADDING * y);


            MainCanvas.Children.Add(textBlock);

            _drawnNodeText.Add(textBlock);

            _drawnNodes.Add(new NodePosition()
            {
                Node = node,

                X = x,
                Y = y,
            });
        }


        /// <summary>
        /// Adds spaces between already drawn nodes
        /// </summary>
        /// <param name="rootNode"> The Startgin node </param>
        private void PadNodes(Node rootNode, int currentX, int xBefore)
        {
            PadNodesLeft(rootNode.LeftNode, currentX);

            currentX = xBefore;

            PadNodeRight(rootNode.RightNode, currentX);
        }


        private void PadNodesLeft(Node node, int currentX)
        {
            if (node is null)
                return;


            if (node.RightNode != null)
                PadNodesLeft(node.LeftNode, currentX);


            var nodeText = _drawnNodeText.FirstOrDefault(nodeText => nodeText.Text == node.NodeID.ToString());

            if (nodeText != null)
                Canvas.SetLeft(nodeText, (PADDING * (--currentX)));


            if (node.LeftNode != null)
                PadNodesLeft(node.RightNode, currentX);
        }

        private void PadNodeRight(Node node, int currentX)
        {
            if (node is null)
                return;


            if (node.LeftNode != null)
                PadNodeRight(node.RightNode, currentX);


            var nodeText = _drawnNodeText.FirstOrDefault(nodeText => nodeText.Text == node.NodeID.ToString());

            if (nodeText != null)
                Canvas.SetLeft(nodeText, (PADDING * (++currentX)));


            if (node.RightNode != null)
                PadNodeRight(node.LeftNode, currentX);
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
            };


            for (int i = 0; i < NUMBER; i++)
            {
                _tree.AddNode(new Node()
                {
                    NodeID = numbers[i],
                });

            };

            return;

            _tree.AddNode(new Node()
            {
                NodeID = 60,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 45,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 50,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 70,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 65,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 75,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 80,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 40,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 85,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 84,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 71,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 72,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 73,
            });
        }

    };
};