using System.ComponentModel.DataAnnotations;
using WebApplication1.Enum;

namespace WebApplication1.Model
{
   
    public class SolicitacaoDocumento
    {
        /// <summary>
        /// Identificador único da solicitação (Chave Primária).
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Número do Registro do Aluno (RM).
        /// </summary>
        [Required]
        public long Rm { get; set; }

        /// <summary>
        /// Tipo do documento solicitado (Enum armazenado como string ou int no banco).
        /// </summary>
        [Required]
        public TipoDocumento TipoDocumento { get; set; }

        /// <summary>
        /// Descrição/detalhes da solicitação.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora da criação/abertura da solicitação.
        /// </summary>
        [Required]
        public DateTime DataAbertura { get; set; }

        /// <summary>
        /// Situação atual da solicitação.
        /// </summary>
        [Required]
        public SituacaoSolicitacao Situacao { get; set; }

        /// <summary>
        /// Construtor padrão da entidade que garante as regras iniciais.
        /// </summary>
        public SolicitacaoDocumento()
        {
            Id = Guid.NewGuid();
            DataAbertura = DateTime.UtcNow;
            Situacao = SituacaoSolicitacao.Aberta;
        }

        /// <summary>
        /// Construtor conveniente para criação da solicitação com os parâmetros iniciais.
        /// </summary>
        public SolicitacaoDocumento(int rm, TipoDocumento tipoDocumento, string descricao) : this()
        {
            Rm = rm;
            TipoDocumento = tipoDocumento;
            Descricao = descricao;
        }
    }
}
