using Core.DTOs.Customers;

namespace Core.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<long> CreateAsync(CreateCustomerRequestDto request);
    Task<bool> UpdateAsync(UpdateCustomerRequestDto request);
    Task<bool> UpdateStatusAsync(UpdateCustomerStatusRequestDto request);
    Task<CustomerDto?> GetByIdAsync(long tenantId, long customerId);
    Task<PagedCustomersDto> ListAsync(CustomerListQueryDto query);
}
