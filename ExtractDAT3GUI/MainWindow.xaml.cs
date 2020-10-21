using ExtractDAT3.Common.ViewModels;
using LogLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtractDAT3GUI
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ILogger logger;


		public static readonly DependencyProperty IsIdleProperty = DependencyProperty.Register("IsIdle", typeof(bool), typeof(MainWindow),new PropertyMetadata(true));
		public bool IsIdle
		{
			get { return (bool)GetValue(IsIdleProperty); }
			set { SetValue(IsIdleProperty, value); }
		}


		public ApplicationViewModel ViewModel
		{
			get;
			set;
		}

		public MainWindow()
		{
			logger = new FileLogger(new DefaultLogFormatter(), global::ExtractDAT3GUI.Properties.Settings.Default.LogFileName);
			ViewModel = new ApplicationViewModel(logger);
			InitializeComponent();
			DataContext = ViewModel;
		}
		private void Window_Closed(object sender, EventArgs e)
		{
			logger = null;
		}

		private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsIdle;e.Handled = true;
		}

		private async void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			FolderBrowserDialog dialog;

			IsIdle = false;
			DataContext = null;
			dialog = new FolderBrowserDialog() { Description = "Open WAV folder" };
			dialog.SelectedPath = global::ExtractDAT3GUI.Properties.Settings.Default.DefaultFolder;
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				await Task.Run(()=>ViewModel.LoadDirectory(dialog.SelectedPath, global::ExtractDAT3GUI.Properties.Settings.Default.MaxItemsPerPage));
				DataContext = ViewModel;
			}
			IsIdle = true;
		}


		private void ClearCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsIdle && (ViewModel.Pages.Count>0); e.Handled = true;
		}

		private async void ClearCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			IsIdle = false;
			DataContext = null;
			await Task.Run(()=>ViewModel.Clear());
			DataContext = ViewModel;
			IsIdle = true;
		}



		private void GoCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsIdle && (ViewModel.Pages.Count > 0); e.Handled = true;
		}

		private async void GoCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			IsIdle = false;
			DataContext = null;
			await Task.Run(()=>ViewModel.Analyse());
			DataContext = ViewModel;
			IsIdle = true;
		}

		private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsIdle && (ViewModel.Pages.Count > 0); e.Handled = true;
		}

		private async void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			System.Windows.Forms.SaveFileDialog dialog;

			IsIdle = false;

			dialog = new System.Windows.Forms.SaveFileDialog() { Title = "Save report as" };
			dialog.FileName = "report.csv";
			dialog.Filter = "csv files|*.csv|All files|*.*";
			if (dialog.ShowDialog()== System.Windows.Forms.DialogResult.OK)
			{
				await Task.Run(()=>ViewModel.SaveReport(dialog.FileName, global::ExtractDAT3GUI.Properties.Settings.Default.CSVSeparator));

			}


			IsIdle = true;
		}

	}
}
