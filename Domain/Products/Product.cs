using Flunt.Validations;
using IWantApp.Domain.Orders;

namespace IWantApp.Domain.Products;

public class Product : Entity
{
    public string Name { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public string Description { get; private set; }
    public bool HasStock { get; private set; }
    public bool Active { get; private set; } = true;
    public decimal Price { get; private set; }
    public ICollection<Order> Orders { get; private set; }

    private Product() { }

    public Product(string name, Guid categoryId, Category category, string description, bool hasStock, bool active, decimal price, string createdBy)
    {
        Name = name;
        CategoryId = categoryId;
        Category = category;
        Description = description;
        HasStock = hasStock;
        Active = active;
        Price = price;

        CreatedBy = createdBy;
        EditedBy = createdBy;
        CreatedOn = DateTime.Now;
        EditedOn = DateTime.Now;
        Validate();
    }
    private void Validate()
    {
        var contract = new Contract<Category>()
                    .IsNotNullOrEmpty(Name, "Name")
                    .IsGreaterOrEqualsThan(Name, 3, "Name")
                    .IsNotNull(Category, "Category", "Category not found!")
                    .IsNotNullOrEmpty(Description, "Description")
                    .IsGreaterOrEqualsThan(Description, 3, "Description")
                    .IsGreaterOrEqualsThan(Price, 1, "Price")
                    .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
                    .IsNotNullOrEmpty(EditedBy, "EditedBy");
        AddNotifications(contract);
    }
}
