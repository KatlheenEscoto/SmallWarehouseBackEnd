using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class PaginatedList
    {
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; } // Actual pagina.
        public int TotalPages { get; set; } // Total de paginas.

        public List<Item> Items { get; set; }

        // Constructor.
        public PaginatedList(List<Item> items, int count, int pageIndex, int pageSize)
        {
            Count = count;
            PageSize = pageSize;
            PageIndex = pageIndex; 
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); //Redondeo del tamaño de paginas: total de items / items especificados a mostrar.
            Items = items;
        }

        public static async Task<PaginatedList> CreateAsync(IQueryable<Item> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync(); // Numero de elementos.
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); // Solo contiene la pagina especificada con los elementos especificados. 
            List<Item> lista = new List<Item>();
            lista.AddRange(items);
            return new PaginatedList(lista, count, pageIndex, pageSize); // Retornar elementos.
        }


    }
}
