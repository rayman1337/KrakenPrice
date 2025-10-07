using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.WebHost.UseUrls($"http://0.0.0.0:5000");

var app = builder.Build();
app.UseCors();

var cache = app.Services.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
var httpFactory = app.Services.GetRequiredService<IHttpClientFactory>();

const string HTML = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>BTC Price - NZXT Kraken</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
            color: #ffffff;
            overflow: hidden;
            position: relative;
        }

        .kraken-display::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: linear-gradient(45deg, #f7931a, #ff9500, #f7931a, #ffb84d);
            background-size: 400% 400%;
            animation: gradientShift 15s ease infinite;
            opacity: 0.1;
            z-index: 0;
        }

        @keyframes gradientShift {
            0% { background-position: 0% 50%; }
            50% { background-position: 100% 50%; }
            100% { background-position: 0% 50%; }
        }

        .particles {
    position: absolute;
    width: 100%;
    height: 100%;
    overflow: hidden;
    z-index: 3; 
}

     .particle {
    position: absolute;
    width: 6px;  
    height: 6px;
    background: rgba(247, 147, 26, 1); 
    border-radius: 50%;
    box-shadow: 0 0 15px rgba(247, 147, 26, 1); 
    animation: float-particle 20s infinite;
}

        @keyframes float-particle {
            0% { transform: translateY(100vh) translateX(0); opacity: 0; }
            10% { opacity: 1; }
            90% { opacity: 1; }
            100% { transform: translateY(-100vh) translateX(100px); opacity: 0; }
        }

        .kraken-display {
            width: 100vw;
            height: 100vh;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 15px;
            position: relative;
            z-index: 2;
        }

        .bitcoin-logo {
            width: 60px;
            height: 60px;
            margin-bottom: 12px;
            animation: pulse 2s ease-in-out infinite;
            filter: drop-shadow(0 0 15px rgba(247, 147, 26, 0.8));
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.05); }
        }

.price-container {
    text-align: center;
    background: rgba(255, 255, 255, 0.98);
    padding: 30px 30px;
    border-radius: 20px; /* Changed from 50% */
    backdrop-filter: blur(10px);
    border: 2px solid rgba(247, 147, 26, 0.3);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3), 0 0 40px rgba(247, 147, 26, 0.2);
    animation: fadeIn 0.5s ease-out;
    width: 300px; 
    height: 300px; 
    display: flex; 
    flex-direction: column; 
    justify-content: center; 
    align-items: center;
    aspect-ratio: 1 / 1;
}

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .currency-label {
            font-size: 13px;
            font-weight: 700;
            letter-spacing: 2px;
            opacity: 0.7;
            margin-bottom: 6px;
            color: #f7931a;
        }

        .price {
            font-size: 55px;
            font-weight: 900;
            background: linear-gradient(135deg, #f7931a, #ff9500);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
            margin-bottom: 8px;
            animation: priceGlow 2s ease-in-out infinite;
            letter-spacing: -1px;
            line-height: 1;
        }

        @keyframes priceGlow {
            0%, 100% { filter: brightness(1); }
            50% { filter: brightness(1.15); }
        }

        .source {
            font-size: 11px;
            opacity: 0.6;
            margin-top: 8px;
            font-weight: 600;
            color: #666;
        }

        .last-updated {
            font-size: 10px;
            opacity: 0.5;
            margin-top: 6px;
            font-weight: 500;
            color: #888;
        }

        .error {
            color: #ef4444;
            font-size: 13px;
            padding: 12px;
            background: rgba(239, 68, 68, 0.1);
            border-radius: 8px;
            animation: shake 0.5s ease;
            max-width: 300px;
        }

        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }

        .loading {
            font-size: 16px;
            opacity: 0.8;
            animation: blink 1.5s ease-in-out infinite;
            color: #f7931a;
        }

        @keyframes blink {
            0%, 100% { opacity: 0.8; }
            50% { opacity: 0.3; }
        }
    </style>
