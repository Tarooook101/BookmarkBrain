using BookMarkBrain.Core.DTOs.Category;
using BookMarkBrain.Core.Entities;
using Mapster;


namespace BookMarkBrain.Services.Mappings;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Entity to DTO
        config.NewConfig<Category, CategoryDto>()
            .Map(dest => dest.ChildCategoryIds, src => src.ChildCategories.Select(c => c.Id).ToList());

        // DTO to Entity
        config.NewConfig<CreateCategoryDto, Category>();
        config.NewConfig<UpdateCategoryDto, Category>();

        // Entity to TreeDTO (for hierarchical display)
        config.NewConfig<Category, CategoryTreeDto>()
            .Map(dest => dest.Children, src => src.ChildCategories);
    }
}