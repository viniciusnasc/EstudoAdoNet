using EstudoAdoNet.Models;

namespace EstudoAdoNet.Data
{
    public class AnimalRepositorio : RepositorioGenerico<Animal>
    {
        public override string TableName { get; set; } = "Animal";
    }
}