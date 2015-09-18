using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppBar;
using Microsoft.Win32;
using System.IO;

namespace SnippetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
		  InitializeComponent();
		}


		private List<Item> dataItems = new List<Item>();
		private void SnippetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object payload = dataItems[SnippetsList.SelectedIndex].FullText;

			DataObject dataObject = new DataObject();
			dataObject.SetData(payload);

			Clipboard.SetDataObject(dataObject);
		}

		private void SnippetsList_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Size dragSize = new Size(20, 20);

			dragStartPoint = e.GetPosition(SnippetsList);
			draggingElement = sender as FrameworkElement;
		}

		private Point dragStartPoint = new Point(0, 0);
		private FrameworkElement draggingElement;

		private void SnippetsList_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				e.Handled = true;
				FrameworkElement currentElement = sender as FrameworkElement;
				if (draggingElement != currentElement) return;


				// If the mouse moves outside the rectangle, start the drag.
				if (!(dragStartPoint.X == 0) || !(dragStartPoint.Y == 0))
				{
					Point currentPoint = e.GetPosition(SnippetsList);
					if ((Math.Abs(dragStartPoint.X - currentPoint.X) > 2) && (Math.Abs(dragStartPoint.Y - currentPoint.Y) > 2))
					{						
						if (SnippetsList.SelectedIndex == -1)
							return;

						object payload = dataItems[SnippetsList.SelectedIndex].FullText;

						DataObject dataObject = new DataObject();
						dataObject.SetData(payload);

						Clipboard.SetDataObject(dataObject);

						DragDropEffects dropEffect = DragDrop.DoDragDrop(draggingElement, dataObject, DragDropEffects.Copy | DragDropEffects.Move);
						if (dropEffect != DragDropEffects.None)
						{
							if (SnippetsList.SelectedIndex < SnippetsList.Items.Count - 1)
							{
								SnippetsList.SelectedIndex++;
							}
						}						
					}
				}
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{

			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = false;
			bool? result = fileDialog.ShowDialog();
			if (result == true)
			{
				LoadFile(fileDialog.FileName);
			}

			AppBarFunctions.SetAppBar(this, ABEdge.Left, grid);
		}

		public void LoadFile(string filename)
		{

			StreamReader reader;
			try
			{
				reader = File.OpenText(filename);

			}
			catch (IOException exception)
			{
				MessageBox.Show(filename + " is not a valid file");
				return;
			}
			Item item = null;
			int itemLines = 0;
			int snippetNum = 1;
			for (;;)
			{
				string line = reader.ReadLine();
				if (line == null)
					break;

				if (line.StartsWith("--------------------------"))
				{
					snippetNum = 1;
					item.ShortText = line.Trim();
					item.FullText = line.Trim() + "\n";
					itemLines++;
				}
				else if (line.StartsWith("///////////////////////////////"))
				{
					if (item != null)
					{
						SnippetsList.Items.Add(item.ShortText);
						dataItems.Add(item);
					}
					item = new Item();
					itemLines = 0;
				}
				else if (line.StartsWith("partial line:"))
				{
					string text = line.Substring("partial line:".Length);
					item.FullText = ""; // blow away the header comment
					item.FullText = text; // no trailing "\n"
					itemLines++;
				}
				else
				{
					if (itemLines == 0)
					{
						item.ShortText = snippetNum.ToString() + ".  ";
						snippetNum++;
						if ((line.Length > 2) && line.Substring(0, 2).Equals("//"))
						{
							item.ShortText += line.Substring(2).Trim();
						}
						else
						{

							item.ShortText += line.Trim();
						}
					}
					else
					{
						item.FullText += line + "\n";
					}

					itemLines++;
				}
			}

			if (item != null)
			{
				SnippetsList.Items.Add(item.ShortText);
				dataItems.Add(item);
			}

			reader.Close();
		}
	}
}