using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtractDAT3.Common.ViewModels
{
	public class ApplicationViewModel:ViewModel
	{
		public override int ComponentID => 1;

		public List<PageViewModel> Pages
		{
			get;
			set;
		}


		public PageViewModel SelectedPage
		{
			get;
			set;
		}

		public bool ForceInvalid
		{
			get;
			set;
		}


		public bool IncludeHash
		{
			get;
			set;
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
			Pages = new List<PageViewModel>();
		}

		public void LoadDirectory(string Path,int MaxItemsPerPage)
		{
			string[] files;
			FileViewModel file;
			PageViewModel page;

			LogEnter();

			Log(LogLevels.Information, $"Parsing directory {Path}");
			Pages.Clear();
			page = new PageViewModel(Logger,Pages.Count+1);
			Pages.Add(page);

			try
			{
				files = Directory.EnumerateFiles(Path, "*.wav", SearchOption.AllDirectories).ToArray();
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

		public void Analyse()
		{

			LogEnter();

			Log(LogLevels.Information, $"Analysing items");
			foreach (FileViewModel file in Files)
			{
				file.BeginAnalyse();
			}
			foreach (FileViewModel file in Files)
			{
				file.Analyse(ForceInvalid);
			}
			foreach (FileViewModel file in Files)
			{
				file.EndAnalyse(IncludeHash);
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
