using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Corona_Statistics.DataAccess
{
    public class FederalState
    {
        public int Id { get; set; }

        [MaxLength(150)] [Required] public string Name { get; set; } = string.Empty;

        public List<District> Districts { get; set; } = new List<District>();
    }

    public class District
    {
        public int Id { get; set; }

        [JsonIgnore]
        public FederalState FederalState { get; set; }

        public int Code { get; set; }

        [MaxLength(150)] [Required] public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public Covid19Case Covid19Case { get; set; }

        [JsonIgnore]
        public int Covid19CasesId { get; set; }
    }

    public class Covid19Case
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public District District { get; set; }

        [JsonIgnore]
        public int DistrictId { get; set; }

        public int Population{ get; set; }

        public int Cases { get; set; }

        public int Deaths { get; set; }

        public int SevenDayIncidence { get; set; }
    }
    
    public class TotalCase
    {
        public int Id { get; set; }
        
        public DateTime Date { get; set; }

        public int PopulationsSum { get; set; }

        public int CasesSum { get; set; }

        public int DeathsSum { get; set; }

        public int SevenDaysIncidentsSum { get; set; }
    }
}