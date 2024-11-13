namespace Preventyon.Models.DTO.Incidents
{
    public class UpdateIncidentDTO
    {
        public string IncidentTitle { set; get; }


        public string IncidentDescription { set; get; }

        public string IncidentType { set; get; }


        public string Category { set; get; }


        public string Priority { set; get; }


        public string InvestigationDetails { set; get; }


        public string AssociatedImpacts { set; get; }


        public string CollectionOfEvidence { set; get; }


        public string Correction { set; get; }


        public string CorrectiveAction { set; get; }
        public DateTime CorrectionCompletionTargetDate { set; get; }
        public DateTime CorrectionActualCompletionDate { set; get; }
        public DateTime CorrectiveActualCompletionDate { set; get; }


        public string IncidentStatus { set; get; }
        public double CorrectionDetailsTimeTakenToCloseIncident { set; get; }
        public double CorrectiveDetailsTimeTakenToCloseIncident { set; get; }

    }
}
