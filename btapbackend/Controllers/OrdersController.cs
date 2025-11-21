using Microsoft.AspNetCore.Mvc;
using btapbackend.Data;
using btapbackend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context) => _context = context;

    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // GET: api/orders/myorders → Chỉ xem đơn của mình
    [HttpGet("myorders")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == CurrentUserId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Items = o.OrderDetails.Select(od => new OrderItemDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            })
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/orders → Chỉ Admin xem tất cả đơn
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
            .Include(o => o.Customer)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerName = o.Customer.Name,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                Items = o.OrderDetails.Select(od => new OrderItemDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    // POST: api/orders → User tạo đơn hàng
    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto dto)
    {
        if (!dto.Items.Any()) return BadRequest("Đơn hàng phải có ít nhất 1 sản phẩm");

        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        var order = new Order
        {
            CustomerId = CurrentUserId,
            OrderDate = DateTime.Now,
            Status = "Pending",
            TotalAmount = 0
        };

        decimal total = 0;
        foreach (var item in dto.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) return BadRequest($"Sản phẩm ID {item.ProductId} không tồn tại");
            if (product.Stock < item.Quantity) return BadRequest($"Sản phẩm {product.Name} không đủ hàng");

            total += product.Price * item.Quantity;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

            product.Stock -= item.Quantity; // trừ tồn kho
        }

        order.TotalAmount = total;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyOrders), new { id = order.Id }, "Tạo đơn hàng thành công");
    }
}

// DTOs
public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; }
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
    public int Quantity { get; set; }
}

public class OrderResponseDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } // chỉ Admin thấy
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}