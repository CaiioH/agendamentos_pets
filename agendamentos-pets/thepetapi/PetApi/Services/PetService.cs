using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PetApi.DTOs;
using PetApi.Models;
using PetApi.Repository;
using PetApi.Services.Interfaces;

namespace PetApi.Services
{
    public class PetService
    {
        private readonly IPetRepository _petRepository;
        private readonly RacaService _racaService;
        private readonly IRabbitMqService _rabbitMqService;

        public PetService(IPetRepository petRepository, RacaService racaService, IRabbitMqService rabbitMqService)
        {
            _petRepository = petRepository;
            _racaService = racaService;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<PetDTO> CriarPet(PetDTO dto)
        {
            try
            {
                var raca = await _racaService.ObterRacaPorNome(dto.NomeRaca, dto.Especie);
                if (raca == null)
                {
                    throw new Exception("Raça não encontrada");
                }

                var imagemUrl = await _racaService.ObterImagemUrl(raca.ReferenceImageId, dto.Especie);

                var pet = new Pet
                {
                    Tutor = dto.Tutor,
                    EmailTutor = dto.EmailTutor,
                    Nome = dto.Nome,
                    Cor = dto.Cor,
                    Sexo = dto.Sexo,
                    Idade = dto.Idade,
                    Tipo = dto.Especie.ToUpper(),
                    NomeRaca = raca.Name,
                    ImagemUrl = imagemUrl,
                    RacaId = raca.Id.ToString(),
                };

                await _petRepository.Add(pet);

                // Cria a mensagem JSON como string
                var message = new
                {
                    type = "pet_created",
                    data = new
                    {
                        Id = pet.Id,
                        Nome = pet.Nome,
                        Especie = pet.Tipo,
                        Tutor = pet.Tutor,
                        EmailTutor = pet.EmailTutor

                    }
                };

                string jsonString = JsonSerializer.Serialize(message);
    
                _rabbitMqService.SendMessage(jsonString, message.type);

                return new PetDTO
                {
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Especie = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public async Task<PetDTO> AtualizarPet(int id, PetDTO dto)
        {
            try
            {
                var pet = await _petRepository.BuscarPetPorId(id);
                if (pet == null)
                {
                    throw new Exception("Pet não encontrado");
                }

                var especie = string.IsNullOrEmpty(dto.Especie) ? pet.Tipo : dto.Especie;
                if (dto.NomeRaca != null)
                {                    
                    var raca = await _racaService.ObterRacaPorNome(dto.NomeRaca, especie);
                    if (raca == null)
                    {
                        throw new Exception("Raça não encontrada");
                    }

                    var imagemUrl = await _racaService.ObterImagemUrl(raca.ReferenceImageId, especie);
                    
                    pet.NomeRaca = string.IsNullOrEmpty(dto.NomeRaca) ? pet.NomeRaca : raca.Name;
                    pet.ImagemUrl = string.IsNullOrEmpty(imagemUrl) ? pet.ImagemUrl : imagemUrl;
                    pet.RacaId = string.IsNullOrEmpty(raca.Id.ToString()) ? pet.RacaId : raca.Id.ToString();
                }

                pet.Tutor = string.IsNullOrEmpty(dto.Tutor) ? pet.Tutor : dto.Tutor;
                pet.EmailTutor = string.IsNullOrEmpty(dto.EmailTutor) ? pet.EmailTutor : dto.EmailTutor;
                pet.Nome = string.IsNullOrEmpty(dto.Nome) ? pet.Nome : dto.Nome;
                pet.Cor = string.IsNullOrEmpty(dto.Cor) ? pet.Cor : dto.Cor;
                pet.Sexo = string.IsNullOrEmpty(dto.Sexo) ? pet.Sexo : dto.Sexo;
                pet.Idade = string.IsNullOrEmpty(dto.Idade) ? pet.Idade : dto.Idade;
                pet.Tipo = string.IsNullOrEmpty(dto.Especie) ? pet.Tipo : especie.ToUpper();

                await _petRepository.Update(pet);

                return new PetDTO
                {
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Especie = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeletarPet(int id)
        {
            try
            {
                await _petRepository.Delete(id);

                // Cria a mensagem JSON como string
                var message = new
                {
                    type = "pet_deleted",
                    data = new
                    {
                        PetId = id
                    }
                };

                string jsonString = JsonSerializer.Serialize(message);
                
                Console.WriteLine($"Mensagem enviada para a fila: {jsonString}");
                _rabbitMqService.SendMessage(jsonString, message.type);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Pet>> ListarTodosPets()
        {
            try
            {
                var pets = await _petRepository.ListarTodosPets();
                if (pets == null)
                {
                    throw new Exception("Nenhum pet encontrado");
                }
                return pets.Select(pet => new Pet
                {
                    Id = pet.Id,
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Tipo = pet.Tipo,
                    RacaId = pet.RacaId,
                    NomeRaca = pet.NomeRaca,
                    ImagemUrl = pet.ImagemUrl,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                }).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar pets no banco de dados: {e.Message}");
            }

        }

        public async Task<List<PetDTO>> ListarPetsPorEspecie(string especie)
        {
            try
            {
                var pets = await _petRepository.ListarPetsPorEspecie(especie);
                
                if (pets.Count == 0)
                {
                    throw new Exception("Nenhum pet encontrado");
                }

                return pets.Select(pet => new PetDTO
                {
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Especie = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                }).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar pets no banco de dados: {e.Message}");
            }
        }

        public async Task<List<PetDTO>> ListarPetsPorRaca(string raca)
        {
            try
            {
                var pets = await _petRepository.ListarPetsPorRaca(raca);
                if (pets.Count == 0)
                {
                    throw new Exception("Nenhum pet encontrado");
                }
                return pets.Select(pet => new PetDTO
                {
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Especie = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                }).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao buscar pets no banco de dados: {e.Message}");
            }
        }

        public async Task<PetDTO> BuscarPetPorId(int id)
        {
            try
            {
                var pet = await _petRepository.BuscarPetPorId(id);
                if (pet == null)
                {
                    throw new Exception("Pet não encontrado");
                }
                return new PetDTO
                {
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Especie = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public async Task<Pet> BuscarPetPorIdRabbitMq(string nomePet, string emailTutor)
        {
            try
            {
                var pet = await _petRepository.BuscarPetPorNomeETutor(nomePet, emailTutor);
                if (pet == null)
                {
                    throw new Exception("Pet não encontrado");
                }
                return new Pet
                {
                    Id = pet.Id,
                    Nome = pet.Nome,
                    Cor = pet.Cor,
                    Sexo = pet.Sexo,
                    Idade = pet.Idade,
                    Tipo = pet.Tipo,
                    NomeRaca = pet.NomeRaca,
                    Tutor = pet.Tutor,
                    EmailTutor = pet.EmailTutor
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}