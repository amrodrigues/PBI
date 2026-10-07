using System.Text.Json.Serialization;
using WebApplication1.Enum;

namespace WebApplication1.DTO
{
    
    public class SolicitacaoDocumentoResponseDto
    {
        /// <summary>
        /// Identificador único gerado pela aplicação.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Número de Registro do Aluno.
        /// </summary>
        public long Rm { get; set; }

        /// <summary>
        /// Tipo de documento solicitado.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoDocumento TipoDocumento { get; set; }

        /// <summary>
        /// Descrição informada.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora da abertura geradas pela aplicação.
        /// </summary>
        public DateTime DataAbertura { get; set; }

        /// <summary>
        /// Situação da solicitação (definida como Aberta no momento da criação).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SituacaoSolicitacao Situacao { get; set; }
    }
}
