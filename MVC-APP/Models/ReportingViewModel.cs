namespace MVCAPP.Models {
    public class ReportingViewModel {
        public Dictionary<string, int> StatusCounts { get; set; }
        public List<TicketStatus> TicketStatuses { get; set; }
        public Dictionary<string, int> PriorityCounts { get; set; }
    }

    public class TicketStatus {
        public string Name { get; set; }
        public List<TicketData> Tickets { get; set; }
    }

    public class TicketData {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}