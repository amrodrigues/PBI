using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Enum;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/solicitacoes-documentos")]
    public class SolicitacoesController : ControllerBase
    {
        // Simulação de banco de dados em memória para teste
        private static readonly List<SolicitacaoDocumentoResponseDto> SolicitacoesDb = new();

        /// <summary>
        /// Abre uma nova solicitação de documento acadêmico.
        /// </summary>
        /// <param name="request">Dados da solicitação.</param>
        /// <returns>A solicitação criada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SolicitacaoDocumentoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SolicitacaoDocumentoResponseDto> AbrirSolicitacao([FromBody] SolicitarDocumentoRequestDto request)
        {
            // CA02: O ASP.NET Core valida os DataAnnotations do DTO ([Required], [Range], [RegularExpression]) 
            // e retorna automaticamente HTTP 400 Bad Request informando os motivos caso haja erros.

            // CA03 & CA07: Impedir duas solicitações Abertas do mesmo tipo para o mesmo RM.
            // Permite tipos diferentes para o mesmo RM, ou o mesmo tipo se a anterior foi Cancelada.
            bool possuiSolicitacaoAbertaDoMesmoTipo = SolicitacoesDb.Any(s =>
                s.Rm == request.Rm &&
                s.TipoDocumento == request.TipoDocumento &&
                s.Situacao == SituacaoSolicitacao.Aberta
            );

            if (possuiSolicitacaoAbertaDoMesmoTipo)
            {
                return Conflict(new
                {
                    motivo = $"Já existe uma solicitação Aberta do tipo '{request.TipoDocumento}' para o RM {request.Rm}."
                });
            }
            var novaSolicitacao = new SolicitacaoDocumentoResponseDto
            {
                Id = Guid.NewGuid(),
                Rm = request.Rm,
                TipoDocumento = request.TipoDocumento,
                Descricao = request.Descricao.Trim(),
                DataAbertura = DateTime.UtcNow,
                Situacao = SituacaoSolicitacao.Aberta
            };

            SolicitacoesDb.Add(novaSolicitacao);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = novaSolicitacao.Id },
                novaSolicitacao
            );
        }

        /// <summary>
        /// CA04 - Consulta todas as solicitações de um aluno pelo RM (Abertas e Canceladas)
        /// </summary>
        [HttpGet("aluno/{rm:int}")]
        [ProducesResponseType(typeof(IEnumerable<SolicitacaoDocumentoResponseDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SolicitacaoDocumentoResponseDto>> ConsultarPorRm([FromRoute] int rm)
        {
            // CA04: Retorna somente as solicitações do aluno (Abertas e Canceladas) com todos os campos.
            // Sem registros, retorna lista vazia (HTTP 200 com []).
            var solicitacoesDoAluno = SolicitacoesDb
                .Where(s => s.Rm == rm)
                .ToList();

            return Ok(solicitacoesDoAluno);
        }

        /// <summary>
        /// Consulta uma solicitação específica pelo seu ID (GUID).
        /// </summary>
        /// <param name="id">Identificador único da solicitação.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SolicitacaoDocumentoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SolicitacaoDocumentoResponseDto> ObterPorId([FromRoute] Guid id)
        {
            var solicitacao = SolicitacoesDb.FirstOrDefault(s => s.Id == id);

            if (solicitacao is null)
            {
                return NotFound(new { mensagem = $"Solicitação com ID '{id}' não foi encontrada." });
            }

            return Ok(solicitacao);
        }

        /// <summary>
        /// Consulta todas as solicitações ou filtra por RM do aluno.
        /// </summary>
        /// <param name="rm">Número do RM do aluno (opcional).</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SolicitacaoDocumentoResponseDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SolicitacaoDocumentoResponseDto>> Consultar([FromQuery] int? rm)
        {
            var query = SolicitacoesDb.AsEnumerable();

            if (rm.HasValue && rm.Value > 0)
            {
                query = query.Where(s => s.Rm == rm.Value);
            }

            return Ok(query.ToList());
        }

        /// <summary>
        /// Cancela uma solicitação existente caso ela ainda esteja aberta.
        /// </summary>
        /// <param name="id">Identificador único da solicitação a ser cancelada.</param>
        [HttpPatch("{id:guid}/cancelar")]
        [ProducesResponseType(typeof(SolicitacaoDocumentoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SolicitacaoDocumentoResponseDto> CancelarSolicitacao([FromRoute] Guid id)
        {
            var solicitacao = SolicitacoesDb.FirstOrDefault(s => s.Id == id);

            if (solicitacao is null)
            {
                return NotFound(new { mensagem = $"Solicitação com ID '{id}' não foi encontrada." });
            }

            if (solicitacao.Situacao == SituacaoSolicitacao.Cancelada)
            {
                return BadRequest(new { mensagem = "A solicitação já está cancelada." });
            }

            if (solicitacao.Situacao == SituacaoSolicitacao.Concluida)
            {
                return BadRequest(new { mensagem = "Não é possível cancelar uma solicitação já concluída." });
            }

            solicitacao.Situacao = SituacaoSolicitacao.Cancelada;

            return Ok(solicitacao);
        }
    }
}
