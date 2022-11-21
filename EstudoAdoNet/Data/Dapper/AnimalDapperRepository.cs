using Dapper;
using EstudoAdoNet.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace EstudoAdoNet.Data
{
    public class AnimalDapperRepository : RepositorioDapperGenerico<Animal>
    {
        public override string TableName { get; set; } = "Animal";
    }
}