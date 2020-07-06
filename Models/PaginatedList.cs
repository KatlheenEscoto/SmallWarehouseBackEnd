using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class PaginatedList<T>: List<T>
    {
        public int PageIndex { get; set; } // Actual pagina.
        public int TotalPages { get; set; } // Total de paginas.

        // Constructor.
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex; 
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); //Redondeo del tamaño de paginas: total de items / items especificados a mostrar.
            this.AddRange(items); // Agregar elementos de una coleccion especifica al final de la lista.
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync(); // Numero de elementos.
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); // Solo contiene la pagina especificada con los elementos especificados. 
            return new PaginatedList<T>(items, count, pageIndex, pageSize); // Retornar elementos.
        }
    }
}
