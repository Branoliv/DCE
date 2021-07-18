using DCE.Data.Interface;
using DCE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCE.Data
{
    public class DocumentRepository : IDocumentRepository
    {

        public async Task<bool> AddAsync(Document obj)
        {
            var response = 0;
            using (var _context = new DCEContext())
            {
                await _context.Documents
                 .AddAsync(obj);
                response = await _context.SaveChangesAsync();

            }

            if (response > 0)
                return true;
            else
                return false;
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            using (var _context = new DCEContext())
            {
                var doc = await _context.Documents
                   .AddAsync(document);
                await _context.SaveChangesAsync();
                return doc.Entity;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var _context = new DCEContext())
            {
                var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
                _context.Documents.Remove(doc);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }
        }

        public async Task<bool> ExistingContainerAndControlRegister(string containerNumber, string controlNumber)
        {
            using (var _context = new DCEContext())
            {
                var doc = await _context.Documents.FirstOrDefaultAsync(d => d.ContainerNumber.Equals(containerNumber) && d.ControlNumber.Equals(controlNumber));

                if (doc == null)
                    return false;
                else
                    return true;
            }
        }

        public async Task<Document> GetByIdAsync(Guid id)
        {
            using (var _context = new DCEContext())
            {
                return await _context.Documents
                    .Include(p => p.Photos)
                    .Where(d => d.Id == id)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Document> GetDocumentByContainerNumberAndControlNumber(string containerNumber, string controlNumber)
        {
            using (var _context = new DCEContext())
            {
                return await _context.Documents.FirstOrDefaultAsync(d => d.ContainerNumber.Equals(containerNumber) && d.ControlNumber.Equals(controlNumber));
            }
        }

        public async Task<IEnumerable<Document>> ListAsync()
        {
            using (var _context = new DCEContext())
            {
                return await _context.Documents
                .ToListAsync();
            }
        }

        public async Task<bool> UpdateAsync(Document obj)
        {
            using (var _context = new DCEContext())
            {
                var doc = _context.Documents.Update(obj);
                await _context.SaveChangesAsync();
                if (doc == null)
                    return false;
                else
                    return true;
            }
        }
    }
}
