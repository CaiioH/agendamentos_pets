using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetApi.Models;

namespace PetApi.Repository
{
    public interface IPetRepository
    {
        Task Add(Pet pet);
        Task Update(Pet pet);
        Task Delete(int id);
        Task<List<Pet>> ListarTodosPets();
        Task<Pet?> BuscarPetPorId(int id);
        Task<List<Pet>> ListarPetsPorEspecie(string especie);
        Task<List<Pet>> ListarPetsPorRaca(string raca);
        Task<Pet?> BuscarPetPorNomeETutor(string nomePet, string emailTutor);
    }
}