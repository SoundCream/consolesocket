using System.Net.Sockets;

namespace ConsoleSocket
{
    public class SyncDataService
    {
        private readonly char splitCodeMessage = ';';
        private readonly string splitMessage = "$";

        private int port = 8080;
        private string serverIP = "127.0.0.1";
        private string programCode = "0";

        public async void SendMessage(string message)
        {
            Action sendMessageToServer = () =>
            {
                var clientSocket = new TcpClient();
                clientSocket.Connect(serverIP, port);
                var serverStream = clientSocket.GetStream();
                var sendMessage = string.Concat(programCode, splitCodeMessage, message, splitMessage);
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(sendMessage);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            };
            await Task.Run(sendMessageToServer);
        }


        public async void OpenSocket(Action<string> callBackFunction = null)
        {
            Action beginListener = () =>
            {
                var server = new TcpListener(port);
                server.Start();
                var result = server.AcceptTcpClientAsync().Result;
                var responds = result.GetStream();

                byte[] bytesFrom = new byte[result.ReceiveBufferSize];
                responds.Read(bytesFrom, 0, (int)result.ReceiveBufferSize);
                string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                var indexSplit = dataFromClient.IndexOf(splitMessage);
                if (indexSplit > 0)
                {
                    var clientMessage = dataFromClient.Substring(0, indexSplit);
                    var systemMessage = clientMessage.Split(splitCodeMessage);
                    if (systemMessage.Count() == 2 && systemMessage[0] == programCode)
                    {
                        var message = systemMessage[1];
                        if (callBackFunction != null)
                        {
                            callBackFunction(message);
                        }
                    }
                }
                server.Stop();
            };
            try
            {
                while (true)
                {
                    await Task.Run(beginListener);
                }
            }
            catch (Exception ex)
            {
                //logger.ErrorException(LogTemplate.LogMessageWithCode("ConnectionLose", "{96B744C6-4CDE-4D84-B2CF-C46FBC37606C}"), ex);
                //ApplicationHelper.AlertMessageForm("Background Connection to server has been lost.");
            }
        }
    }
}
