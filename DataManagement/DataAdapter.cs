using DataManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;
using DataManagement;


namespace DataManagement
{
    public class DataAdapter
    {
        //#TeamDetail
        public List<TeamDetail> GetAllTeamDetails()
        {
            //The SQL Query to be sent to the database with our request.
            string query = "SELECT * FROM TeamDetails ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                return connection.Query<TeamDetail>(query).ToList();
            }
        }

     
        public TeamDetail GetTeamDetailByTeamID(int TeamID)
        {
            //The query to be passsed to the databse to be executed.
            string query = "SELECT * FROM TeamDetails " +
                           $"WHERE TeamID = {TeamID} ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                return connection.QuerySingle<TeamDetail>(query);
            }
        }

        public void AddNewTeamDetail(TeamDetail teamDetailEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "INSERT INTO TeamDetails (TeamName,PrimaryContact,ContactPhone,ContactEmail,CompetitionPoints) " +
                           "VALUES (@TeamName,@PrimaryContact,@ContactPhone,@ContactEmail,@CompetitionPoints) ";

          
            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, teamDetailEntry);
            }
        }

        public void UpdateTeamDetail(TeamDetail teamDetailEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "UPDATE TeamDetails " +
                           "SET TeamName = @TeamName, PrimaryContact = @PrimaryContact, ContactPhone = @ContactPhone, ContactEmail = @ContactEmail, CompetitionPoints = @CompetitionPoints " +
                           "WHERE TeamID = @TeamID ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, teamDetailEntry);
            }
        }

        public void DeleteTeamDetail(int TeamID)
        {
            // Create a connection object
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Open the connection
                connection.Open();

                // Begin a transaction to ensure both deletions succeed or fail together
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First, delete related records from the TeamResults table
                        string deleteTeamResultsQuery = "DELETE FROM TeamResults WHERE TeamID = @TeamID OR OppositeTeamID = @TeamID";
                        connection.Execute(deleteTeamResultsQuery, new { TeamID }, transaction);

                        // Now delete the record from the TeamDetails table
                        string deleteTeamDetailQuery = "DELETE FROM TeamDetails WHERE TeamID = @TeamID";
                        connection.Execute(deleteTeamDetailQuery, new { TeamID }, transaction);

                        // Commit the transaction if both deletions succeed
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there's an error
                        transaction.Rollback();
                        throw;
                    }
                }
            } // Connection is automatically closed when leaving the using block
        }
        /*
        public void DeleteTeamDetail(int TeamID)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "DELETE FROM TeamDetails " +
                          $"WHERE TeamID = {TeamID} " ;

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query);
            }
        }
        //#EndTeamDetail
        */


        //#Events

        public List<Event> GetAllEvents()
        {
            //The SQL Query to be sent to the database with our request.
            string query = "SELECT * FROM Events ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                return connection.Query<Event>(query).ToList();
            }
        }


        public Event GetEventByEventID(int EventID)
        {
            //The query to be passsed to the databse to be executed.
            string query = "SELECT * FROM Events " +
                           $"WHERE EventID = {EventID} ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                return connection.QuerySingle<Event>(query);
            }
        }

        public void AddNewEvent(Event EventEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "INSERT INTO Events (EventName,EventLocation,EventDate) " +
                           "VALUES (@EventName,@EventLocation,@EventDate) ";


            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, EventEntry);
            }
        }

        public void UpdateEvent(Event EventEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "UPDATE Events " +
                           "SET EventName = @EventName, EventLocation = @EventLocation, EventDate = @EventDate " +
                           "WHERE EventID = @EventID ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, EventEntry);
            }
        }
        public void DeleteEvent(int EventID)
        {
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                connection.Open();
                // Begin a transaction to ensure both deletions succeed or fail together
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete related records from the TeamResults table
                        string deleteTeamResultsQuery = "DELETE FROM TeamResults WHERE EventID = @EventID";
                        connection.Execute(deleteTeamResultsQuery, new { EventID }, transaction);

                        // Now delete the record from the Events table
                        string deleteEventQuery = "DELETE FROM Events WHERE EventID = @EventID";
                        connection.Execute(deleteEventQuery, new { EventID }, transaction);

                        // Commit the transaction if both deletions succeed
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there's an error
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        /*
        public void DeleteEvent(int EventID)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "DELETE FROM Events " +
                          $"WHERE EventID = {EventID} ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query);
            }
        }
        */
        //#End Events



        //#Games


        public List<Game> GetAllGames()
        {
            //The SQL Query to be sent to the database with our request.
            string query = "SELECT * FROM Games ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                return connection.Query<Game>(query).ToList();
            } 
        }


        public Game GetGameByGameID(int GameID)
        {
            //The query to be passsed to the databse to be executed.
            string query = "SELECT * FROM Games " +
                           $"WHERE GameID = {GameID} ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                return connection.QuerySingle<Game>(query);
            }
        }

        public void AddNewGame(Game GameEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "INSERT INTO Games (GameName,GameType) " +
                           "VALUES (@GameName,@GameType) ";


            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, GameEntry);
            }
        }

        public void UpdateGame(Game GameEntry)
        {
            //The SQL Query to be sent to the database with our request.
            string query = "UPDATE Games " +
                           "SET GameName = @GameName, GameType = @GameType " +
                           "WHERE GameID = @GameID ";

            // A using statement to manage the connection resource(object) which will dispose
            // of the resource once it is finished.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Request to be sent to the database.
                connection.Execute(query, GameEntry);
            }
        }

        public void DeleteGame(int GameID)
        {
            // Create a connection object
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                // Open the connection
                connection.Open();

                // Begin a transaction to ensure both deletions succeed or fail together
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First, delete related records from the TeamResults table
                        string deleteTeamResultsQuery = "DELETE FROM TeamResults WHERE GameID = @GameID";
                        connection.Execute(deleteTeamResultsQuery, new { GameID }, transaction);

                        // Now delete the record from the Games table
                        string deleteGameQuery = "DELETE FROM Games WHERE GameID = @GameID";
                        connection.Execute(deleteGameQuery, new { GameID }, transaction);

                        // Commit the transaction if both deletions succeed
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there's an error
                        transaction.Rollback();
                        throw;
                    }
                }
            } // Connection is automatically closed when leaving the using block
        }
        /*
        public void DeleteGame(int GameID)
        {
            try
            {
                //Using statement structure which uses the provided resource  to perform the provided logic and then automatically
                //disposes of the resource once the structure finishes or an error occurs.
                using (var connection = Helper.GetSQLServerConnection("Default"))
                {
                    //Query string to be passed to the SQL database to perform the desired database interaction.
                    string query = "DELETE FROM Games " +
                                    $"WHERE GameID = {GameID} ";

                    //Method to requests the desired record to be removed from the database.
                    connection.Execute(query);
                }
            }
            catch (Exception e)
            {

            }
        }
        */

        //# End of Game

        //# Start of Team Result


        public List<ResultView> GetAllTeamResults()
        {
            //The query to be passsed to the databse to be executed.

            

            string query = "Select TeamResults.TeamResultID as ResultViewID, TeamResults.Result as Result, " +
                           "Events.EventName as EventHeld, Games.GameName as GamesPlayed, Team.TeamName as Team, OpposingTeam.TeamName as OpposingTeam " +
                           "From TeamResults " +
                           "INNER JOIN " +
                           "Events ON TeamResults.EventID = Events.EventID " +
                           "INNER JOIN " +
                           "Games ON TeamResults.GameID = Games.GameID " +
                           "INNER JOIN " +
                           "TeamDetails AS Team ON Team.TeamID = TeamResults.TeamID " +
                            "INNER JOIN " +
                            "TeamDetails AS OpposingTeam ON OpposingTeam.TeamID = TeamResults.OppositeTeamID ";



            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                return connection.Query<ResultView>(query).ToList();
            }
        }

        public TeamResult GetTeamResultByTeamResultID(int TeamResultID)
        {
            //The query to be passsed to the databse to be executed.
            string query = "SELECT * FROM TeamResults " +
                           $"WHERE TeamResultID = {TeamResultID} ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                return connection.QuerySingle<TeamResult>(query);
            }
        }

        public void AddNewTeamResult(TeamResult newTeamResult)
        {
            //The query to be passsed to the databse to be executed.
            string query = "INSERT INTO TeamResults (EventID, GameID, TeamID, OppositeTeamID, Result) " +
                           "VALUES (@EventID, @GameID, @TeamID, @OppositeTeamID, @Result) ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                connection.Execute(query, newTeamResult);
            }
        }

        public void UpdateTeamResult(TeamResult teamresultEntry)
        {
            //The query to be passsed to the databse to be executed.
            string query = "UPDATE TeamResults " +
                           "SET EventID = @EventID, GameID = @GameID, TeamID = @TeamID, OppositeTeamID = @OppositeTeamID, Result = @Result " +
                           "WHERE TeamResultID = @TeamResultID ";

                          
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                connection.Execute(query, teamresultEntry);
            }
        }

        public void DeleteTeamResult(int TeamResultID)
        {
            //The query to be passsed to the databse to be executed.
            string query = "DELETE FROM TeamResults " +
                           $"WHERE TeamResultID = {TeamResultID} ";
            //The using statement which manages our connection and disposes of it
            //once the request is compelted.
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                //The request/command method being executed by Dapper to perform the
                //SQL query.
                connection.Execute(query);
            }
        }

        public void PerformTransaction(TeamDetail model, int TeamID)
        {
            using (var connection = Helper.GetSQLServerConnection("Default"))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "update TeamDetails " +
                                        $"set CompetitionPoints = CompetitionPoints + {model.CompetitionPoints} " +
                                        $"where TeamID = {TeamID} " ;


                        connection.Execute(query, model, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void DrawInventoryPoints(SqlConnection sql, SqlTransaction transaction, int TeamID)
        {
            try
            {
                string query = "update TeamDetails " +
                               $"set CompetitionPoints = CompetitionPoints + 1 " +
                               $"where TeamID = {TeamID} " ;
                sql.Execute(query, null, transaction);
            }
            catch (Exception ex)
            {
                throw;
            }
        }




    }
}

    

