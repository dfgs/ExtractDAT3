using ExtractDAT3.Common;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtractDAT3GUI.ViewModels
{
	public class FileViewModel : ViewModel
	{
		public override int ComponentID => 4;

		public WavFile File
		{
			get;
			private set;
		}


		public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(FileViewModel));
		public string Path
		{
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}


		public static readonly DependencyProperty MetadataProperty = DependencyProperty.Register("Metadata", typeof(string), typeof(FileViewModel));
		public string Metadata
		{
			get { return (string)GetValue(MetadataProperty); }
			set { SetValue(MetadataProperty, value); }
		}



		public static readonly DependencyProperty WavChunkStatusProperty = DependencyProperty.Register("WavChunkStatus", typeof(Statuses), typeof(FileViewModel));
		public Statuses WavChunkStatus
		{
			get { return (Statuses)GetValue(WavChunkStatusProperty); }
			set { SetValue(WavChunkStatusProperty, value); }
		}



		public static readonly DependencyProperty DataChunkStatusProperty = DependencyProperty.Register("DataChunkStatus", typeof(Statuses), typeof(FileViewModel));
		public Statuses DataChunkStatus
		{
			get { return (Statuses)GetValue(DataChunkStatusProperty); }
			set { SetValue(DataChunkStatusProperty, value); }
		}



		public static readonly DependencyProperty MetadataStatusProperty = DependencyProperty.Register("MetadataStatus", typeof(Statuses), typeof(FileViewModel));
		public Statuses MetadataStatus
		{
			get { return (Statuses)GetValue(MetadataStatusProperty); }
			set { SetValue(MetadataStatusProperty, value); }
		}



		public static readonly DependencyProperty DAT3StatusProperty = DependencyProperty.Register("DAT3Status", typeof(Statuses), typeof(FileViewModel));
		public Statuses DAT3Status
		{
			get { return (Statuses)GetValue(DAT3StatusProperty); }
			set { SetValue(DAT3StatusProperty, value); }
		}



		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(FileViewModel));
		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}


		public FileViewModel(ILogger Logger):base(Logger)
		{

		}

		public void Initialize(WavFile File)
		{
			this.File = File;
			this.Path = File.Path;
		}

		public async Task AnalyseAsync(bool ForceInvalid)
		{
			await Task.Run(()=>File.Analyse(ForceInvalid));
			WavChunkStatus = File.WavChunkStatus;
			DataChunkStatus = File.DataChunkStatus;
			MetadataStatus = File.MetadataStatus;
			Metadata = File.Metadata;
			DAT3Status = File.DAT3Status;
			Message = File.Message;
		}

		public async Task WriteDAT3Async(bool IncludeHash)
		{
			await Task.Run(() => File.WriteDAT3(IncludeHash));
			DAT3Status = File.DAT3Status;
			Message = File.Message;
		}

	}
}
