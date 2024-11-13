using System.ComponentModel.DataAnnotations.Schema;
namespace Preventyon.Models
{
    public class Incident
    {

        public int Id { get; set; }


        public string IncidentNo { set; get; }

        [Column(TypeName = "text")]
        public string IncidentTitle { set; get; }


        public string IncidentDescription { set; get; }


        public string ReportedBy { set; get; }


        public string RoleOfReporter { set; get; }
        public DateTime IncidentOccuredDate { set; get; }
        public DateTime MonthYear { set; get; }


        public string IncidentType { set; get; }


        public string Category { set; get; }


        public string Priority { set; get; }


        public string ActionAssignedTo { set; get; }


        public string DeptOfAssignee { set; get; }


        public string InvestigationDetails { set; get; }


        public string AssociatedImpacts { set; get; }


        public string CollectionOfEvidence { set; get; }


        public string Correction { set; get; }


        public string CorrectiveAction { set; get; }
        public DateTime CorrectionCompletionTargetDate { set; get; }
        public DateTime CorrectionActualCompletionDate { set; get; }
        public DateTime CorrectiveActualCompletionDate { set; get; }


        public string IncidentStatus { set; get; }

        public bool IsDraft { set; get; }

        public bool IsSubmittedForReview { set; get; }

        public int EmployeeId { get; set; }


        public List<string> DocumentUrls { get; set; }

        [NotMapped]
        public bool IsCorrectionFilled { get; set; }

        [NotMapped]
        public int? Accepted { get; set; }
        public ICollection<AssignedIncidents> AssignedIncidents { get; set; }

        public DateTime createdAt { set; get; }



        public Incident()
        {
            createdAt = DateTime.UtcNow;
        }
        public double CorrectionDetailsTimeTakenToCloseIncident { set; get; }
        public double CorrectiveDetailsTimeTakenToCloseIncident { set; get; }
        public string PreventiveAction { set; get; }
        public string Remarks { get; set; }

    }

}
