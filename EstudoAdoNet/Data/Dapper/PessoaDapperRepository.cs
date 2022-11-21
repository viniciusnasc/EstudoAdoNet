using EstudoAdoNet.Models;

namespace EstudoAdoNet.Data.Dapper
{
    public class PessoaDapperRepository : RepositorioDapperGenerico<Pessoa>
    {
        public override string TableName { get; set; } = "Pessoa";
    }
}