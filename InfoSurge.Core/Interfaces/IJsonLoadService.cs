using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
	public interface IJsonLoadService
	{
		List<(string, string)> LoadValues(string fileName);
	}
}
