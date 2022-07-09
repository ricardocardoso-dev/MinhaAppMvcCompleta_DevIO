using System.Threading.Tasks;
using System;
using DevIO.Business.Models;

namespace DevIO.Business.Services
{
    public interface IProdutoService : IDisposable
    {
        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);
    }
}