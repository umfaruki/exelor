﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exelor.Dto;
using Exelor.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exelor.Features.Users
{
    public class UpdateUser
    {
        public class Command : IRequest<UserDetailsDto>
        {
            public Command(
                int id,
                string firstName,
                string lastName)
            {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
            }

            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotNull().NotEmpty();
                RuleFor(x => x.FirstName).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, UserDetailsDto>
        {
            private readonly ApplicationDbContext _dbContext;

            public Handler(
                ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<UserDetailsDto> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                    .FirstAsync(
                    x => x.Id == request.Id,
                    cancellationToken);
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new UserDetailsDto(
                    user.Id,
                    user.FirstName,
                    user.LastName, 
                    user.FullName,
                    user.Email,
                    string.Join(", ", user.Roles.Select(r => r.Role.Name)));
            }
        }
    }
}