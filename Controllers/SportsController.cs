using ApiWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        IConfiguration Configuration;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        public SportsController(IConfiguration configuration)
        {
            Configuration = configuration;
            sqlConnection=new SqlConnection(Configuration.GetConnectionString("College Sports Management System"));
            sqlCommand=sqlConnection.CreateCommand();
        }
        public List<int> Data=new List<int>() { 1,1,1,99};
        
        // GET: api/<SportsController>
        [HttpGet]
        public IActionResult Get()
        {
            List<SportsModel> sportsList = new List<SportsModel>();
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    using (sqlCommand)
                    {
                        sqlCommand.CommandText = "SELECT * FROM SCOREBOARD";
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            SportsModel sport = new SportsModel();
                            sport.SportId = Convert.ToInt32(reader["Sport_Id"]);
                            sport.SportName = Convert.ToString(reader["Sport_Name"]);
                            sport.TournamentId = Convert.ToInt32(reader["Tournament_Id"]);
                            sport.TournamentName = Convert.ToString(reader["Tournament_Name"]);
                            List<Player> Tempplayers = new List<Player>();
                            for (int i = 1; i <=reader.FieldCount - 4; i++)
                            {
                                Player p1 = new Player(Convert.ToInt32(reader[$"Player{i}"]), $"Player{i}");
                                Tempplayers.Add(p1);
                            }
                            sport.players= Tempplayers;
                            sportsList.Add(sport);
                        }
                    }
                }
                return Ok(sportsList);
            }
            catch (SqlException e){
                Console.WriteLine(e.Message);
           
                    return NotFound();
            }
            
        }

        // GET api/<SportsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            List<SportsModel> sportsList = new List<SportsModel>();
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    using (sqlCommand)
                    {
                        sqlCommand.CommandText = $"SELECT * FROM SCOREBOARD WHERE SPORT_ID={id}";
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            SportsModel sport = new SportsModel();
                            sport.SportId = Convert.ToInt32(reader["Sport_Id"]);
                            sport.SportName = Convert.ToString(reader["Sport_Name"]);
                            sport.TournamentId = Convert.ToInt32(reader["Tournament_Id"]);
                            sport.TournamentName = Convert.ToString(reader["Tournament_Name"]);
                            for(int i=1;i<=reader.FieldCount-4;i++)
                            {
                                sport.players.Add(new Player(Convert.ToInt32(reader[$"Player{i}"]),$"Player{i}"));
                            }
                            sportsList.Add(sport);
                        }
                    }
                }
                return Ok(sportsList);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);

                return NotFound();
            }
        }

        // POST api/<SportsController>
        [HttpPost]
        public void Post(List<string> Data)
        {
            
            //Data list takes sport_id,tournament_id,player_id,score
            string TableName = "SCOREBOARD";
            using (sqlConnection)
            {
                sqlConnection.Open();
                //CREATE TABLE IF DOES NOT EXIST
                using (sqlCommand)
                {

                    sqlCommand.CommandText = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{TableName}' and xtype='U') " +
                        $"CREATE TABLE {TableName} (SPORT_ID INT,SPORT_NAME VARCHAR(30),TOURNAMENT_ID INT,TOURNAMENT_NAME VARCHAR(30))";

                    sqlCommand.ExecuteNonQuery();

                }
                string PlayerName;
                //FETCHING PLAYER NAME
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"SELECT PLAYER_NAME FROM PLAYERS WHERE PLAYER_ID={Data[2]}";
                    PlayerName = sqlCommand.ExecuteScalar().ToString();
                }
                //ADDING PLAYER AS COLUMN IF DOESNOT EXIST
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{TableName}' AND COLUMN_NAME = '{PlayerName}')"
                        + $"ALTER TABLE {TableName} ADD {PlayerName} int NULL";

                    sqlCommand.ExecuteNonQuery();
                }
                //ADDING ROW IF DOESNOT EXIST
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"IF NOT EXISTS (SELECT* FROM {TableName} WHERE  SPORT_ID = {Data[0]} AND TOURNAMENT_ID = {Data[1]}) " +
                                    $"BEGIN INSERT INTO {TableName} (SPORT_ID,SPORT_NAME,TOURNAMENT_ID ,TOURNAMENT_NAME,{PlayerName})" +
                                    $" SELECT {Data[0]},S.SPORT_NAME,{Data[1]},T.TOURNAMENT_NAME,{Data.Last()} FROM SPORTS AS S,TOURNAMENTS AS T " +
                                       $"WHERE S.SPORT_ID={Data[0]} AND T.TOURNAMENT_ID={Data[1]} END" +
                                       $" ELSE BEGIN UPDATE {TableName} SET {PlayerName}={Data.Last()} WHERE SPORT_ID={Data[0]} AND TOURNAMENT_ID={Data[1]} END";

                    sqlCommand.ExecuteNonQuery();
                }


            }
        }
        
        // PUT api/<SportsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SportsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        /*public void AddOrEditScoreBoard(List<string> Data)
        {
            //Data list takes sport_id,tournament_id,player_id,score
            string TableName = "SCOREBOARD";
            using (sqlConnection)
            {
                sqlConnection.Open();
                //CREATE TABLE IF DOES NOT EXIST
                using (sqlCommand)
                {

                    sqlCommand.CommandText = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{TableName}' and xtype='U') " +
                        $"CREATE TABLE {TableName} (SPORT_ID INT,SPORT_NAME VARCHAR(30),TOURNAMENT_ID INT,TOURNAMENT_NAME VARCHAR(30))";

                    sqlCommand.ExecuteNonQuery();

                }
                string PlayerName;
                //FETCHING PLAYER NAME
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"SELECT PLAYER_NAME FROM PLAYERS WHERE PLAYER_ID={Data[2]}";
                    PlayerName = sqlCommand.ExecuteScalar().ToString();
                }
                //ADDING PLAYER AS COLUMN IF DOESNOT EXIST
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '{TableName}' AND COLUMN_NAME = '{PlayerName}')"
                        + $"ALTER TABLE {TableName} ADD {PlayerName} int NULL";

                    sqlCommand.ExecuteNonQuery();
                }
                //ADDING ROW IF DOESNOT EXIST
                using (sqlCommand)
                {
                    sqlCommand.CommandText = $"IF NOT EXISTS (SELECT* FROM {TableName} WHERE  SPORT_ID = {Data[0]} AND TOURNAMENT_ID = {Data[1]}) " +
                                    $"BEGIN INSERT INTO {TableName} (SPORT_ID,SPORT_NAME,TOURNAMENT_ID ,TOURNAMENT_NAME,{PlayerName})" +
                                    $" SELECT {Data[0]},S.SPORT_NAME,{Data[1]},T.TOURNAMENT_NAME,{Data.Last()} FROM SPORTS AS S,TOURNAMENTS AS T " +
                                       $"WHERE S.SPORT_ID={Data[0]} AND T.TOURNAMENT_ID={Data[1]} END" +
                                       $" ELSE BEGIN UPDATE {TableName} SET {PlayerName}={Data.Last()} WHERE SPORT_ID={Data[0]} AND TOURNAMENT_ID={Data[1]} END";

                    sqlCommand.ExecuteNonQuery();
                }


            }
        }*/
    }
}
