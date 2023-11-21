namespace IWantApp.Endpoints.Orders;

public record OrderResponse(Guid Id, string ClientEmail, IEnumerable<OrderProduct> Products, string DeliveryAdress);
public record OrderProduct(Guid id, string Name);

