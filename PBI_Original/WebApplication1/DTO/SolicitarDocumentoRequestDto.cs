using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.DTO
{
    public enum TipoDocumento
    {
        DeclaracaoMatricula,
        HistoricoEscolar
    }
    public class SolicitarDocumentoRequestDto
    {
        /// <summary>
        /// Número de Registro do Aluno (RM).
        /// </summary>
        [Required(ErrorMessage = "O RM é obrigatório.")]
        [Range(1, long.MaxValue, ErrorMessage = "O RM deve ser um número maior que zero.")]
        public long Rm { get; set; }

        /// <summary>
        /// Tipo de documento solicitado.
        /// </summary>
        [Required(ErrorMessage = "O tipo de documento é obrigatório.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoDocumento TipoDocumento { get; set; }

        /// <summary>
        /// Descrição ou justificativa da solicitação.
        /// </summary>
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "A descrição não pode ser vazia ou conter apenas espaços.")]
        public string Descricao { get; set; } = string.Empty;
    }
}
