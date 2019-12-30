namespace BinaryTreeTest
{
    using System;
    using System.Collections.Generic;
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
        /// How much space to put between nodes
        /// </summary>
        private const int NODE_PADDING = 20;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Draws the tree based on node positions
            DrawTree(DI.NodeItems);

            // Draws the lines that "connect" the nodes
            DrawLines(DI.Tree.RootNode);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Find selected nodes
            var selectedNodes = DI.NodeItems.Where(node => node.Selected == true);

            // If there are less than 2 selected nodes
            if (selectedNodes.Count() != 2)
                // Exit method
                return;

            // Get node paths 
            var path1 = DI.Tree.GetNodePath(selectedNodes.FirstOrDefault().Node);
            var path2 = DI.Tree.GetNodePath(selectedNodes.LastOrDefault().Node);
            
            // I am getting the first common ancestor because of a limitation of this algorithm
            var ancestor = DI.Tree.FindFirstCommonAncestor(selectedNodes.FirstOrDefault().Node, selectedNodes.LastOrDefault().Node);

            // Combine the path without discarding the duplicates
            List<Node> combinedpaths = new List<Node>();
            combinedpaths.AddRange(path1);
            combinedpaths.AddRange(path2);

            // Find the correct node path by...
            var intersections = combinedpaths
            // Grouping nodes 
            .GroupBy(group => group)
            // DIscarding the groups that appear more than once
            .Where(group => group.Count() <= 1)
            // Converting the groups back to nodes
            .Select(group => group.Key)
            // Adding the anncestor
            .Append(ancestor);


            // Mark path nodes
            DI.NodeItems.Where(nodeViewModel =>
            {
                var nodes = intersections.Where(node => nodeViewModel.Node == node);

                return nodes.Count() != 0;
            })
            .ToList()
            .ForEach(node =>
            {
                node.Selected = true;
            });
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
            var currentNodePosition = DI.NodeItems.FirstOrDefault(nodePosition => nodePosition.Node == node);

            // If the node on the left exists
            if (node.LeftNode != null)
            {
                // Find it's position
                var leftNodePosition = DI.NodeItems.FirstOrDefault(nodePosition => nodePosition.Node == node.LeftNode);

                DrawLines(node.LeftNode, currentNodePosition.X, currentNodePosition.Y, leftNodePosition.X, leftNodePosition.Y);
            };

            // Draw the line between the nodes
            DrawLine(x1, y1, x2, y2);


            // If the node on the right exists
            if (node.RightNode != null)
            {
                // Find it's position
                var rightNodePosition = DI.NodeItems.FirstOrDefault(nodePosition => nodePosition.Node == node.RightNode);

                DrawLines(node.RightNode, currentNodePosition.X, currentNodePosition.Y, rightNodePosition.X, rightNodePosition.Y);
            };
        }



        /// <summary>
        /// Takes a list of NodePosition and draws each node accordingly
        /// </summary>
        /// <param name="nodePositions"></param>
        private void DrawTree(List<NodeItemViewModel> nodePositions)
        {
            nodePositions.ForEach(nodePosition =>
            {
                var nodeViewItem = new NodeItemView()
                {
                    DataContext = nodePosition,
                };


                // Set node's X and Y position inside the canvas
                Canvas.SetLeft(nodeViewItem, (NODE_PADDING * nodePosition.X));
                Canvas.SetTop(nodeViewItem, (NODE_PADDING * nodePosition.Y));

                // Set boder Z position to 1 to draw over the lines
                Panel.SetZIndex(nodeViewItem, 1);

                // Add the "node" to the canvas
                MainCanvas.Children.Add(nodeViewItem);
            });
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

    };
};