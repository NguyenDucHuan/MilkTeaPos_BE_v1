using MilkTeaPosManagement.Api.Constants;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.ProductModel;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Extensions.Filter;
using AutoMapper;
using System.Linq;
using System;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, IMapper mapper)
        {
            _uow = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }
        public async Task<MethodResult<List<Product>>> CreateProductWithSizesAsync(int userId, CreateProductParentRequest parentRequest, List<ProductSizeRequest> sizes)
        {
            try
            {
                var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                    predicate: c => c.CategoryId == parentRequest.CategoryId
                );

                if (category == null)
                {
                    return new MethodResult<List<Product>>.Failure(
                        "Category not found",
                        StatusCodes.Status400BadRequest
                    );
                }

                await _uow.BeginTransactionAsync();

                string parentImageUrl = null;
                if (parentRequest.ParentImage != null)
                {
                    parentImageUrl = await _cloudinaryService.UploadImageAsync(parentRequest.ParentImage);
                    if (string.IsNullOrEmpty(parentImageUrl))
                    {
                        return new MethodResult<List<Product>>.Failure(
                            "Failed to upload parent product image",
                            StatusCodes.Status500InternalServerError
                        );
                    }
                }
                var parentProduct = new Product
                {
                    ProductName = parentRequest.ProductName,
                    CategoryId = parentRequest.CategoryId,
                    Description = parentRequest.Description,
                    ImageUrl = parentImageUrl,
                    Prize = null,
                    ProductType = ProductConstant.PRODUCT_TYPE_MATTER_PRODUCT,
                    ParentId = null,
                    SizeId = ProductConstant.PRODUCT_SIZE_PARENT,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    UpdateAt = DateTime.Now,
                    UpdateBy = userId,
                    Status = parentRequest.Status
                };

                await _uow.GetRepository<Product>().InsertAsync(parentProduct);
                await _uow.CommitAsync();

                var createdParentProduct = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductName == parentRequest.ProductName &&
                                   p.ProductType == ProductConstant.PRODUCT_TYPE_MATTER_PRODUCT &&
                                   p.SizeId == ProductConstant.PRODUCT_SIZE_PARENT,
                    orderBy: q => q.OrderByDescending(p => p.CreateAt)
                );

                if (createdParentProduct == null)
                {
                    await _uow.RollbackTransactionAsync();
                    return new MethodResult<List<Product>>.Failure(
                        "Failed to create parent product",
                        StatusCodes.Status500InternalServerError
                    );
                }

                var createdProducts = new List<Product> { createdParentProduct };

                foreach (var sizeRequest in sizes)
                {

                    var sizeProduct = new Product
                    {
                        ProductName = parentRequest.ProductName,
                        CategoryId = parentRequest.CategoryId,
                        Description = parentRequest.Description,
                        ImageUrl = parentImageUrl,
                        Prize = sizeRequest.Price,
                        ProductType = ProductConstant.PRODUCT_TYPE_SINGLE_PRODUCT,
                        ParentId = createdParentProduct.ProductId,
                        SizeId = sizeRequest.Size,
                        CreateAt = DateTime.Now,
                        CreateBy = userId,
                        UpdateAt = DateTime.Now,
                        UpdateBy = userId,
                        Status = parentRequest.Status
                    };

                    await _uow.GetRepository<Product>().InsertAsync(sizeProduct);
                    await _uow.CommitAsync();
                    createdProducts.Add(sizeProduct);
                }

                await _uow.CommitTransactionAsync();
                return new MethodResult<List<Product>>.Success(createdProducts);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return new MethodResult<List<Product>>.Failure(
                    $"Error creating product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<MethodResult<Product>> CreateExtraProductAsync(int userId, CreateExtraProductRequest request)
        {
            try
            {
                var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                       predicate: c => c.CategoryName == "Topping"
                   );
                if (category == null)
                {
                    return new MethodResult<Product>.Failure(
                        "Category 'Topping' not found",
                        StatusCodes.Status400BadRequest
                    );
                }
                string? imageUrl = null;
                if (request.Image != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new MethodResult<Product>.Failure(
                            "Failed to upload image",
                            StatusCodes.Status500InternalServerError
                        );
                    }
                }

                var extraProduct = new Product
                {
                    ProductName = request.ProductName,
                    CategoryId = category.CategoryId,
                    Description = request.Description,
                    ImageUrl = imageUrl,
                    Prize = request.Price,
                    ProductType = ProductConstant.PRODUCT_TYPE_EXTRA_PRODUCT,
                    ParentId = null,
                    SizeId = ProductConstant.PRODUCT_SIZE_PARENT,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    UpdateAt = DateTime.Now,
                    UpdateBy = userId,
                    Status = request.Status ?? ProductConstant.PRODUCT_STATUS_ACTIVE
                };

                await _uow.GetRepository<Product>().InsertAsync(extraProduct);
                await _uow.CommitAsync();

                return new MethodResult<Product>.Success(extraProduct);
            }
            catch (Exception ex)
            {
                return new MethodResult<Product>.Failure(
                    $"Error creating extra product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<Product>> CreateComboAsync(int userId, CreateComboRequest request)
        {
            try
            {
                var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                       predicate: c => c.CategoryName == "Combo"
                   );
                if (category == null)
                {
                    return new MethodResult<Product>.Failure(
                        "Category 'Combo' not found",
                        StatusCodes.Status400BadRequest
                    );
                }

                string? imageUrl = null;
                if (request.Image != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new MethodResult<Product>.Failure(
                            "Failed to upload image",
                            StatusCodes.Status500InternalServerError
                        );
                    }
                }

                await _uow.BeginTransactionAsync();
                var comboProduct = new Product
                {
                    ProductName = request.ComboName,
                    Description = request.Description,
                    CategoryId = category.CategoryId,
                    ImageUrl = imageUrl,
                    Prize = request.Price,
                    ProductType = ProductConstant.PRODUCT_TYPE_COMBO,
                    ParentId = null,
                    SizeId = ProductConstant.PRODUCT_SIZE_PARENT,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    UpdateAt = DateTime.Now,
                    UpdateBy = userId,
                    Status = request.Status ?? ProductConstant.PRODUCT_STATUS_ACTIVE
                };

                await _uow.GetRepository<Product>().InsertAsync(comboProduct);
                await _uow.CommitAsync();

                foreach (var item in request.ComboItems)
                {
                    var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                        predicate: p => p.ProductId == item.ProductId
                    );

                    if (product == null)
                    {
                        await _uow.RollbackTransactionAsync();
                        return new MethodResult<Product>.Failure(
                            $"Product with ID {item.ProductId} not found",
                            StatusCodes.Status400BadRequest
                        );
                    }

                    var comboItem = new Comboltem
                    {
                        Combod = comboProduct.ProductId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Discount = item.Discount,
                        MasterId = null
                    };

                    await _uow.GetRepository<Comboltem>().InsertAsync(comboItem);

                    if (item.ExtraItems != null && item.ExtraItems.Count > 0)
                    {
                        foreach (var extraItem in item.ExtraItems)
                        {
                            var extraProduct = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                                predicate: p => p.ProductId == extraItem.ExtraProductId &&
                                              p.ProductType == ProductConstant.PRODUCT_TYPE_EXTRA_PRODUCT
                            );

                            if (extraProduct == null)
                            {
                                await _uow.RollbackTransactionAsync();
                                return new MethodResult<Product>.Failure(
                                    $"Extra product with ID {extraItem.ExtraProductId} not found or is not an extra product",
                                    StatusCodes.Status400BadRequest
                                );
                            }

                            var extraComboItem = new Comboltem
                            {
                                Combod = comboProduct.ProductId,
                                ProductId = extraItem.ExtraProductId,
                                Quantity = extraItem.Quantity,
                                Discount = extraItem.Discount,
                                MasterId = item.ProductId
                            };

                            await _uow.GetRepository<Comboltem>().InsertAsync(extraComboItem);
                        }
                    }
                }
                await _uow.CommitAsync();
                await _uow.CommitTransactionAsync();
                var createdCombo = await _uow.GetRepository<Product>()
                    .SingleOrDefaultAsync(
                        predicate: p => p.ProductId == comboProduct.ProductId,
                        include: q => q.Include(p => p.Comboltems)
                    );

                return new MethodResult<Product>.Success(createdCombo);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return new MethodResult<Product>.Failure(
                    $"Error creating combo: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<PaginatedResponse<ProductResponse>>> GetFilteredProductsAsync(ProductFilterModel filter)
        {
            try
            {
                if (filter == null)
                    filter = new ProductFilterModel();
                var paginatedProducts = await _uow.GetRepository<Product>().GetPagingListAsync(
                    selector: product => _mapper.Map<ProductResponse>(product),
                    predicate: filter.BuildProductFilterExpression(),
                    orderBy: query => query.ApplyProductSorting(filter.SortBy, filter.SortAscending),
                    include: q => q.Include(p => p.Category),
                    page: filter.Page,
                    size: filter.PageSize
                );

                await EnrichProductDetails(paginatedProducts.Items.ToList());
                var result = new PaginatedResponse<ProductResponse>
                {
                    Items = paginatedProducts.Items.ToList(),
                    TotalCount = paginatedProducts.TotalPages,
                    PageSize = filter.PageSize,
                    CurrentPage = filter.Page,
                    TotalPages = paginatedProducts.TotalPages
                };

                return new MethodResult<PaginatedResponse<ProductResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return new MethodResult<PaginatedResponse<ProductResponse>>.Failure(
                    $"Error retrieving products: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        private async Task EnrichProductDetails(List<ProductResponse> products)
        {
            var masterProducts = products
                .Where(p => p.ProductType == ProductConstant.PRODUCT_TYPE_MATTER_PRODUCT)
                .ToList();

            var comboProducts = products
                .Where(p => p.ProductType == ProductConstant.PRODUCT_TYPE_COMBO)
                .ToList();

            if (masterProducts.Any())
            {
                var masterIds = masterProducts.Select(p => p.ProductId).ToList();

                var sizeProducts = await _uow.GetRepository<Product>().GetListAsync(
                    selector: p => _mapper.Map<ProductResponse>(p),
                    predicate: p => p.ParentId.HasValue && masterIds.Contains(p.ParentId.Value),
                    include: q => q.Include(p => p.Category)
                );

                foreach (var master in masterProducts)
                {
                    master.Variants = sizeProducts
                        .Where(s => s.ParentId == master.ProductId)
                        .ToList();
                }
            }
            if (comboProducts.Any())
            {
                var comboIds = comboProducts.Select(p => p.ProductId).ToList();

                var allComboItems = await _uow.GetRepository<Comboltem>().GetListAsync(
                    selector: c => _mapper.Map<ComboItemResponse>(c),
                    predicate: c => c.Combod.HasValue && comboIds.Contains(c.Combod.Value),
                    include: q => q.Include(c => c.Product)
                );

                foreach (var combo in comboProducts)
                {
                    var mainItems = allComboItems
                        .Where(c => (c.Combod == combo.ProductId && c.MasterId == null) || (c.Combod == combo.ProductId && c.MasterId == 0))
                        .ToList();

                    foreach (var mainItem in mainItems)
                    {
                        mainItem.ExtraItems = allComboItems
                            .Where(c => c.Combod == combo.ProductId && c.MasterId == mainItem.ProductId)
                            .ToList();

                        combo.ComboItems.Add(mainItem);
                    }
                }
            }
        }
        public async Task<MethodResult<ProductResponse>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductId == id,
                    include: q => q.Include(p => p.Category)
                );

                if (product == null)
                {
                    return new MethodResult<ProductResponse>.Failure(
                        "Product not found",
                        StatusCodes.Status404NotFound
                    );
                }

                var productResponse = _mapper.Map<ProductResponse>(product);
                await EnrichProductDetails(new List<ProductResponse> { productResponse });

                return new MethodResult<ProductResponse>.Success(productResponse);
            }
            catch (Exception ex)
            {
                return new MethodResult<ProductResponse>.Failure(
                    $"Error retrieving product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<ProductResponse>> UpdateMasterProductAsync(int userId, UpdateMasterProductRequest request , List<UpdateSizeProductRequest> Variants)
        {
            try
            {
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductId == request.ProductId && p.ParentId == null && p.ProductType != "Combo" && p.ProductType != "Extra",
                    include: q => q.Include(p => p.Category)
                );

                if (product == null)
                {
                    return new MethodResult<ProductResponse>.Failure(
                        "Master product not found",
                        StatusCodes.Status404NotFound
                    );
                }
                await _uow.BeginTransactionAsync();

                if (request.CategoryId.HasValue && request.CategoryId != product.CategoryId)
                {
                    var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                        predicate: c => c.CategoryId == request.CategoryId
                    );
                    if (category == null)
                    {
                        return new MethodResult<ProductResponse>.Failure(
                            "Category not found",
                            StatusCodes.Status404NotFound
                        );
                    }
                    product.CategoryId = request.CategoryId;
                }
                if (!string.IsNullOrEmpty(request.ProductName))
                {
                    product.ProductName = request.ProductName;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    product.Description = request.Description;
                }
                if (request.Status.HasValue)
                {
                    product.Status = request.Status;
                }

                if (request.Image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);

                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new MethodResult<ProductResponse>.Failure(
                            "Failed to upload product image",
                            StatusCodes.Status500InternalServerError
                        );
                    }

                    product.ImageUrl = imageUrl;
                }
                product.UpdateAt = DateTime.Now;
                product.UpdateBy = userId;

                _uow.GetRepository<Product>().UpdateAsync(product);
                await _uow.CommitAsync();

                if (Variants != null && Variants.Any())
                {
                    foreach (var variant in Variants)
                    {
                        var sizeProduct = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                            predicate: p => p.SizeId == variant.SizeId && p.ParentId == product.ProductId
                        );

                        if (sizeProduct != null)
                        {
                            if (variant.Image != null)
                            {
                                var imageUrl = await _cloudinaryService.UploadImageAsync(variant.Image);
                                if (string.IsNullOrEmpty(imageUrl))
                                {
                                    return new MethodResult<ProductResponse>.Failure(
                                        "Failed to upload product image",
                                        StatusCodes.Status500InternalServerError
                                    );
                                }
                                sizeProduct.ImageUrl = imageUrl;
                            }
                            if (variant.Prize.HasValue)
                                sizeProduct.Prize = variant.Prize;

                            if (!string.IsNullOrEmpty(variant.SizeId))
                                sizeProduct.SizeId = variant.SizeId;

                            if (variant.Status.HasValue)
                                sizeProduct.Status = variant.Status;

                            sizeProduct.UpdateAt = DateTime.Now;
                            sizeProduct.UpdateBy = userId;

                            _uow.GetRepository<Product>().UpdateAsync(sizeProduct);
                            await _uow.CommitAsync();
                        }
                        else
                        {
                            var imageUrl = product.ImageUrl;
                            if (variant.Image != null)
                            {
                                var imageUrlu = await _cloudinaryService.UploadImageAsync(variant.Image);
                                if (string.IsNullOrEmpty(imageUrl))
                                {
                                    return new MethodResult<ProductResponse>.Failure(
                                        "Failed to upload product image",
                                        StatusCodes.Status500InternalServerError
                                    );
                                }
                                imageUrl = imageUrlu;
                            }
                            var newSizeProduct = new Product
                            {
                                ProductName = product.ProductName,
                                CategoryId = product.CategoryId,
                                Description = product.Description,
                                ImageUrl = product.ImageUrl,
                                Prize = variant.Prize,
                                ProductType = ProductConstant.PRODUCT_TYPE_SINGLE_PRODUCT,
                                ParentId = product.ProductId,
                                SizeId = variant.SizeId,
                                CreateAt = DateTime.Now,
                                CreateBy = userId,
                                UpdateAt = DateTime.Now,
                                UpdateBy = userId,
                                Status = variant.Status ?? ProductConstant.PRODUCT_STATUS_ACTIVE
                            };
                            await _uow.GetRepository<Product>().InsertAsync(newSizeProduct);
                            await _uow.CommitAsync();
                        }
                    }
                }

                await _uow.CommitAsync();
                var productResponse = _mapper.Map<ProductResponse>(product);
                await EnrichProductDetails(new List<ProductResponse> { productResponse });

                await _uow.CommitTransactionAsync();
                return new MethodResult<ProductResponse>.Success(productResponse);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return new MethodResult<ProductResponse>.Failure(
                    $"Error updating master product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }


        public async Task<MethodResult<ProductResponse>> UpdateExtraProductAsync(int userId, UpdateExtraProductRequest request)
        {
            try
            {
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductId == request.ProductId && p.ProductType == "Extra",
                    include: q => q.Include(p => p.Category)
                );
                if (product == null)
                {
                    return new MethodResult<ProductResponse>.Failure(
                        "Extra product not found",
                        StatusCodes.Status404NotFound
                    );
                }
                if (request.CategoryId.HasValue && request.CategoryId != product.CategoryId)
                {
                    var category = await _uow.GetRepository<Category>().SingleOrDefaultAsync(
                       predicate: c => c.CategoryId == request.CategoryId
                    );
                    if (category == null)
                    {
                        return new MethodResult<ProductResponse>.Failure(
                            "Category not found",
                            StatusCodes.Status404NotFound
                        );
                    }

                    product.CategoryId = request.CategoryId;
                }
                if (request.Image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);

                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new MethodResult<ProductResponse>.Failure(
                            "Failed to upload product image",
                            StatusCodes.Status500InternalServerError
                        );
                    }

                    product.ImageUrl = imageUrl;
                }
                if (!string.IsNullOrEmpty(request.ProductName))
                    product.ProductName = request.ProductName;

                if (!string.IsNullOrEmpty(request.Description))
                    product.Description = request.Description;

                if (request.Prize.HasValue)
                    product.Prize = request.Prize;

                if (request.Status.HasValue)
                    product.Status = request.Status;


                product.UpdateAt = DateTime.Now;
                product.UpdateBy = userId;
                _uow.GetRepository<Product>().UpdateAsync(product);
                await _uow.CommitAsync();
                var productResponse = _mapper.Map<ProductResponse>(product);
                await EnrichProductDetails(new List<ProductResponse> { productResponse });

                return new MethodResult<ProductResponse>.Success(productResponse);
            }
            catch (Exception ex)
            {
                return new MethodResult<ProductResponse>.Failure(
                    $"Error updating extra product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<MethodResult<ProductResponse>> UpdateComboProductAsync(int userId, UpdateComboProductRequest request)
        {
            try
            {
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductId == request.ProductId && p.ProductType == "Combo",
                    include: q => q.Include(p => p.Category));

                var comboItems = await _uow.GetRepository<Comboltem>().GetListAsync(
                    predicate: c => c.Combod == product.ProductId
                );
                if (product == null)
                {
                    return new MethodResult<ProductResponse>.Failure(
                        "Combo product not found",
                        StatusCodes.Status404NotFound
                    );
                }
                await _uow.BeginTransactionAsync();
                if (request.Image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);

                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new MethodResult<ProductResponse>.Failure(
                            "Failed to upload product image",
                            StatusCodes.Status500InternalServerError
                        );
                    }

                    product.ImageUrl = imageUrl;
                }
                if (!string.IsNullOrEmpty(request.ProductName))
                    product.ProductName = request.ProductName;

                if (!string.IsNullOrEmpty(request.Description))
                    product.Description = request.Description;

                if (request.Prize.HasValue)
                    product.Prize = request.Prize;

                if (request.Status.HasValue)
                    product.Status = request.Status;
                product.UpdateAt = DateTime.Now;
                product.UpdateBy = userId;

                _uow.GetRepository<Product>().UpdateAsync(product);

                if (request.ComboItems != null && request.ComboItems.Any())
                {
                    foreach (var item in comboItems.ToList())
                    {
                        _uow.GetRepository<Comboltem>().DeleteAsync(item);
                    }
                    foreach (var comboItem in request.ComboItems)
                    {
                        var productExists = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: p => p.ProductId == comboItem.ProductId);
                        if (productExists == null)
                        {
                            return new MethodResult<ProductResponse>.Failure(
                                $"Product with ID {comboItem.ProductId} not found for combo item",
                                StatusCodes.Status404NotFound
                            );
                        }

                        var newComboItem = new Comboltem
                        {
                            Combod = product.ProductId,
                            ProductId = comboItem.ProductId,
                            Quantity = comboItem.Quantity,
                            MasterId = comboItem.MasterId,
                            Discount = comboItem.Discount
                        };

                        await _uow.GetRepository<Comboltem>().InsertAsync(newComboItem);
                        await _uow.CommitAsync();
                    }
                }
                await _uow.CommitTransactionAsync();
                var productResponse = _mapper.Map<ProductResponse>(product);

                await EnrichProductDetails(new List<ProductResponse> { productResponse });

                return new MethodResult<ProductResponse>.Success(productResponse);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return new MethodResult<ProductResponse>.Failure(
                    $"Error updating combo product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        public async Task<MethodResult<bool>> UpdateProductStatusAsync(int userId, int productId)
        {
            try
            {
                var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(
                    predicate: p => p.ProductId == productId
                );
                if (product == null)
                {
                    return new MethodResult<bool>.Failure(
                        "Product not found",
                        StatusCodes.Status404NotFound
                    );
                }
                await _uow.BeginTransactionAsync();
                product.DisableAt = DateTime.Now;
                product.DisableBy = userId;
                product.Status = product.Status == true ? false : true;
                if (product.ProductType == ProductConstant.PRODUCT_TYPE_MATTER_PRODUCT)
                {
                    var sizeProducts = await _uow.GetRepository<Product>().GetListAsync(
                        predicate: p => p.ParentId == product.ProductId
                    );
                    foreach (var sizeProduct in sizeProducts)
                    {
                        sizeProduct.Status = product.Status;
                        sizeProduct.DisableAt = DateTime.Now;
                        sizeProduct.DisableBy = userId;
                        _uow.GetRepository<Product>().UpdateAsync(sizeProduct);
                    }
                }
                _uow.GetRepository<Product>().UpdateAsync(product);
                await _uow.CommitAsync();
                return new MethodResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return new MethodResult<bool>.Failure(
                    $"Error deleting product: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }

}

