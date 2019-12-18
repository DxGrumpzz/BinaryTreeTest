namespace BinaryTreeTest
{
    using System.Collections.Generic;
    using System.Diagnostics;
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
                if(nodeToFind.NodeID == currentNode.NodeID)
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
            {
                // If nodeToAdd ID is smaller than parent ID
                if (nodeToAdd.NodeID < parentNode.NodeID)
                {
                    // If parent already has a value to it's left
                    if (parentNode.LeftNode != null)
                    {
                        _AddNode(parentNode.LeftNode, nodeToAdd);
                        return;
                    };

                    parentNode.LeftNode = nodeToAdd;
                }
                // If nodeToAdd ID is bigger than parent ID
                else
                {
                    // If parent already has a value to it's left
                    if (parentNode.RightNode != null)
                    {
                        _AddNode(parentNode.RightNode, nodeToAdd);
                        return;
                    };

                    parentNode.RightNode = nodeToAdd;
                };
            }

            public List<Node> Traverse()
            {
                var nodes = new List<Node>();
                _Traverse(RootNode, nodes);
                return nodes;
            }

            private void _Traverse(Node node, List<Node> nodes)
            {
                if (node.LeftNode != null)
                    _Traverse(node.LeftNode, nodes);

                nodes.Add(node);

                if (node.RightNode != null)
                    _Traverse(node.RightNode, nodes);
            }
        };


        private class NodePosition
        {
            public Node Node { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

        };


        private Tree _tree = new Tree();




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupTree();

            DrawTree(_tree.RootNode);
        }

        private void SetupTree()
        {
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

        }




        private List<NodePosition> _drawnNodes = new List<NodePosition>();

        private const int PADDING = 15;


        private void DrawTree(Node rootNode)
        {
            _DrawTree(rootNode, 0, 0);
        }


        private void _DrawTree(Node rootNode, int currentX, int currentY)
        {
            // Check if node already exists in this coordinate
            if (_drawnNodes.Exists(coord => coord.X == currentX && coord.Y == currentY))
            {
                currentX+= 2;
            };


            if (rootNode.LeftNode != null)
            {
                _DrawTree(rootNode.LeftNode, currentX - 1, currentY + 1);
            };

            DrawNode(rootNode, currentX, currentY);


            if (rootNode.RightNode != null)
            {
                _DrawTree(rootNode.RightNode, currentX + 1, currentY + 1);
            };
        }


        private void DrawNode(Node node, int x, int y)
        {
            var textBlock = new TextBlock(new Run(node.NodeID.ToString()));

            //Canvas.SetLeft(textBlock, (canvasMiddle - textBlock.ActualWidth) * 1);
            Canvas.SetLeft(textBlock, (PADDING * x));// - textBlock.ActualWidth) ;
            Canvas.SetTop(textBlock, PADDING * y);


            MainCanvas.Children.Add(textBlock);


            _drawnNodes.Add(new NodePosition()
            {
                Node = node,

                X = x,
                Y = y,
            });

        }

    };
};