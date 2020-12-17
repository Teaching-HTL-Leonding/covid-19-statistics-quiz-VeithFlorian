using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Corona_Statistics.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Corona_Statistics.Controllers
{
    [ApiController]
    [Route("api")]
    public class CoronaStatisticsController : ControllerBase
    {
        private readonly CoronaStatisticsContext context;

        public CoronaStatisticsController(CoronaStatisticsContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("states")]
        public IEnumerable<FederalState> GetStates() => context.FederalStates.Include(s => s.Districts);

        [HttpGet]
        [Route("states/{stateId}/cases")]
        public IEnumerable<Covid19Case> GetStates([FromRoute] int stateId) => context.Covid19Cases
            .Where(c => c.District.FederalState.Id == stateId)
            .Include(c => c.District);

        
        [HttpGet]
        [Route("cases")]
        public IEnumerable<TotalCase> GetCases() => context.TotalCases;
    
        

        [HttpPost]
        [Route("importData")]
        public async Task ImportData()
        {
            HttpClient http = new HttpClient();
            if (!context.FederalStates.Any() && !context.Districts.Any())
            {
                string districtsString = await http.GetStringAsync("http://www.statistik.at/verzeichnis/reglisten/polbezirke.csv");
                string[] districtArray = districtsString.Split("\n");


                var federalStates = districtArray.Skip(3)
                    .SkipLast(2)
                    .Select(s => s.Split(";"))
                    .Select(s => s[1])
                    .Distinct()
                    .Select(s => new FederalState() {Name = s});
                
                //await context.FederalStates.AddRangeAsync(federalStates);
                
                var districts=  districtArray.Skip(3)
                    .SkipLast(2)
                    .Select(d => d.Split(";"))
                    .GroupBy(d => d[1])
                    .ToList()
                    .Select(d =>
                    {
                        var federalState = federalStates.FirstOrDefault(s => s.Name == d.Key);
                        if (d.ToList().Select(a => a[3]).Any(a => Regex.IsMatch(a, $".*{federalState.Name}.*\\d.*")))
                        {
                            var list = new List<District>()
                            {
                                new()
                                {
                                    FederalState = federalState,
                                    Code = int.Parse(d.FirstOrDefault()[2]),
                                    Name = federalState.Name
                                }
                            };
                            //federalState.Districts.AddRange(list);
                            return list;
                        }
                        var stateDistricts = d.ToList().Select(a =>
                        {
                            return new District()
                            {
                                FederalState = federalState,
                                Code = int.Parse(a[2]),
                                Name = a[3],
                            };
                        });
                        //federalState.Districts.AddRange(stateDistricts);
                        return stateDistricts;
                    })
                    .SelectMany(d => d);
                
                await context.Districts.AddRangeAsync(districts);
                await context.SaveChangesAsync();
            }
            
            
            if (context.Covid19Cases.Select(c => c.Date).Contains(DateTime.Today))
            {
                context.Covid19Cases.FromSqlRaw("DROP FROM Covid19Cases");
            }
            
            string covid19String =
                await http.GetStringAsync("https://covid19-dashboard.ages.at/data/CovidFaelle_GKZ.csv");
            string[] covid19Array = covid19String.Split("\n");

            var covid19Cases = covid19Array.Skip(1)
                .Select(c => c.Split(";"))
                .Select(c =>
                {
                    var district = context.Districts.FirstOrDefault(d => d.Code == int.Parse(c[1]));
                    var covid19Case = new Covid19Case()
                    {                        
                        Date = DateTime.Today,
                        District = district,
                        DistrictId = district.Id,
                        Population = int.Parse(c[2]),
                        Cases = int.Parse(c[3]),
                        Deaths = int.Parse(c[4]),
                        SevenDayIncidence = int.Parse(c[5])
                    };
                    district.Covid19Case = covid19Case;
                    return covid19Case;
                });
            
            context.TotalCases.AddRange(new TotalCase()
            {
                Date = DateTime.Today,
                PopulationsSum = covid19Cases.Sum(c => c.Population),
                CasesSum = covid19Cases.Sum(c => c.Cases),
                DeathsSum = covid19Cases.Sum(c => c.Deaths),
                SevenDaysIncidentsSum = covid19Cases.Sum(c => c.SevenDayIncidence),
            });

            await context.Covid19Cases.AddRangeAsync(covid19Cases);
            
            await context.SaveChangesAsync();
        }
    }
}