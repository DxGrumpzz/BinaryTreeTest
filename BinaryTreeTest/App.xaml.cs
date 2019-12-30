namespace BinaryTreeTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize tree
            SetupTree(staticTree: true);

            // Get node positions
            SetupNodeItemViewModels(DI.Tree.RootNode);

            // Root node Center fix
            TranslateNodes();

            (Current.MainWindow = new MainWindow())
            .Show();
        }


        private void TranslateNodes()
        {
            // Because Knuth's binary tree algorithm doesn't start drawing with the root node the root's X is set after all the left-hand branches are drawn.
            // So I translate back to center (horizontally)

            // Find root coordinates
            var rootDrawnNode = DI.NodeItems.FirstOrDefault(nodePosition => nodePosition.Node == DI.Tree.RootNode).X;

            // Translate every node's X back by rootDrawnNode
            DI.NodeItems.ForEach(nodePosition =>
            {
                nodePosition.X -= rootDrawnNode;
            });
        }


        /// <summary>
        /// Prepares a list of <see cref="NodePosition"/> 
        /// </summary>
        /// <param name="rootNode"> The root node of the tree </param>
        private void SetupNodeItemViewModels(Node rootNode)
        {
            DI.NodeItems = new List<NodeItemViewModel>();

            int x = 0;
            SetupNodeItemViewModels(rootNode, ref x, 0);
        }


        private void SetupTree(bool staticTree = false)
        {
            // Initialize the tree
            DI.Tree = new Tree();

            if (staticTree == true)
            {


                DI.Tree.AddNode(new Node()
                {
                    NodeID = 60,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 45,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 70,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 46,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 75,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 47,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 80,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 48,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 76
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 85,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 49
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 77,
                });
                DI.Tree.AddNode(new Node()
                {
                    NodeID = 84,
                });
            }
            else
            {
                var rng = new Random();

                const int SIZE = 20;

                int[] numbers = new int[SIZE];

                for (int a = 0; a < SIZE; a++)
                {
                    int number = rng.Next(0, SIZE + 1);

                    while (numbers.Contains(number) == true)
                        number = rng.Next(0, SIZE + 1);

                    numbers[a] = number;

                    DI.Tree.AddNode(new Node()
                    {
                        NodeID = numbers[a],
                    });

                };
            };
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
        private void SetupNodeItemViewModels(Node node, ref int x, int y)
        {
            // If left node isn't null
            if (node.LeftNode != null)
                // Increment Y and pass X without modifiying it yet
                SetupNodeItemViewModels(node.LeftNode, ref x, y + 1);


            // Left node is null, This is the end of the branch.
            // Draw node and increment X without modyfing Y
            DI.NodeItems.Add(new NodeItemViewModel()
            {
                Node = node,

                X = x++,
                Y = y,
            });


            // If right node isn't null
            if (node.RightNode != null)
                // Increment Y and pass X without modifiying it yet
                SetupNodeItemViewModels(node.RightNode, ref x, y + 1);
        }

    };
};