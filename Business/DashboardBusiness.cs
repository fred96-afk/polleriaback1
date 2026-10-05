using DbModel;
using IBusiness;
using IRepository;
using Models.Dashboard;
using DbModel.Tables;

namespace Business;

public class DashboardBusiness(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IOrderDetailRepository orderDetailRepository,
    IClientRepository clientRepository) : IDashboardBusiness
{
    public async Task<DashboardResponse> GetDashboardDataAsync()
    {
        var allOrders = await orderRepository.GetAllAsync();
        var ordersList = allOrders.ToList();

        var totalRevenue = ordersList.Sum(o => o.TotalAmount);
        var totalOrders = ordersList.Count;
        
        // As there's no status yet, let's assume all orders are completed or use a dummy for now
        var pendingOrders = 0; 

        var allProducts = await productRepository.GetAllAsync();
        var totalProducts = allProducts.Count();

        // Top 5 products
        var allDetails = await orderDetailRepository.GetAllAsync();
        // Since we need product names, we might need a more efficient query, 
        // but for now let's use what we have.
        var topProducts = allDetails
            .GroupBy(d => d.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(d => d.Quantity) })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToList();

        var topProductResponses = new List<TopProductResponse>();
        foreach (var tp in topProducts)
        {
            var product = await productRepository.GetByIdAsync(tp.ProductId);
            topProductResponses.Add(new TopProductResponse(product?.Name ?? "Unknown", tp.Quantity));
        }

        // Sales Last 7 Days
        var sevenDaysAgo = PeruTimeHelper.Now.Date.AddDays(-7);
        var salesLast7Days = ordersList
            .Where(o => o.OrderDate >= sevenDaysAgo)
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new SalesByDayResponse(g.Key.ToString("dd/MM"), g.Sum(o => o.TotalAmount)))
            .OrderBy(x => x.Day)
            .ToList();

        // Recent Orders (last 10)
        var recentOrdersRaw = ordersList
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .ToList();

        var clients = await clientRepository.GetAllAsync();
        var clientDict = clients.ToDictionary(c => c.Id, c => c.Name);

        var recentOrders = recentOrdersRaw.Select(o => new RecentOrderResponse(
            o.Id,
            o.ClientId.HasValue && clientDict.TryGetValue(o.ClientId.Value, out var name) ? name : "General",
            o.TotalAmount,
            o.OrderDate
        )).ToList();

        return new DashboardResponse(
            totalRevenue,
            totalOrders,
            pendingOrders,
            totalProducts,
            topProductResponses,
            salesLast7Days,
            recentOrders
        );
    }
}
