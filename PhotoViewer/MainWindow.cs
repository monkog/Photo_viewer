﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PhotoViewer.Properties;

namespace PhotoViewer
{
	public partial class MainWindow : Form
	{
		public List<DirectoryContent> PathList;
		public int CurrentPath = -1;

		public MainWindow()
		{
			InitializeComponent();
			CurrentImage.Visible = false;
			History.Checked = true;
			PathList = new List<DirectoryContent>();
			HistoryList.SmallImageList = new ImageList();
			HistoryList.SmallImageList.Images.Add(Resources.imageDir);
		}

		private void BrowseClick(object sender, MouseEventArgs e)
		{
			var directoryPicker = new DirectoryPicker { Owner = this };
			directoryPicker.ShowDialog();
		}

		private void HistoryCheckedChanged(object sender, EventArgs e)
		{
			var displayHistory = History.Checked;
			HistoryList.Enabled = displayHistory;
			HistoryList.Visible = displayHistory;
			TreeView.Enabled = !displayHistory;
			TreeView.Visible = !displayHistory;
		}

		private void ButtonLeftClick(object sender, MouseEventArgs e)
		{
			PathList[CurrentPath].CurrentIndex--;
			ButtonLeft.Enabled = PathList[CurrentPath].CurrentIndex > 1;
			ButtonRight.Enabled = true;

			UpdateImageAndIndex();
		}

		private void ButtonRightClick(object sender, MouseEventArgs e)
		{
			PathList[CurrentPath].CurrentIndex++;
			ButtonLeft.Enabled = true;
			ButtonRight.Enabled = PathList[CurrentPath].CurrentIndex != PathList[CurrentPath].FileCount;

			UpdateImageAndIndex();
		}

		private void UpdateImageAndIndex()
		{
			ImageIndex.Text = PathList[CurrentPath].CurrentIndex.ToString();
			var files = Directory.GetFiles(PathList[CurrentPath].Path);
			CurrentImage.ImageLocation = files[PathList[CurrentPath].CurrentIndex - 1];
		}

		private void CurrentImageDoubleClick(object sender, EventArgs e)
		{
			if (!CurrentImage.Visible) return;
			TransparentBackground image;

			if (CurrentImage.Image == CurrentImage.ErrorImage)
			{
				image = new TransparentBackground(CurrentImage.Image.Size.Width, CurrentImage.Image.Size.Height, Resources.question) { Owner = this };
			}
			else
			{
				var dirElems = Directory.GetFiles(PathList[CurrentPath].Path);
				var imagePath = dirElems[PathList[CurrentPath].CurrentIndex - 1];
				image = new TransparentBackground(CurrentImage.Image.Size.Width, CurrentImage.Image.Size.Height, imagePath) { Owner = this };
			}

			image.Show();
		}

		private void HistoryListClicked(object sender, EventArgs e)
		{
			CurrentPath = HistoryList.SelectedIndices[0];
			InitializeImageViewer();
		}

		private void TreeItemSelected(object sender, TreeViewEventArgs e)
		{
			var path = e.Node.FullPath;
			path = path.Substring(0, 3) + path.Substring(4);

			for (var i = 0; i < PathList.Count; i++)
				if (PathList[i].Path == path)
				{
					CurrentPath = i;
					break;
				}

			InitializeImageViewer();
		}

		private void InitializeImageViewer()
		{
			var hasFiles = PathList[CurrentPath].FileCount > 0;

			CurrentImage.Visible = hasFiles;
			ButtonLeft.Visible = hasFiles;
			ButtonRight.Visible = hasFiles;
			ImageIndex.Visible = hasFiles;

			if (!hasFiles) return;

			var files = Directory.GetFiles(PathList[CurrentPath].Path);
			CurrentImage.ImageLocation = files[PathList[CurrentPath].CurrentIndex - 1];
			ImageIndex.Text = PathList[CurrentPath].CurrentIndex.ToString();

			ButtonLeft.Enabled = PathList[CurrentPath].CurrentIndex != 1;
			ButtonRight.Enabled = PathList[CurrentPath].CurrentIndex != PathList[CurrentPath].FileCount;
		}
	}
}
