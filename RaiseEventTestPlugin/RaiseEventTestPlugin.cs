using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using Photon.Hive.Plugin;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {
        private string connStr;
        private MySqlConnection conn;
        public string ServerString
        {
            get;
            private set;
        }
        public int CallsCount
        {
            get;
            private set;
        }
        public RaiseEventTestPlugin()
        {
            this.UseStrictMode = true;
            this.ServerString = "ServerMessage";
            this.CallsCount = 0;
            // --- Connect to MySQL.
            ConnectToMySQL();
        }
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (1 == info.Request.EvCode)
            {
                string playerName = Encoding.Default.GetString((byte[])info.Request.Data);
                string sql = "INSERT INTO users (name, date_created) VALUES ('" + playerName + "', now())";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                ++this.CallsCount;
                int cnt = this.CallsCount;
                string ReturnMessage = playerName + " clicked the button. Now the count is " + cnt.ToString();
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                senderActor: 0,
                targetGroup: 0,

                evCode: info.Request.EvCode,
                data: new Dictionary<byte, object>()
                {
                    {
                        (byte)245, ReturnMessage
                    }
                },
                
                cacheOp: 0);
            }
        }
        public void ConnectToMySQL()
        {

            // Connect to MySQL

            ////hy's
            //connStr = "server=localhost;user=root;database=accountdb;port=3306;password=1234;sslmode=none";

            connStr =
           "server=localhost;user=root;database=photon;port=3306;password=DM2341sidm;MySqlSslMode=none;";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void DisconnectFromMySQL()
        {
            conn.Close();
        }
    }
}
