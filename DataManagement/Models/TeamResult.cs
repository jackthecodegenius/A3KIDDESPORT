using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class TeamResult
    {

        public int TeamResultID { get; set; }

        public int EventID { get; set; }
        public int GameID { get; set; }

        public int TeamID { get; set; }

        public int OppositeTeamID { get; set; } 

        public string Result { get; set; } = string.Empty;

        public TeamResult() 
        { 

        }
        public TeamResult(int eventHeld, int gamesPlayed, int team, int opposingTeam, string result)
        {

           EventID = eventHeld;
           GameID = gamesPlayed;
           TeamID = team;
           OppositeTeamID = opposingTeam;
           Result = result;
        }
    }
}
