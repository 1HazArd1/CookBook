﻿using CookBook.Application.Auth;
using CookBook.Application.Common.Encryptor;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CookBook.Application
{
    public static class ConfigurationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped(typeof(ICryptor), typeof(Cryptor));
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            return services;
        }
    }
}
