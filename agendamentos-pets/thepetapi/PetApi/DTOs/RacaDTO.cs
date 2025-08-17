using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetApi.DTOs
{
    public class RacaDTO
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntToStringConverter))]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("reference_image_id")]
        public string? ReferenceImageId { get; set; }
    }

    //Aqui serve pra converter o Id do tipo int pra string (TheDogApi utiliza int)
    internal class IntToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Se for número, converte para string
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32().ToString();
            }
            // Se for string, retorna normalmente
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            throw new JsonException("Formato inesperado para ID.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}