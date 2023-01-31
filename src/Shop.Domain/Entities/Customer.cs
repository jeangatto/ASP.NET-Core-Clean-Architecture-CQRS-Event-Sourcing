using System;
using Shop.Core.Abstractions;
using Shop.Core.Interfaces;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities;

/// <summary>
/// Cliente
/// </summary>
public class Customer : BaseAuditEntity, IAggregateRoot
{
    /// <summary>
    /// Inicializa uma nova inståncia do cliente.
    /// </summary>
    /// <param name="firstName">Primeiro Nome.</param>
    /// <param name="lastName">Sobrenome.</param>
    /// <param name="gender">Gênero.</param>
    /// <param name="email">Endereço de e-mail.</param>
    /// <param name="dateOfBirth">Data de Nascimento.</param>
    public Customer(string firstName, string lastName, EGender gender, string email, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        Email = email;
        DateOfBirth = dateOfBirth;
    }

    private Customer() { }

    /// <summary>
    /// Primeiro Nome.
    /// </summary>
    public string FirstName { get; private init; }

    /// <summary>
    /// Sobrenome.
    /// </summary>
    public string LastName { get; private init; }

    /// <summary>
    /// Gênero.
    /// </summary>
    public EGender Gender { get; private init; }

    /// <summary>
    /// Endereço de e-mail.
    /// </summary>
    public string Email { get; private init; }

    /// <summary>
    /// Data de Nascimento.
    /// </summary>
    public DateTime DateOfBirth { get; private init; }
}