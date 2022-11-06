using EstudoAdoNet.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace EstudoAdoNet.Data
{
    public abstract class RepositorioGenerico<T> where T : BaseEntity
    {
        public abstract string TableName { get; set; }

        private SqlConnection cn;

        private void Conexao()
        {
            var configuration = GetConfiguration();
            cn = new SqlConnection(configuration.GetConnectionString("SqlConn"));
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public async Task<SqlConnection> AbrirConexaoAsync()
        {
            try
            {
                Conexao();
                await cn.OpenAsync();
                return cn;
            }
            catch (SqlException e)
            {
                var testecn = new SqlConnection(cn.ConnectionString.Replace("EstudoAdoNet", "master"));
                await testecn.OpenAsync();

                if (!await ExisteDatabase(testecn))
                {
                    CriarDatabase(testecn).GetAwaiter();
                    await testecn.CloseAsync();
                    //Thread.Sleep(5000);
                    return await AbrirConexaoAsync();
                }

                return null;
            }
        }

        private async Task<bool> ExisteDatabase(SqlConnection cn)
        {
            SqlCommand command = new($"DECLARE @dbname nvarchar(128) SET @dbname = N'EstudoAdoNet' IF(EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE('[' + name + ']' = @dbname OR name = @dbname))) PRINT 'db exists'", cn);
            var teste = await command.ExecuteReaderAsync();
            return teste.HasRows;
        }

        private async Task CriarDatabase(SqlConnection cn)
        {
            SqlCommand createDatabase = new($"Create database EstudoAdoNet", cn);
            await createDatabase.ExecuteNonQueryAsync();
        }

        public async Task FecharConexaoAsync()
        {
            try
            {
                await cn.CloseAsync();
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task<T> Incluir(T entity)
        {
            try
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

                SqlConnection cn = await AbrirConexaoAsync();
                SqlCommand command = new($"insert into {TableName}({atributos}) values({valores})", cn);
                await command.ExecuteNonQueryAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                await FecharConexaoAsync();

                SqlConnection cn = await AbrirConexaoAsync();
                SqlCommand existeTabela = new("IF OBJECT_ID (N'animal', N'U') IS NOT NULL SELECT 1", cn);
                var tabela = await existeTabela.ExecuteReaderAsync();

                if (!tabela.HasRows)
                {
                    SqlCommand command = new($"create table Animal(id UNIQUEIDENTIFIER PRIMARY KEY, Especie varchar(100))", cn);
                    var teste = await command.ExecuteNonQueryAsync();
                    await FecharConexaoAsync();
                    return await Incluir(entity);
                }
                return null;
            }
            finally
            {
                await FecharConexaoAsync();
            }
        }
    }
}