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
    [Table(Name = "Cities")]
    public class City
    {
        [Column(Name = "Id", IsDbGenerated = true, IsPrimaryKey = true, DbType = "INTEGER")]
        [Key]
        public int Id { get; set; }

        [Column(Name = "Name", DbType = "TEXT")]
        public string Name { get; set; }

        [Column(Name = "Country", DbType = "TEXT")]
        public string Country { get; set; }

        [Column(Name = "Weather_api_id", DbType = "INTEGER")]
        public int Weather_api_id { get; set; }
    }
}




