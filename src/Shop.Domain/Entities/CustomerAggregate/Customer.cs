using System;
using Shop.Core.Domain;
using Shop.Core.ValueObjects;
using Shop.Domain.Entities.CustomerAggregate.Events;

namespace Shop.Domain.Entities.CustomerAggregate;

/// <summary>
/// Entidade Cliente.
/// </summary>
public class Customer : Entity, IAggregateRoot
{
    /// <summary>
    /// Inicializa uma inståncia de um novo cliente.
    /// </summary>
    /// <param name="firstName">Primeiro Nome.</param>
    /// <param name="lastName">Sobrenome.</param>
    /// <param name="gender">Gênero.</param>
    /// <param name="email">Endereço de e-mail.</param>
    /// <param name="dateOfBirth">Data de Nascimento.</param>
    public Customer(string firstName, string lastName, EGender gender, Email email, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;

        // Adicionando a nova instancia nos eventos de domínio.
        AddDomainEvent(new CustomerCreatedEvent(Id, firstName, lastName, gender, email.Address, dateOfBirth));
    }

    private Customer() { } // ORM

    /// <summary>
    /// Primeiro Nome.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Sobrenome.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Gênero.
    /// </summary>
    public EGender Gender { get; }

    /// <summary>
    /// Endereço de e-mail.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Data de Nascimento.
    /// </summary>
    public DateTime DateOfBirth { get; }

    /// <summary>
    /// Altera o endereço de e-mail atual.
    /// </summary>
    /// <param name="newEmail">Novo endereço de e-mail.</param>
    public void ChangeEmail(Email newEmail)
    {
        // Só será alterado o e-mail se for diferente do existente.
        if (Email.Equals(newEmail))
            return;

        Email = newEmail;

        // Adicionando a alteração nos eventos de domínio.
        AddDomainEvent(new CustomerUpdatedEvent(Id, FirstName, LastName, Gender, newEmail.Address, DateOfBirth));
    }

    /// <summary>
    /// Adiciona o evento de entidade deletada.
    /// </summary>
    public void Delete() =>
        AddDomainEvent(new CustomerDeletedEvent(Id, FirstName, LastName, Gender, Email.Address, DateOfBirth));
}