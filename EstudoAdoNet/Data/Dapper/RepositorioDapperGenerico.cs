using Dapper;
using EstudoAdoNet.Models;
using Newtonsoft.Json;
using System.Data.Common;
using System.Data.SqlClient;

namespace EstudoAdoNet.Data
{
    public abstract class RepositorioDapperGenerico<T> where T : BaseEntity
    {
        public abstract string TableName { get; set; }

        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Server=DESKTOP-R9JFMSC\\SQLEXPRESS;Database=EstudoAdoNet;Trusted_Connection=True;MultipleActiveResultSets=true");
            connection.Open();
            return connection;
        }

        public async Task<T> FindById(Guid id)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<T>($"select * from {TableName} where id = @id", new { id = id });
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (var connection = GetOpenConnection())
            {// nome da procedure, storedprocedure
                return await connection.QueryAsync<T>($"select * from {TableName}");
            }
        }

        public async Task<T> Incluir(T entity)
        {
            using (var connection = GetOpenConnection())
            {
                var tuplaAtributoValor = FormatarEntidadeCreate(entity);

                var query = $"insert into {TableName}({tuplaAtributoValor.Item1}) values({tuplaAtributoValor.Item2})";

                await connection.ExecuteAsync(query);

                return await FindById(entity.Id);
            }
        }

        public async Task<T> Alterar(T entity, Guid id)
        {
            using (var connection = GetOpenConnection())
            {
                if(entity.Id == Guid.Empty)
                    entity.Id = id;

                var queryAtributos = FormatarEntidadeUpdate(entity);

                var query = $"update {TableName} set {queryAtributos} where Id like '{entity.Id}'";

                await connection.ExecuteAsync(query);

                return await FindById(id);
            }
        }

        public async Task Deletar(Guid id)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync($"delete from {TableName} where Id like '{id}'");
            }
        }

        private Tuple<string, string> FormatarEntidadeCreate(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string atributos = "";
            string valores = "";

            foreach (var objeto in values)
            {
                if (values.Last().Equals(objeto))
                {
                    atributos += objeto.Key;
                    valores += "'" + objeto.Value + "'";
                }
                else
                {
                    atributos += objeto.Key + ", ";
                    valores += "'" + objeto.Value + "', ";
                }
            }

            return new Tuple<string, string>(atributos, valores);
        }

        private string FormatarEntidadeUpdate(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string query = "";

            foreach (var objeto in values)
            {
                if (values.Last().Equals(objeto))
                {
                    query += objeto.Key + " = ";
                    query += "'" + objeto.Value + "'";
                }
                else
                {
                    query += objeto.Key + " = ";
                    query += "'" + objeto.Value + "', ";
                }
            }

            return query;
        }
    }
}