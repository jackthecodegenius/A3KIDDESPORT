using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement.Models;
using DataManagement;
using System.ComponentModel;
using Dapper;



namespace DataManagement
{
    public class DBBuilder : DataAdapter
    {
        public void CreateDatabase()
        {
            //Our connection object to link to the database
            SqlConnection connection = Helper.GetSQLServerConnection("Default");

            try
            {
                //Custom connection string to only connect to the server layer of your SQL Database
                string connectionString = $"Data Source={connection.DataSource}; Integrated Security = True";
                //Query to build new Database if it does not already exist.
                string query = $"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name ='{connection.Database}') " +
                               $" CREATE DATABASE {connection.Database}";

                using (connection = new SqlConnection(connectionString))
                {
                    //A command object which will send our request to the Database <= Normally done for us by Dapper 
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //Checks if the connection is currently open, if not, it opens the connection.<= Normally done for us by Dapper 
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        //Executes an SQL Request that does not expect a response(Query) to be returned.
                        command.ExecuteNonQuery();
                        //Closes the connection to the database manually.<= Normally done for us by Dapper 
                        connection.Close();
                    }
                }

            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Runs a query against the database to get a count of how many  base tables exist in the database.
        /// </summary>
        /// <returns>A confirmation of whther there are tables (TRUE) or not (FALSE)</returns>
        public bool DoTablesExist()
        {
            //Our using statemtnqwhich builds our connection and disposes of it once finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //Quey to request the count of how many base tables are in the database structure. Base tables refers to user
                //built tables and ignores inbuild tables such as index tables and reference/settings tables.
                string query = $"SELECT COUNT(*) FROM {connection.Database}.INFORMATION_SCHEMA.TABLES " +
                               $"WHERE TABLE_TYPE = 'BASE TABLE'";
                //Sends the query to the databse and stores the returned table count.
                int count = connection.QuerySingle<int>(query);

                //If the count is above 0 return true, otherwise return false to indicate whether the database has tabes or not.
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Method to send a request to the databse to create a new databse table. This method requires the table name and column/attributes details to be pre-populated and 
        /// passed to the method for it to work.
        /// </summary>
        /// <param name="name">The name to be given to the table when created.</param>
        /// <param name="structure">A string oulining all the table column/attributes and their names, types and any other special rules for each of them such as PK 
        ///                         identification, nullability rules and foreighn key connections.</param>
        private void CreateTable(string name, string structure)
        {
            try
            {
                //Partial query to build table in database. Parameters passe to method will be inserted to complete the query string.
                string query = $"CREATE TABLE {name} ({structure})";
                //Our using statemtnqwhich builds our connection and disposes of it once finished.
                using (var connection = Helper.GetSQLServerConnection("Default"))
                {
                    //Passes the query to the databse to be perfomed.
                    connection.Execute(query);
                }
            }
            catch (Exception e)
            {
                //Log error on failure
            }

        }

        /// <summary>
        /// Runs all the separate methods to create all the database tables, ensuring to run them in the correct sequence to ensure tables with foreign key declarations are 
        /// not made until after the tables they are referencing are already created.
        /// </summary>
        public void BuildDatabaseTables()
        {
            BuildEventTable();
            BuildGameTable();
            BuildTeamDetailTable();
            BuildTeamResultTable();
        }

        /// <summary>
        /// Runs all the separate methods to populate test data into all the tables, ensuring to run them in the correct sequence to ensure tables with foreign key declarations are 
        /// not seeded until after the tables they are referencing are already completed and contain data.
        /// </summary>
        public void SeedDatabaseTables()
        {
            SeedEventsTable();
            SeedGamesTable();
            SeedTeamDetailsTable();
            SeedTeamResultsTable();
        }

        /// <summary>
        /// Outlines the table structure of the Customer table and passes it to the CreateTable() method to be built.
        /// Each column/attribute is defined using the following format: 
        ///     <Name> <DataType> <Rules>
        /// 
        /// The rules for each attribute use the following options:
        ///     IDENTITY (1,1) - Sets auto-incrementation for the column with a start number and increment matching the provided numbers
        ///     PRIMARY KEY - Marks the clumn as the primary key for the table
        ///     NULL or NOT NULL sets the nullability of the column accordingly. If not defined the NULL 
        /// 
        /// </summary>
        private void BuildEventTable()
        {
            //Stores desired table name
            string tableName = "Events";
            //Outlines structure of table
            string tableStructure = "EventID int IDENTITY (1,1) PRIMARY KEY, " +
                                    "EventName VARCHAR(50) NOT NULL, " +
                                    "EventLocation VARCHAR(50) NOT NULL, " +
                                    "EventDate DATETIME NOT NULL, ";
                                    
            //Pases name and strucutre to creation method.
            CreateTable(tableName, tableStructure);
        }

        
        private void BuildGameTable()
        {
            //Stores desired table name
            string tableName = "Games";
            //Outlines structure of table
            string tableStructure = "GameID int IDENTITY(1,1) PRIMARY KEY, " +
                                    "GameName VARCHAR(50) NOT NULL, " +
                                    "GameType VARCHAR(50) NOT NULL, ";
            //Pases name and strucutre to creation method.
            CreateTable(tableName, tableStructure);
        }

        /// <summary>
        /// Outlines the table structure of the Products table and passes it to the CreateTable() method to be built.
        /// Each column/attribute is defined using the following format: 
        ///     <Name> <DataType> <Rules>
        /// 
        /// The rules for each attribute use the following options:
        ///     IDENTITY (1,1) - Sets auto-incrementation for the column with a start number and increment matching the provided numbers
        ///     PRIMARY KEY - Marks the clumn as the primary key for the table
        ///     NULL or NOT NULL sets the nullability of the column accordingly. If not defined the NULL 
        /// 
        /// </summary>
        private void BuildTeamDetailTable()
        {
            //Stores desired table name
            string tableName = "TeamDetails";
            //Outlines structure of table
            //Also defines a foreigh key reference to the Category table using the following format for foreign key connections:
            //  FOREIGN KEY (<FK column name>) REFERENCES <Table being referenced>(<Primary Key of referecned table>)

            //Constrint rules for the foreigh key(optional) can be dfined using the following format:
            //  ON DELETE <Rule> ON CASCADE <Rule>
            //
            string tableStructure = "TeamID int IDENTITY(1,1) PRIMARY KEY, " +
                                    "TeamName VARCHAR(50) NOT NULL, " +
                                    "PrimaryContact VARCHAR(50) NOT NULL, " +
                                    "ContactPhone VARCHAR(50) NOT NULL, " +
                                    "ContactEmail VARCHAR(50) NOT NULL, " +
                                    "CompetitionPoints int NOT NULL, " ;
                                    
            //Pases name and strucutre to creation method.
            CreateTable(tableName, tableStructure);
        }

        /// <summary>
        /// Outlines the table structure of the Builds table and passes it to the CreateTable() method to be built.
        /// Each column/attribute is defined using the following format: 
        ///     <Name> <DataType> <Rules>
        /// 
        /// The rules for each attribute use the following options:
        ///     IDENTITY (1,1) - Sets auto-incrementation for the column with a start number and increment matching the provided numbers
        ///     PRIMARY KEY - Marks the clumn as the primary key for the table
        ///     NULL or NOT NULL sets the nullability of the column accordingly. If not defined the NULL 
        /// 
        /// </summary>
        private void BuildTeamResultTable()
        {
            //Stores desired table name
            string tableName = "TeamResults";
            //Outlines structure of table
            //Also defines a foreigh key reference to the Category table using the following format for foreign key connections:
            //  FOREIGN KEY (<FK column name>) REFERENCES <Table being referenced>(<Primary Key of referecned table>)

            //Constrint rules for the foreigh key(optional) can be dfined using the following format:
            //  ON DELETE <Rule> ON CASCADE <Rule>
            //
            string tableStructure = "TeamResultID int IDENTITY(1,1) PRIMARY KEY, " +
                                   "EventID int NOT NULL, " +
                                   "GameID int NOT NULL, " +
                                   "TeamID int NOT NULL, " +
                                   "OppositeTeamID int NOT NULL, " +
                                   "Result VARCHAR(50) NOT NULL, " +
                                   "FOREIGN KEY (EventID) REFERENCES Events(EventID), " +
                                   "FOREIGN KEY (GameID) REFERENCES Games(GameID), " +
                                   "FOREIGN KEY (TeamID) REFERENCES TeamDetails(TeamID), " +
                                   "FOREIGN KEY (OppositeTeamID) REFERENCES TeamDetails(TeamID) " ;
            //Pases name and strucutre to creation method.
            CreateTable(tableName, tableStructure);
        }

        /// <summary>
        /// Creates a list of test data to be added to the table and then sends each item to the database.
        /// </summary>
        private void SeedEventsTable()
        {
            //populate list of data model objects with pre-filled data
            List<Event> eventList = new List<Event>
            {
                new Event
                {
                    EventName = "Finals",
                    EventLocation = "O2 Stadium",
                    EventDate = DateTime.Now.AddDays(-30)

                },
                new Event
                {
                    EventName = "SemiFinals",
                    EventLocation = "The Outback",
                    EventDate = DateTime.Now.AddDays(-7)
                },
                new Event
                {
                    EventName = "Quarter-Finals",
                    EventLocation = "London",
                    EventDate = DateTime.Now.AddDays(-100)
                }
            };
            //Add each to the database using the relevant DataAccess "Add" method.
            foreach (var EventEntry in eventList)
            {
                AddNewEvent(EventEntry);
            }
        }

        /// <summary>
        /// Creates a list of test data to be added to the table and then sends each item to the database.
        /// </summary>
        private void SeedGamesTable()
        {
            //populate list of data model objects with pre-filled data
            List<Game> gameList = new List<Game>
            {
                new Game
                {
                    GameName = "Call of heroes",
                    GameType = "team"
                },
                new Game
                {
                    GameName = "The warriors",
                    GameType = "team",
                },
                new Game
                {
                    GameName = "Adventures",
                    GameType = "solo",
                }
            };
            //Add each to the database using the relevant DataAccess "Add" method.
            foreach (var GameEntry in gameList)
            {
                AddNewGame(GameEntry);
            }
        }

        /// <summary>
        /// Creates a list of test data to be added to the table and then sends each item to the database.
        /// </summary>
        private void SeedTeamDetailsTable()
        {
            //populate list of data model objects with pre-filled data
            List<TeamDetail> teamList = new List<TeamDetail>
            {
                new TeamDetail
                {
                    TeamName = "Fnatic",
                    PrimaryContact = "Will",
                    ContactPhone = "0646723918",
                    ContactEmail = "willfnatic@gmail.com",
                    CompetitionPoints = 80,

                },
                new TeamDetail
                {
                    TeamName = "G2",
                    PrimaryContact = "Greg",
                    ContactPhone = "0923932034",
                    ContactEmail = "Greg@Yahoo.com",
                    CompetitionPoints = 30,
                },
                new TeamDetail
                {
                    TeamName = "Faze",
                    PrimaryContact = "Henry",
                    ContactPhone = "028492184",
                    ContactEmail = "faze@gmail.com",
                    CompetitionPoints = 30,
                }
            };
            //Add each to the database using the relevant DataAccess "Add" method.
            foreach (var teamDetailEntry in teamList)
            {
                AddNewTeamDetail(teamDetailEntry);
            }

        }

        /// <summary>
        /// Creates a list of test data to be added to the table and then sends each item to the database.
        /// </summary>
        private void SeedTeamResultsTable()
        {
            //populate list of data model objects with pre-filled data
            List<TeamResult> resultViewList = new List<TeamResult>();
            resultViewList.Add(new TeamResult
            {
                TeamResultID = 1,
                EventID = 1,
                GameID = 2,
                TeamID = 1,
                OppositeTeamID = 2,
                Result = "Win",
            });
            resultViewList.Add(new TeamResult
            {
                TeamResultID = 2,
                EventID = 3,
                GameID = 3,
                TeamID = 2,
                OppositeTeamID = 2,
                Result = "Draw",
            });
            resultViewList.Add(new TeamResult
            {
                TeamResultID = 3,
                EventID = 3,
                GameID = 3,
                TeamID = 3,
                OppositeTeamID = 3,
                Result = "Lose",
            });

            foreach (var newTeamResult in resultViewList)
            {
                AddNewTeamResult(newTeamResult);
            }
        }
    }
}
