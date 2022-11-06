using EstudoAdoNet.Data;
using EstudoAdoNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace EstudoAdoNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        // GET: api/<AnimalController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AnimalController>/5
        [HttpGet("{id}")]
        public async Task<string> GetAsync(int id)
        {
            AnimalRepositorio rep = new();
            var teste = await rep.AbrirConexaoAsync();
            return teste.ConnectionString;
        }

        // POST api/<AnimalController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string value)
        {
            AnimalRepositorio rep = new();
            Animal anumal = new Animal
            {
                Especie = value,
                Id = Guid.NewGuid()
            };
            var result = await rep.Incluir(anumal);
            return Ok(result);
        }

        // PUT api/<AnimalController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AnimalController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
