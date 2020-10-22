using ExtractDAT3.Common;
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

		public ObservableCollection<PageViewModel> Pages
		{
			get;
			private set;
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


		private IEnumerable<FileViewModel> FileViewModels
		{
			get
			{
				foreach(PageViewModel page in Pages)
				{
					foreach(FileViewModel fileViewModel in page.Items)
					{
						yield return fileViewModel;
					}
				}
			}
		}

		public ApplicationViewModel(ILogger Logger):base(Logger)
		{
			Pages = new ObservableCollection<PageViewModel>();
		}

		public async Task LoadDirectoryAsync(string Path,int MaxItemsPerPage)
		{
			string[] files;
			WavFile file;
			PageViewModel page;
			FileViewModel fileViewModel;

			LogEnter();

			Log(LogLevels.Information, $"Parsing directory {Path}");
			Pages.Clear();
			page = new PageViewModel(Logger,Pages.Count+1);
			Pages.Add(page);

			try
			{
				files = await Task.Run(()=>Directory.EnumerateFiles(Path, "*.wav", SearchOption.AllDirectories).ToArray());
			}
			catch(Exception ex)
			{
				Log(ex);
				return;
			}


			Log(LogLevels.Information, $"Creating items");
			foreach (string path in files)
			{
				file = new WavFile(Logger,path);
				fileViewModel = new FileViewModel(Logger);
				fileViewModel.Initialize(file);

				page.Items.Add(fileViewModel);
				if (page.Items.Count== MaxItemsPerPage)
				{
					page = new PageViewModel(Logger, Pages.Count+1);
					Pages.Add(page);
				}
			}

			this.SelectedPage = Pages.FirstOrDefault();
		}

		public void Clear()
		{
			LogEnter();

			Pages.Clear();
			SelectedPage = null;
		}

		public async Task AnalyseAsync()
		{

			LogEnter();

			Log(LogLevels.Information, $"Analysing items");
			foreach (FileViewModel fileViewModel in FileViewModels)
			{
				await fileViewModel.AnalyseAsync(ForceInvalid);
				await fileViewModel.WriteDAT3Async(IncludeHash);
			}
		}

		public async Task SaveReportAsync(string FileName,string Separator)
		{
			LogEnter();

			Log(LogLevels.Information, $"Saving report");

			try
			{
				using(FileStream stream=new FileStream(FileName,FileMode.Create))
				{
					StreamWriter writer = new StreamWriter(stream,Encoding.UTF8);
					await writer.WriteLineAsync(string.Join(Separator, "Path", "WAV chunk", "Data chunk", "Metadata", "DAT3","Message")); 
					foreach (FileViewModel fileViewModel in FileViewModels)
					{
						await writer.WriteLineAsync(string.Join(Separator, fileViewModel.Path,fileViewModel.WavChunkStatus,fileViewModel.DataChunkStatus,fileViewModel.MetadataStatus,fileViewModel.DAT3Status,fileViewModel.Message));
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
