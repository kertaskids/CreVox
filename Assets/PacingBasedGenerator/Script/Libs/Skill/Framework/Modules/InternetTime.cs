using UnityEngine;
using System.Collections;
using System;
namespace Skill.Framework.Modules
{
    public class InternetTime : MonoBehaviour
    {
        [System.Serializable]
        public class ServerInfo
        {
            public string Name = "Nist";
            public string Url = "http://nist.time.gov/actualtime.cgi?lzbc=siqm9b";
            public float Factor = 0.001f;
        }

        public ServerInfo[] Servers = new ServerInfo[]
        {
       new ServerInfo() { Url =  "http://nist.time.gov/actualtime.cgi?lzbc=siqm9b" , Factor = 0.001f  }
        };

        public bool IsSynced { get; private set; }
        public bool IsBusy { get; private set; }

        public System.DateTime DateTime { get; private set; }



        public void Sync()
        {
            if (IsBusy) return;
            IsSynced = false;
            IsBusy = true;
            StartCoroutine(SyncCoroutine());
        }

        private IEnumerator SyncCoroutine()
        {
            IsBusy = true;

            foreach (ServerInfo server in Servers)
            {
                WWW www = new WWW(server.Url);
                yield return www;

                if (string.IsNullOrEmpty(www.error) && !string.IsNullOrEmpty(www.text))
                {
                    try
                    {
                        string time = System.Text.RegularExpressions.Regex.Match(www.text, @"(?<=\btime="")[^""]*").Value;
                        double milliseconds = Convert.ToInt64(time) * server.Factor;
                        DateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();

                        Debug.Log("Server is : " + server.Url);
                        Debug.Log("Time : " + DateTime.ToString());

                        IsSynced = true;
                        break;
                    }
                    catch
                    {
                        IsSynced = false;
                    }
                }
                else
                {
                    IsSynced = false;
                }
            }
            IsBusy = false;
        }

        //private void GetFastestNISTDate()
        //{
        //    foreach (string server in _Servers)
        //    {
        //        try
        //        {
        //            // Connect to the server (at port 13) and get the response
        //            string serverResponse = string.Empty;
        //            using (var reader = new System.IO.StreamReader(new System.Net.Sockets.TcpClient(server, 13).GetStream()))
        //            {
        //                serverResponse = reader.ReadToEnd();
        //            }
        //            // If a response was received
        //            if (!string.IsNullOrEmpty(serverResponse))
        //            {
        //                // Split the response string ("55596 11-02-14 13:54:11 00 0 0 478.1 UTC(NIST) *")
        //                string[] tokens = serverResponse.Split(' ');

        //                // Check the number of tokens
        //                if (tokens.Length >= 6)
        //                {
        //                    // Check the health status
        //                    string health = tokens[5];
        //                    if (health == "0")
        //                    {
        //                        // Get date and time parts from the server response
        //                        string[] dateParts = tokens[1].Split('-');
        //                        string[] timeParts = tokens[2].Split(':');

        //                        // Create a DateTime instance
        //                        DateTime utcDateTime = new System.DateTime(
        //                            Convert.ToInt32(dateParts[0]) + 2000,
        //                            Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]),
        //                            Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]),
        //                            Convert.ToInt32(timeParts[2]));

        //                        // Convert received (UTC) DateTime value to the local timezone
        //                        Time = utcDateTime.ToLocalTime();
        //                        IsSynced = true;
        //                        break;
        //                        // Response successfully received; exit the loop
        //                    }
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            // Ignore exception and try the next server
        //        }
        //    }
        //    _SyncComplete = true;
        //}   
    }
}
