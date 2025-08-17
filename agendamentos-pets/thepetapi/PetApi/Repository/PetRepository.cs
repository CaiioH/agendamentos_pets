using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetApi.Data;
using PetApi.Models;

namespace PetApi.Repository
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Pet pet)
        {
            try
            {
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao adicionar pet ao banco de dados", ex);
            }
        }

        public async Task Update(Pet pet)
        {
            try
            {
                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao atualizar pet no banco de dados", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet == null)
                {
                    throw new Exception("Pet não encontrado");
                }
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao remover pet do banco de dados", ex);
            }
        }
    
        public async Task<List<Pet>> ListarTodosPets()
        {
            try
            {
                var pets = await _context.Pets.ToListAsync();
                if (pets == null)
                {
                    throw new Exception("Não há pets no banco de dados");
                }
                return pets;
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao buscar pets no banco de dados", ex);
            }
        }

        public async Task<Pet?> BuscarPetPorId(int id)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(u => u.Id == id);
                if (pet == null)
                {
                    throw new Exception("Pet não existe no banco de dados");
                }
                return pet;
            }
            catch(Exception ex)
            {
                throw new Exception("Error ao buscar pet no banco de dados", ex);
            }
        }
        
        public async Task<Pet?> BuscarPetPorNomeETutor(string nomePet, string emailTutor)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(u => u.Nome == nomePet && u.EmailTutor == emailTutor);
                if (pet == null)
                {
                    throw new Exception("Pet não existe no banco de dados");
                }
                Console.WriteLine($"TA AQUI: {pet}");
                return pet;
            }
            catch(Exception ex)
            {
                 throw new KeyNotFoundException("Error ao buscar pet no banco de dados", ex);
            }
        }

        public async Task<List<Pet>> ListarPetsPorEspecie(string especie)
        {
            try
            {
                var pets = await _context.Pets.Where(p => p.Tipo.Contains(especie)).ToListAsync();
                if (pets == null)
                {
                    throw new Exception("Não há pets com essa espécie no banco de dados");
                }
                return pets;
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar pets por espécie no banco de dados", ex);
            }
        }

        public async Task<List<Pet>> ListarPetsPorRaca(string raca)
        {
            try
            {
                var pets = await _context.Pets.Where(p => p.NomeRaca.Contains(raca)).ToListAsync();
                if (pets == null)
                {
                    throw new Exception("Não há pets com essa raça no banco de dados");
                }
                return pets;
            }
            catch (Exception ex)
            {
                throw new Exception("Error ao buscar pets por raça no banco de dados", ex);
            }
        }
    }
}