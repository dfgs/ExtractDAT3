using ExtractDAT3.Common;
using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtractDAT3GUI.ViewModels
{
	public class PageViewModel : ViewModel
	{
		public override int ComponentID => 3;


		public int Index
		{
			get;
			private set;
		}

		public ObservableCollection<FileViewModel> Items
		{
			get;
			private set;
		}

		public PageViewModel(ILogger Logger,int Index):base(Logger)
		{
			Items = new ObservableCollection<FileViewModel>();
			this.Index = Index;
		}

	}
}
