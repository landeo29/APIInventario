﻿using APIInventario.Core.Models;

namespace APIInventario.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task<Usuario> ObtenerPorNombreAsync(string username);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task EliminarAsync(int id);
    }
}
