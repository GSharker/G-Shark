using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;
using HelixToolkit.Wpf;

namespace VerbNurbsSharp.Desktop.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Point3DCollection ptCollection { get; set; }
        public MainWindow()
        {
            InitiPointCollection();
            InitializeComponent();
            this.Grids.Length = 50;
            this.Grids.Width = 50;

            this.Points.Points = ptCollection;
            this.Points.Size = 10;
            AddMesh();
        }

        private void InitiPointCollection()
        {
            ptCollection = new Point3DCollection
            {
                new Point3D(0,0,0),
                new Point3D(0,10,10),
                new Point3D(10,10,10)
            };
            
        }

        private void AddMesh()
        {
            MeshBuilder mBuilder = new MeshBuilder();
            mBuilder.AddTube(ptCollection, 0.2, 8, false);
            Mesh.MeshGeometry = mBuilder.ToMesh();
        }
    }
}
