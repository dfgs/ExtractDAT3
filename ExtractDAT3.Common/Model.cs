using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtractDAT3.Common
{
	public abstract class Model 
	{
		private ILogger logger;
		protected ILogger Logger
		{
			get => logger;
		}
		public abstract int ComponentID
		{
			get;
		}

		public string ComponentName
		{
			get { return this.GetType().Name; }
		}
		public Model(ILogger Logger)
		{
			this.logger = Logger;

		}

		protected void LogEnter([CallerMemberName] string MethodName = null)
		{
			logger.LogEnter(ComponentID, ComponentName, MethodName);
		}

		protected void Log(LogLevels Level,string Message, [CallerMemberName] string MethodName = null)
		{
			logger.Log(ComponentID, ComponentName, MethodName,Level,Message);
		}
		protected void Log(Exception ex, [CallerMemberName] string MethodName = null)
		{
			logger.Log(ComponentID, ComponentName, MethodName, ex);
		}
	}

}