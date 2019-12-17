namespace BinaryTreeTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;


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

        private class NodeList
        {
            public List<Node> Nodes { get; set; } = new List<Node>();
            public Node HeadNode { get; set; }



            public void AddNode(Node node)
            {
                if(HeadNode is null)
                {
                    HeadNode = node;
                    return;
                }

                _AddNode(HeadNode, node);
            }

            public void _AddNode(Node parentNode, Node nodeToAdd)
            {
                // If nodeToAdd ID is smaller than parent ID
                if(nodeToAdd.NodeID < parentNode.NodeID)
                {
                    // If parent already has a value to it's left
                    if(parentNode.LeftNode != null)
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
        };

        private NodeList _nodeList = new NodeList();




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupTree();
        }


        private void SetupTree()
        {
            _nodeList.AddNode(new Node()
            {
                NodeID = 60,
            });

            _nodeList.AddNode(new Node()
            {
                NodeID = 45,
            });

            _nodeList.AddNode(new Node()
            {
                NodeID = 50,
            });

            _nodeList.AddNode(new Node()
            {
                NodeID = 70,
            });


            _nodeList.AddNode(new Node()
            {
                NodeID = 65,
            });

        }
    };
};