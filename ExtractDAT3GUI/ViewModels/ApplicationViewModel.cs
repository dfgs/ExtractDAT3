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

		public static readonly DependencyProperty PagesProperty = DependencyProperty.Register("Pages", typeof(ObservableCollection<PageViewModel>), typeof(ApplicationViewModel));
		public ObservableCollection<PageViewModel> Pages
		{
			get { return (ObservableCollection<PageViewModel>) GetValue(PagesProperty); }
			set { SetValue(PagesProperty, value); }
		}


		public static readonly DependencyProperty SelectedPageProperty = DependencyProperty.Register("SelectedPage", typeof(PageViewModel), typeof(ApplicationViewModel));
		public PageViewModel SelectedPage
		{
			get { return (PageViewModel)GetValue(SelectedPageProperty); }
			set { SetValue(SelectedPageProperty, value); }
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

		private IEnumerable<FileViewModel> Files
		{
			get
			{
				foreach(PageViewModel page in Pages)
				{
					foreach(FileViewModel file in page.Items)
					{
						yield return file;
					}
				}
			}
		}

		public ApplicationViewModel(ILogger Logger):base(Logger)
		{
			Pages = new ObservableCollection<PageViewModel>();
		}

		public async Task LoadDirectory(string Path,int MaxItemsPerPage)
		{
			string[] files;
			FileViewModel file;
			ObservableCollection<PageViewModel> items;
			PageViewModel page;

			LogEnter();

			Log(LogLevels.Information, $"Parsing directory {Path}");
			items = new ObservableCollection<PageViewModel>();
			page = new PageViewModel(Logger,items.Count+1);
			items.Add(page);

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
			foreach (string path in files)
			{
				file = new FileViewModel(Logger);
				file.Initialize(path);
				page.Items.Add(file);
				if (page.Items.Count== MaxItemsPerPage)
				{
					page = new PageViewModel(Logger, items.Count+1);
					items.Add(page);
				}
			}


			this.Pages = items;
			this.SelectedPage = Pages.FirstOrDefault();
		}

		public async Task Clear()
		{
			LogEnter();

			Pages.Clear();
			SelectedPage = null;
			await Task.Delay(0);
		}

		public async Task Analyse()
		{

			LogEnter();

			Log(LogLevels.Information, $"Analysing items");
			foreach (FileViewModel file in Files)
			{
				await file.BeginAnalyse();
			}
			foreach (FileViewModel file in Files)
			{
				await file.Analyse(ForceInvalid);
			}
			foreach (FileViewModel file in Files)
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
					foreach (FileViewModel file in Files)
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
