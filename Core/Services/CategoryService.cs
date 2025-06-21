using AutoMapper;
using Core.Interfaces;
using Core.Models.Category;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CategoryService(AppDbContext context,
    IMapper mapper, IImageService imageService) : ICategoryService
    {
        public async Task<CategoryItemModel> Create(CategoryCreateModel model)
        {
            var entity = mapper.Map<CategoryEntity>(model);
            entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
            await context.Categories.AddAsync(entity);
            await context.SaveChangesAsync();
            var item = mapper.Map<CategoryItemModel>(entity);
            return item;
        }

        public async Task Delete(CategoryDeleteModel model)
        {
            var entity = await context.Categories.SingleOrDefaultAsync(x => x.Id == model.Id);
            entity!.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<CategoryItemModel> Edit(CategoryEditModel model)
        {
            var existing = await context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
  
            existing = mapper.Map(model, existing);

            if (model.ImageFile != null)
            {
                await imageService.DeleteImageAsync(existing.Image);
                existing.Image = await imageService.SaveImageAsync(model.ImageFile);
            }
            await context.SaveChangesAsync();
            var item = mapper.Map<CategoryItemModel>(existing);
            return item;
        }

        public async Task<CategoryItemModel> GetItemById(int id)
        {
            var model = await mapper
            .ProjectTo<CategoryItemModel>(context.Categories.Where(x => x.Id == id))
            .SingleOrDefaultAsync();
            return model;
        }

        public async Task<List<CategoryItemModel>> List()
        {
            var model = await mapper.ProjectTo<CategoryItemModel>(context.Categories.Where(x=>x.IsDeleted == false).OrderBy(x => x.Id)).ToListAsync();
            return model;
        }
    }
}
