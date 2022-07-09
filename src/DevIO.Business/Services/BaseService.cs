using DevIO.Business.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using DevIO.Business.Models;
using FluentValidation;

namespace DevIO.Business.Services
{
    public abstract class BaseService
    {
        public readonly INotificador _notificador;

        public BaseService(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string mensagem)
        {
            _notificador.Handle(new Notificacoes.Notificacao(mensagem));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);
            
            if (validator.IsValid) return true;

            Notificar(validator);
            return false;
        }
    }
}