</head>
<body>
    <div class=""particles"" id=""particles""></div>
    <div id=""app""></div>

    <script>
        function createParticles() {
            const particles = document.getElementById('particles');
            for (let i = 0; i < 15; i++) {
                const particle = document.createElement('div');
                particle.className = 'particle';
                particle.style.left = Math.random() * 100 + '%';
                particle.style.animationDelay = Math.random() * 20 + 's';
                particle.style.animationDuration = (Math.random() * 10 + 15) + 's';
                particles.appendChild(particle);
            }
        }
        createParticles();

        const searchParams = new URLSearchParams(window.location.search);
        const isKraken = searchParams.get(""kraken"") === ""1"";

        const DEFAULT_CONFIG = {
            refreshInterval: 30,
            apiUrl: '/api/btc'
        };

        let config = {...DEFAULT_CONFIG};

        class BTCPriceDisplay {
            constructor() {
                this.price = null;
                this.source = null;
                this.lastUpdated = null;
                this.error = null;
                this.init();
            }

            async fetchPrice() {
                try {
                    const response = await fetch(config.apiUrl);
                    
                    if (!response.ok) throw new Error('Failed to fetch price');
                    
                    const data = await response.json();
                    
                    if (!data.success) {
                        throw new Error(data.source || 'API Error');
                    }
                    
                    this.price = data.price;
                    this.source = data.source;
                    this.lastUpdated = new Date();
                    this.error = null;
                } catch (err) {
                    this.error = 'Unable to fetch BTC price';
                    console.error(err);
                }
            }

            formatPrice(price) {
                return `$${price.toLocaleString('en-US', { 
                    minimumFractionDigits: 0,
                    maximumFractionDigits: 0
                })}`;
            }

            renderKraken() {
                const app = document.getElementById('app');
                
                if (this.error) {
                    app.innerHTML = `
                        <div class=""kraken-display"">
                            <div class=""error"">${this.error}</div>
                        </div>
                    `;
                    return;
                }

                if (!this.price) {
                    app.innerHTML = `
                        <div class=""kraken-display"">
                            <div class=""loading"">? Loading BTC Price...</div>
                        </div>
                    `;
                    return;
                }

                const timeStr = this.lastUpdated.toLocaleTimeString();

                app.innerHTML = `
                    <div class=""kraken-display"">
                        <div class=""price-container"">
                            <div class=""currency-label"">BITCOIN</div>
                            <div class=""price"">${this.formatPrice(this.price)}</div>
                            <div class=""source"">${this.source}</div>
                            <div class=""last-updated"">${timeStr}</div>
                        </div>
                    </div>
                `;
            }

            startAutoRefresh() {
                if (this.refreshTimer) {
                    clearInterval(this.refreshTimer);
                }
                
                this.refreshTimer = setInterval(() => {
                    this.fetchPrice().then(() => {
                        this.renderKraken();
                    });
                }, config.refreshInterval * 1000);
            }

            async init() {
                await this.fetchPrice();
                this.renderKraken();
                this.startAutoRefresh();
            }
        }

        const app = new BTCPriceDisplay();
    </script>
</body>
</html>
";


app.MapGet("/", () => Results.Content(HTML, "text/html"));

app.MapGet("/api/btc", async () =>
{

    var (success, price, source) = await GetBitcoinPrice(httpFactory);

    var result = new
    {
        success = success,
        price = decimal.Parse(price),
        source = source
    };

    return Results.Ok(result);

});

app.Run();


static async Task<(bool success, string price, string source)> GetBitcoinPrice(IHttpClientFactory httpFactory)
{
    var httpClient = httpFactory.CreateClient();
    httpClient.Timeout = TimeSpan.FromSeconds(5); 

    // Try Binance first 
    try
    {
        var json = await httpClient.GetStringAsync("https://api.binance.com/api/v3/ticker/24hr?symbol=BTCUSDT");
        var doc = JsonDocument.Parse(json);
        var price = doc.RootElement.GetProperty("lastPrice").GetString();
        return (true, price!, "Binance");
    }
    catch { }

    // Fallback 1: Coinbase
    try
    {
        var json = await httpClient.GetStringAsync("https://api.coinbase.com/v2/prices/BTC-USD/spot");
        var doc = JsonDocument.Parse(json);
        var price = doc.RootElement.GetProperty("data").GetProperty("amount").GetString();
        return (true, price!, "Coinbase");
    }
    catch { }

    // Fallback 2: Kraken
    try
    {
        var json = await httpClient.GetStringAsync("https://api.kraken.com/0/public/Ticker?pair=XBTUSD");
        var doc = JsonDocument.Parse(json);
        var price = doc.RootElement
            .GetProperty("result")
            .GetProperty("XXBTZUSD")
            .GetProperty("c")[0]
            .GetString();
        return (true, price!, "Kraken");
    }
    catch { }

    // Fallback 3: Bitstamp
    try
    {
        var json = await httpClient.GetStringAsync("https://www.bitstamp.net/api/v2/ticker/btcusd");
        var doc = JsonDocument.Parse(json);
        var price = doc.RootElement.GetProperty("last").GetString();
        return (true, price!, "Bitstamp");
    }
    catch { }

    // All failed
    return (false, "", "All APIs failed");
}