
using Core.Models.Category;

namespace Core.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryItemModel>> List();
    Task<CategoryItemModel> GetItemById(int id);
    Task<CategoryItemModel> Create(CategoryCreateModel model);
    Task<CategoryItemModel> Edit(CategoryEditModel model);
    Task Delete(long id);
}
