using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNet.Hosting;
using System.Net.Http.Headers;
using System.Text;

namespace Team.Models
{
	public class SurventrixService : ISurventrixService
	{		
	    private readonly IHostingEnvironment _env;
		private readonly string _userName = "";
		private readonly string _password = "";
		
		
		public SurventrixService(IHostingEnvironment env)
		{
			_env = env;
		}
		
		public async Task<IList<Organization>> GetOrganisations()
		{
			if(_env.IsDevelopment())
			{
				return Fake.GetOrgs();
			}
			
			List<Organization> list = null;
			using(var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://somesite.com/api/");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
					UTF8Encoding.UTF8.GetBytes(_userName + ':' + _password)));
				
				var result = await client.GetAsync("orgs");
			
				Console.WriteLine("api_callout to /api/orgs and returned {0} {1}", result.StatusCode,
					result.IsSuccessStatusCode);
							
				if(!result.IsSuccessStatusCode)
				{				
					throw new Exception("Error returned from api_callout");
				}
				
				var item = await result.Content.ReadAsStringAsync();
				list = JsonConvert.DeserializeObject<List<Organization>>(item);
			}
			return list;	
		}
		
		public async Task<IList<User>> GetUsers()
		{
			if(_env.IsDevelopment())
			{
				return Fake.GetUsers();
			}
			
			List<User> list = null;
			using(var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://somesite.com/api/");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
					UTF8Encoding.UTF8.GetBytes(_userName + ':' + _password)));
				
				var result = await client.GetAsync("users");
			
				Console.WriteLine("api_callout to /api/users and returned {0} {1}", result.StatusCode,
					result.IsSuccessStatusCode);
							
				if(!result.IsSuccessStatusCode)
				{				
					throw new Exception("Error returned from api_callout");
				}
				
				var item = await result.Content.ReadAsStringAsync();
				list = JsonConvert.DeserializeObject<List<User>>(item);
			}
			return list;	
		}
		
		private static class Fake
		{
			public static IList<Organization> GetOrgs()
			{
				return new List<Organization>
				{
					new Organization { OrganizationId = 1, Name = "Big bank", Identifier = "bigbank", Status = "Active"   },
					new Organization { OrganizationId = 2, Name = "Small bank", Identifier = "smallbank", Status = "Active" },
					new Organization { OrganizationId = 3, Name = "Short bank", Identifier = "shortbank", Status = "Active"  },
					new Organization { OrganizationId = 4, Name = "K bank", Identifier = "kbank", Status = "Inactive"  },
					new Organization { OrganizationId = 5, Name = "T bank", Identifier = "tbank", Status = "Passive"  },	
				};
			}
			
		    public static IList<User> GetUsers()
			{
			    return new List<User>
				{
					new User { UserId = 1, Name = "Stan", Identifier = "stan@gmail.com", Email = "stan@gmail.com", IsSurveyor = true  },
					new User { UserId = 2, Name = "Khan", Identifier = "khan@gmail.com", Email = "khan@gmail.com",IsSurveyor = false  },
					new User { UserId = 3, Name = "Steve", Identifier = "steve@gmail.com", Email = "steve@gmail.com", IsSurveyor = true  },
					new User { UserId = 4, Name = "Clive", Identifier = "clive@gmail.com", Email = "clive@gmail.com", IsSurveyor = false  },
					new User { UserId = 5, Name = "Mac", Identifier = "mac@gmail.com", Email = "mac@gmail.com", IsSurveyor = false  },	
				};
			}
		}
	}
}