﻿public record ProductResponse(Guid id, string Name, string CategoryName, string Description, bool HasStock, decimal price, bool Active);
public record ProductSoldResponseReport(Guid Id, string Name, int Amount);