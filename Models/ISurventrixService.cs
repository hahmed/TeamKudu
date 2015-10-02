using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Team.Models
{
    public interface ISurventrixService
    {
        Task<IList<Organization>> GetOrganisations();
        
        Task<IList<User>> GetUsers();
    }
}