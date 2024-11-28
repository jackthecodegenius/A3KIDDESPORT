using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Models
{
    public class ResultView
    {
        public int ResultViewID {  get; set; }
        public string EventHeld { get; set; } = string.Empty;

        public string GamesPlayed { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string OpposingTeam { get; set; } = string.Empty;

        public string Result { get; set; }
    }
}
