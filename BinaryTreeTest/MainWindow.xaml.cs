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
        


        private Tree _tree = new Tree();

        /// <summary>
        /// A list of node with coordinate positions
        /// </summary>
        private List<NodeItemViewModel> _nodePositions = new List<NodeItemViewModel>();

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

            
            // Because Knuth's binary tree algorithm doesn't start drawing with the root node the root's X is set after all the left-hand branches are drawn.
            // So I translate back to center (horizontally)
            
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
            _nodePositions.Add(new NodeItemViewModel()
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
        private void DrawTree(List<NodeItemViewModel> nodePositions)
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

            const int SIZE = 20;

            int[] numbers = new int[SIZE];

            for (int a = 0; a < SIZE; a++)
            {
                int number = rng.Next(0, SIZE+1);

                while (numbers.Contains(number) == true)
                    number = rng.Next(0, SIZE+1);

                numbers[a] = number;

                _tree.AddNode(new Node()
                {
                    NodeID = numbers[a],
                });

            };
        }
    };
};