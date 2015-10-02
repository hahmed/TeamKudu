using System;

namespace Team.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Email { get; set; }

        public string BillingEmail { get; set; }

        public DateTime CreatedAt { get; set; }

        public CompanyType Type { get; set; }
    }

    public enum CompanyType
    {
        Lender = 1,

        SurveyingFirm = 2,
    }
}