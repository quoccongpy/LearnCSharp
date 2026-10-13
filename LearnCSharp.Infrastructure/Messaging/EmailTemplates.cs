namespace LearnCSharp.Infrastructure.Messaging
{
    public static class EmailTemplates
    {
        public static string OrderCreated(string fullName, int orderId, decimal totalMoney, DateTime orderDate)
        {
            return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <style>
                    body {font - family: Arial, sans-serif; background: #f4f4f4; margin: 0; padding: 0; }
                    .container {max - width: 600px; margin: 30px auto; background: #fff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
                    .header {background: linear-gradient(135deg, #667eea, #764ba2); padding: 30px; text-align: center; color: white; }
                    .header h1 {margin: 0; font-size: 24px; }
                    .body {padding: 30px; }
                    .info-box {background: #f8f9fa; border-left: 4px solid #667eea; padding: 15px 20px; border-radius: 4px; margin: 20px 0; }
                    .info-row {display: flex; justify-content: space-between; margin: 8px 0; }
                    .label {color: #666; }
                    .value {font - weight: bold; color: #333; }
                    .total {font - size: 20px; color: #667eea; font-weight: bold; }
                    .footer {background: #f8f9fa; padding: 20px; text-align: center; color: #999; font-size: 13px; }
                    .btn {display: inline-block; background: #667eea; color: white; padding: 12px 30px; border-radius: 25px; text-decoration: none; margin-top: 20px; }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="header">
                        <h1>🛒 Đặt hàng thành công!</h1>
                        <p>Cảm ơn bạn đã tin tưởng chúng tôi</p>
                    </div>
                    <div class="body">
                        <p>Xin chào <strong>{{fullName}}</strong>,</p>
                        <p>Đơn hàng của bạn đã được đặt thành công. Chúng tôi sẽ xử lý và thông báo cho bạn sớm nhất.</p>

                        <div class="info-box">
                            <div class="info-row">
                                <span class="label">Mã đơn hàng:</span>
                                <span class="value">#ORD-{{orderId:D6}}</span>
                            </div>
                            <div class="info-row">
                                <span class="label">Ngày đặt:</span>
                                <span class="value">{{orderDate:dd/MM/yyyy HH:mm}}</span>
                            </div>
                            <div class="info-row">
                                <span class="label">Tổng tiền:</span>
                                <span class="value total">{{totalMoney:N0}} VNĐ</span>
                            </div>
                        </div>

                        <a href="http://localhost:5173/orders/{{orderId}}" class="btn">Xem đơn hàng</a>
                    </div>
                    <div class="footer">
                        <p>© 2026 LearnCSharp Shop. Mọi thắc mắc liên hệ support@learncsharp.com</p>
                    </div>
                </div>
            </body>
            </html>
            """;
        }

        public static string OrderStatusChanged(string fullName, int orderId, string newStatus, string message)
        {
            var (emoji, color) = newStatus switch
            {
                "Confirmed" => ("✅", "#28a745"),
                "Preparing" => ("👨‍🍳", "#fd7e14"),
                "ReadyForPickup" => ("📦", "#17a2b8"),
                "Delivering" => ("🛵", "#007bff"),
                "Delivered" => ("🎉", "#28a745"),
                "Cancelled" => ("❌", "#dc3545"),
                "Rejected" => ("⛔", "#dc3545"),
                _ => ("📋", "#6c757d")
            };

            return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <style>
                    body {font - family: Arial, sans-serif; background: #f4f4f4; margin: 0; padding: 0; }
                    .container {max - width: 600px; margin: 30px auto; background: #fff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
                    .header {background: {color}; padding: 30px; text-align: center; color: white; }
                    .header h1 {margin: 0; font-size: 24px; }
                    .body {padding: 30px; }
                    .status-badge {display: inline-block; background: {color}; color: white; padding: 6px 18px; border-radius: 20px; font-weight: bold; }
                    .footer {background: #f8f9fa; padding: 20px; text-align: center; color: #999; font-size: 13px; }
                    .btn {display: inline-block; background: {color}; color: white; padding: 12px 30px; border-radius: 25px; text-decoration: none; margin-top: 20px; }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="header">
                        <h1>{{emoji}} Cập nhật đơn hàng #{orderId}</h1>
                    </div>
                    <div class="body">
                        <p>Xin chào <strong>{{fullName}}</strong>,</p>
                        <p>{{message}}</p>
                        <p>Trạng thái hiện tại: <span class="status-badge">{{newStatus}}</span></p>
                        <a href="http://localhost:5173/orders/{{orderId}}" class="btn">Xem đơn hàng</a>
                    </div>
                    <div class="footer">
                        <p>© 2026 LearnCSharp Shop.</p>
                    </div>
                </div>
            </body>
            </html>
            """;
        }
    }
}