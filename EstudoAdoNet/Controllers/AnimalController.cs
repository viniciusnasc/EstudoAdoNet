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
        public async Task<IEnumerable<Animal>> GetAsync()
        {
            AnimalDapperRepository rep = new();
            return await rep.GetAll();
        }

        // GET api/<AnimalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            AnimalDapperRepository rep = new();
            var teste = await rep.FindById(id);
            return Ok(teste);
        }

        // POST api/<AnimalController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string value)
        {
            //AnimalRepositorio rep = new();
            AnimalDapperRepository rep = new();
            Animal animal = new Animal
            {
                Especie = value,
                Id = Guid.NewGuid()
            };
            var result = await rep.Incluir(animal);
            return Ok(result);
        }

        // PUT api/<AnimalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string value)
        {
            AnimalDapperRepository rep = new();
            var animal = new Animal
            {
                Especie = value
            };

            var result = await rep.Alterar(animal, id);
            return Ok(result);
        }

        // DELETE api/<AnimalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            AnimalDapperRepository rep = new();
            await rep.Deletar(id);
            return Ok();
        }
    }
}
