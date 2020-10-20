using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtractDAT3GUI.ViewModels
{
	public class ApplicationViewModel:ViewModel
	{
		public override int ComponentID => 1;

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<FileViewModel>), typeof(ApplicationViewModel));
		public ObservableCollection<FileViewModel> Items
		{
			get { return (ObservableCollection<FileViewModel>) GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}


		public static readonly DependencyProperty ForceInvalidProperty = DependencyProperty.Register("ForceInvalid", typeof(bool), typeof(ApplicationViewModel));
		public bool ForceInvalid
		{
			get { return (bool)GetValue(ForceInvalidProperty); }
			set { SetValue(ForceInvalidProperty, value); }
		}


		public static readonly DependencyProperty IncludeHashProperty = DependencyProperty.Register("IncludeHash", typeof(bool), typeof(ApplicationViewModel));
		public bool IncludeHash
		{
			get { return (bool)GetValue(IncludeHashProperty); }
			set { SetValue(IncludeHashProperty, value); }
		}



		public ApplicationViewModel(ILogger Logger):base(Logger)
		{
			Items = new ObservableCollection<FileViewModel>();
		}

		public async Task LoadDirectory(string Path)
		{
			string[] files;
			FileViewModel item;
			ObservableCollection<FileViewModel> items;

			LogEnter();

			Log(LogLevels.Information, $"Parsing directory {Path}");
			items = new ObservableCollection<FileViewModel>();

			try
			{
				files = await Task.Run(() => Directory.EnumerateFiles(Path, "*.wav", SearchOption.AllDirectories).ToArray());
			}
			catch(Exception ex)
			{
				Log(ex);
				return;
			}


			Log(LogLevels.Information, $"Creating items");
			foreach (string file in files)
			{
				item = new FileViewModel(Logger);
				item.Initialize(file);
				items.Add(item);
			}


			this.Items = items;
			
		}

		public async Task Clear()
		{
			LogEnter();

			Items = new ObservableCollection<FileViewModel>();
			await Task.Delay(0);
		}

		public async Task Analyse()
		{

			LogEnter();

			Log(LogLevels.Information, $"Analysing items");
			foreach (FileViewModel file in Items)
			{
				await file.BeginAnalyse();
			}
			foreach (FileViewModel file in Items)
			{
				await file.Analyse(ForceInvalid);
			}
			foreach (FileViewModel file in Items)
			{
				await file.EndAnalyse(IncludeHash);
			}
		}

		public void SaveReport(string FileName,string Separator)
		{
			LogEnter();

			Log(LogLevels.Information, $"Saving report");

			try
			{
				using(FileStream stream=new FileStream(FileName,FileMode.Create))
				{
					StreamWriter writer = new StreamWriter(stream,Encoding.UTF8);
					writer.WriteLine(string.Join(Separator, "Path", "WAV chunk", "Data chunk", "Metadata", "DAT3","Message")); 
					foreach (FileViewModel file in Items)
					{
						writer.WriteLine(string.Join(Separator, file.Path,file.WavChunkStatus,file.DataChunkStatus,file.MetadataStatus,file.DAT3Status,file.Message));
					}
					writer.Flush();
				}
				
			}
			catch(Exception ex)
			{
				Log(ex);
			}
		}



	}
}
