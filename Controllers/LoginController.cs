﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmallWarehouseBackEnd.Contexts;
using SmallWarehouseBackEnd.Models;

namespace SmallWarehouseBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger, AppDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(Usuario usuario)
        {
            try
            {
                // Verificar usuario y contrasenia.
                if (usuario.usuario_username != "katlheen" || usuario.usuario_password != "123")
                {
                    return BadRequest("Usuario o contrasenia incorrectos");
                }

                // Generar token
                var token = GenerarToken(usuario);
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception e)
            {
                _logger.LogError("Login: " + e.Message, e);
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("obtener")]
        [Authorize]
        public string obtener()
        {
            return "Hola";
        }

        private JwtSecurityToken GenerarToken(Usuario usuario)
        {
            string ValidIssuer = _configuration["ApiAuth:Issuer"];
            string ValidAudience = _configuration["ApiAuth:Audience"];
            SymmetricSecurityKey IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiAuth:SecretKey"]));

            //La fecha de expiracion sera el mismo dia a las 12 de la noche
            DateTime dtFechaExpiraToken;
            DateTime now = DateTime.Now;
            dtFechaExpiraToken = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);

            // CREAMOS LOS CLAIMS //
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.usuario_username),
                new Claim(JwtRegisteredClaimNames.Email, usuario.usuario_email),
            };


            return new JwtSecurityToken
            (
                issuer: ValidIssuer,
                audience: ValidAudience,
                claims: claims,
                expires: dtFechaExpiraToken,
                notBefore: now,
                signingCredentials: new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}