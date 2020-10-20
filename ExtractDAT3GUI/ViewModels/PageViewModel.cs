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


		public static readonly DependencyProperty IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(PageViewModel));
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<FileViewModel>), typeof(PageViewModel));
		public ObservableCollection<FileViewModel> Items
		{
			get { return (ObservableCollection<FileViewModel>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public PageViewModel(ILogger Logger,int Index):base(Logger)
		{
			Items = new ObservableCollection<FileViewModel>();
			this.Index = Index;
		}

	}
}
