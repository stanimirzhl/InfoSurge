using InfoSurge.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
	public class JsonLoadService : IJsonLoadService
	{
		private readonly IWebHostEnvironment env;

		public JsonLoadService(IWebHostEnvironment env)
		{
			this.env = env;
		}

		public List<(string, string)> LoadValues(string fileName)
		{

			string fullPath = Path.Combine(env.ContentRootPath, fileName);

			var template = new[] { new { DisplayName = "", ApiValue = "" } };

			List<(string, string)> source;

			using (StreamReader r = new StreamReader(fullPath))
			{
				string json = r.ReadToEnd();
				var dezList = JsonConvert.DeserializeAnonymousType(json, template);

				source = dezList.Select(x => (x.DisplayName, x.ApiValue)).ToList();
			}

			return source;
		}
	}
}
