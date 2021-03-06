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
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            try
            {
                // Buscando usuario.
                var usuario_bd = _context.Usuario.Where(u => u.usuario_username == usuario.usuario_username).FirstOrDefault();
                var rol = await _context.Rol.FindAsync(usuario_bd.rol_id);

                if (usuario_bd == null)
                {
                    return Ok("El usuario no existe.");
                } else if (rol == null)
                {
                    return Ok("El usuario no tiene asignado ningun rol.");
                }

                // Verificar usuario y contrasenia.
                if (usuario.usuario_username != usuario_bd.usuario_username || usuario.usuario_password != usuario_bd.usuario_password)
                {
                    return BadRequest("Usuario o contrasenia incorrectos");
                } 



                // Generar token
                var token = GenerarToken(usuario_bd, rol);
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("obtener")]
        [Authorize(Roles = "admin")]
        public string obtener()
        {
            return "Prueba";
        }

        [HttpPost("welcome")]
        [Authorize]
        public string welcome()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var username = claim[1].Value;
            return "Welcome " + username;
        }


        private JwtSecurityToken GenerarToken(Usuario usuario, Rol rol)
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
                new Claim (ClaimTypes.Role, rol.rol_name)
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