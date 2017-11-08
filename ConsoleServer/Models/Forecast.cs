
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer.Models
{
    [Table(Name = "Forecasts")]
    public class Forecast
    {
        [Column(Name = "Id", IsDbGenerated = true, IsPrimaryKey = true, DbType = "INTEGER")]
        [Key]
        public int Id { get; set; }

        [Column(Name = "City_Id", DbType = "INTEGER")]
        public int City_Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("City_Id")]
        public City City { get; set; }

        [Column(Name = "Time", DbType = "BIGINT")]
        public long Time { get; set; }

        [Column(Name = "Temp", DbType = "DOUBLE")]
        public double Temp { get; set; }

        [Column(Name = "Humidity", DbType = "INTEGER")]
        public int Humidity { get; set; }

        [Column(Name = "Pressure", DbType = "INTEGER")]
        public int Pressure { get; set; }

    }
}
