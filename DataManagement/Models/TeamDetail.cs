using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class TeamDetail
    {
        public int TeamID { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string PrimaryContact { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public int CompetitionPoints { get; set; } 

        public TeamDetail()
        {

        }

        public TeamDetail(string teamName, string primaryContact, string contactPhone, string contactEmail, int competitionPoints)
        {

            TeamName = teamName;
            PrimaryContact = primaryContact;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            CompetitionPoints = competitionPoints;
        }

        
    }
}
