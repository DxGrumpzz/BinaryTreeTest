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


        private const int PADDING = 15;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupTree();

            GetNodePositions(_tree.RootNode);

            
            // Because Knuth's binary tree algorithm doesn't start drawing with the root what is, is
            // The root's X is set after all the left-hand branches are draw 
            // so I translate back to center (horizontally)
            
            // Find root coordinates
            var rootDrawnNode = _drawnNodes.FirstOrDefault(nodePosition => nodePosition.Node == _tree.RootNode).X;

            // Translate every node's X back by rootDrawnNode
            _drawnNodes.ForEach(nodePosition =>
            {
                nodePosition.X -= rootDrawnNode;
            });

            DrawTree(_drawnNodes);
        }


        private void GetNodePositions(Node rootNode)
        {
            GetNodePositions(rootNode, 0);
        }


        private static int x = 0;
        private void GetNodePositions(Node node, int y)
        {
            if (node.LeftNode != null)
                GetNodePositions(node.LeftNode, y + 1);


            _drawnNodes.Add(new NodePosition()
            {
                X = x++,
                Y = y,
                Node = node,
            });


            if (node.RightNode != null)
                GetNodePositions(node.RightNode, y + 1);
        }



        private void DrawTree(List<NodePosition> nodePositions)
        {
            nodePositions.ForEach(nodePosition =>
            {
                DrawNode(nodePosition.Node, nodePosition.X, nodePosition.Y);
            });
        }


        private void DrawNode(Node node, int x, int y)
        {
            var textBlock = new TextBlock()
            {
                Text = node.NodeID.ToString(),

                ToolTip = $"{x},{y}",
            };

            Canvas.SetLeft(textBlock, (PADDING * x));
            Canvas.SetTop(textBlock, PADDING * y);


            MainCanvas.Children.Add(textBlock);
        }


        private void SetupTree()
        {
            /*var rng = new Random();

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

            return;*/

            
            /*_tree.AddNode(new Node()
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

            return;*/

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
                NodeID = 70,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 46,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 75,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 47,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 80,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 48,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 76,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 85,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 49,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 77,
            });

            _tree.AddNode(new Node()
            {
                NodeID = 84,
            });
        }

    };
};