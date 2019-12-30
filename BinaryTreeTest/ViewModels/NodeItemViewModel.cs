namespace BinaryTreeTest
{
    using System.Linq;
    using System.Windows.Input;

    public class NodeItemViewModel : BaseViewModel
    {

        private bool _selected;
        private bool _markedForPath;


        public int X { get; set; }
        public int Y { get; set; }

        public Node Node { get; set; }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }


        public bool MarkedForPath
        {
            get => _markedForPath;
            set
            {
                _markedForPath = value;
                OnPropertyChanged();
            }
        }


        public ICommand NodeClickedCommand { get; }


        public NodeItemViewModel()
        {
            NodeClickedCommand = new RelayCommand(ExecuteNodeClickedCommand);
        }


        private void ExecuteNodeClickedCommand()
        {
            // Deselect MarkedForPath nodes
            DI.NodeItems.ForEach(node => node.MarkedForPath = false);

            // If node is currently selected
            if (Selected == true)
            {
                // de-select this node
                Selected = false;
            }
            else
            {
                // Count how many selected node are there
                var selectedCount = DI.NodeItems.Where(node => node.Selected == true);

                // If there are mode than 2 nodes
                if (selectedCount.Count() < 2)
                {
                    // Select this node
                    Selected = true;
                }
                else
                {
                    // Deselect previous node
                    selectedCount.LastOrDefault().Selected = false;

                    // Select this one
                    Selected = true;
                };
            };
        }
    };
};
