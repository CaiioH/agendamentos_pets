using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PetApi.DTOs;

namespace PetApi.Services
{
    public class RacaService
    {
        private readonly IHttpClientFactory _clientFactory;

        public RacaService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private HttpClient ObterClient(string tipoAnimal)
        {
            HttpClient client;

            if (tipoAnimal.ToUpper() == "CACHORRO")
            {
                client = _clientFactory.CreateClient("DogAPI");
            }
            else if (tipoAnimal.ToUpper() == "GATO")
            {
                client = _clientFactory.CreateClient("CatAPI");
            }
            else
            {
                throw new Exception("API não encontrada");
            }

            return client;
        }

        public async Task<List<RacaDTO>> ObterTodasRacas(string tipoAnimal)
        {
            try
            {
                var client = ObterClient(tipoAnimal); //Metodo privado que retorna um HttpClient de acordo com o tipo de animal

                var response = await client.GetAsync("breeds"); //Faz uma requisição HTTP para obter a lista de raças
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar raças: {response.StatusCode} - {errorMessage}");
                }

                //Usa JsonSerializer para converter o JSON em uma lista de RacaDTO.
                return JsonSerializer.Deserialize<List<RacaDTO>>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao obter todas as raças: {e.Message}");
            }
        }

        public async Task<string> ObterImagemUrl(string referenceImageId, string tipoAnimal)
        {
            try
            {
                var client = ObterClient(tipoAnimal); //Metodo privado que retorna um HttpClient de acordo com o tipo de animal
                var response = await client.GetAsync($"images/{referenceImageId}"); //Faz uma requisição HTTP para obter a URL da imagem
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar imagem: {response.StatusCode} - {errorMessage}");
                }

                var imagemData = await response.Content.ReadFromJsonAsync<ImagemDTO>(); //Usa ReadFromJsonAsync para converter o JSON em um objeto ImagemDTO
                return imagemData!.Url; //Retorna a URL da imagem
                                        // return JsonSerializer.Deserialize<ImagemDTO>(imagemData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!.Url; // Deserialize to ImagemDTO and return the URL
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao obter a URL da imagem: {e.Message}");
            }

        }

        public async Task<RacaDTO> ObterRacaPorNome(string nomeRaca, string tipoAnimal)
        {
            try
            {
                var client = ObterClient(tipoAnimal);
                var response = await client.GetAsync($"breeds/search?q={nomeRaca}"); //Faz uma requisição HTTP para buscar a raça pelo nome

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar raça: {response.StatusCode} - {errorMessage}");
                }

                // var raca = await response.Content.ReadFromJsonAsync<RacaDTO>(); //Usa ReadFromJsonAsync para converter o JSON em um objeto RacaDTO
                var json = await response.Content.ReadAsStringAsync();

                var racas = JsonSerializer.Deserialize<List<RacaDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }); //Retorna a lista de raças

                return racas?.FirstOrDefault(); // Retorna a primeira raça correspondente ou null se não encontrar nenhuma
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao obter a raça por nome: {e.Message}");
            }
        }



    }
}