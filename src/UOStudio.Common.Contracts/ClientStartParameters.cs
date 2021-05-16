namespace UOStudio.Common.Contracts
{
    public class ClientStartParameters
    {
        public string ConnectionTicket { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string UserName { get; set; }
    }
}
